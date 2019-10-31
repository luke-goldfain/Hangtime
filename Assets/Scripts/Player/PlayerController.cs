using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Limit on the distance of the grappling hook.")]
    public float GrappleDistance = 30f;
    [Tooltip("SOFT airborne speed limit, increased by the number of consecutive grapples in the air.")]
    public float SpeedLimit = 60f;
    [Tooltip("The force with which the grappling hook pulls the player.")]
    public float GrappleForce = 30f;
    [Tooltip("The default speed at which the player can move while on the ground. Sliding and landing while moving quickly temporarily circumvent this.")]
    public float DefaultRunSpeed = 25f;
    [Tooltip("The force with which the player jumps.")]
    public float JumpForce = 10f;
    [Tooltip("The maximum force with which the player can jump, applicable only to \"rebound\" jumps.")]
    public float MaxReboundJumpForce = 30f;
    [Tooltip("The time the grappling hook takes to travel to its destination.")]
    public float GrappleTravelTime = 0.3f;
    [Tooltip("The height at which the player will respawn at their last reached checkpoint.")]
    public float RespawnHeight = -20f;

    [SerializeField]
    private GameObject grapplingHookPrefab;


    [SerializeField]
    private GameObject ropeSectionPrefab;

    [SerializeField]
    public GameObject HUD;

    [SerializeField]
    public GameObject windZone;

    [SerializeField]
    private GameObject slidePRight, slidePLeft;
    //public GameObject Reticle, Speedometer, CheckpointMeter, CheckpointMeterFill;

    [HideInInspector]
    public GameObject Reticle, Speedometer, SpeedometerText,
                      SpeedometerNeedle, PCompass, CheckpointMeter,
                      CheckpointMeterFill, PlacementText, CheckpointText,
                      ModeIndicator, ObjectiveIndicator, PlayerMinimap;

    [SerializeField]
    private Sprite pullIcon, swingIcon;

    [SerializeField]
    public int PlayerNumber;

    [SerializeField]
    public LayerMask GrapplableMask;

    public bool AcceptsInput = true;

    [SerializeField]
    private bool inWindZone = false;

    private GameObject countdownObject;

    private bool inCountdown;

    private int numberOfPlayers;
    
    private string playerFireButton;
    private string playerJumpButton;
    private string playerSlideButton;
    private string playerGSwitchButton;
    private string playerVerticalAxis;
    private string playerHorizontalAxis;

    private bool triggerPressed;
    private bool playerFiring;
    private bool playerStoppedFiring;

    private bool isPulling; // Determines whether to pull or swing when grappling. Player-switchable via playerGSwitchButton at (nearly) any time.

    private GameObject grapplingHook;

    private List<GameObject> ropeSections;

    private Rigidbody rb;

    private float currentRunSpeed; // The current running speed of the player, changed on the fly (literally) and lerping towards DefaultRunSpeed while on the ground.

    private Vector3 lastGrapplableRaycastPoint;
    private float grapplableTimer;
    private readonly float grapplableMaxTime = 0.25f; // The amount of time in seconds a "passed" grapple point is still grapplable.

    public Vector3 GrappleHitPosition { get; private set; }

    private Vector3 grappleStartPosition;

    private float startOfSwingSpd;

    private Vector3 grappleDir;

    private float currentGrappleDistance;

    private GameObject currentGrappledObject;

    private Vector3 currentGrappleNormal;

    private Transform cameraTransform;

    private float cameraX, cameraY, cameraW, cameraH;

    private Vector3 airVelocity;
    private Vector3 incidenceVelocity;
    private Vector3 collisionNormal;

    private float groundTimer; // The active timer determining how long the player has been on the ground.
    private readonly float angleJumpCooldown = 0.25f; // The amount of time in seconds the player has to "rebound" jump angularly out of a collision.

    private float slideTimer;
    private readonly float slideMaxTime = 1f; // The amount of time in seconds the player may slide for.

    private readonly float defaultFriction = 1f;
    private readonly float slideFriction = 0.05f;
    
    private bool canWalkOnGround;

    private bool hasAirDashed;

    private bool grappleCasted;

    private bool hasGrappledMovableObject;

    private float currentGrappleTravelTime;

    // consecutiveGrapples keeps track of how many times the player has
    // grappled since they touched a surface. Max Speed increases multiplicatively  
    // with each one up to a maximum of 4.
    private int consecutiveGrapples;

    // bool necessary because there is no method for no collision.
    private bool colliding;

    // TODO: Add a "finished" state that zooms camera out and displays player character in third person
    private enum State
    {
        inAir,
        onGround,
        grappling
    }
    private State currentState;
    private State prevState;

    // Start is called before the first frame update
    void Start()
    {
        numberOfPlayers = GameStats.NumOfPlayers;

        StartAssignInputButtons();
        
        StartAssignHUDObjects();

        PlacementText.SetActive(false);

        rb = this.GetComponent<Rigidbody>();

        ropeSections = new List<GameObject>();

        Cursor.lockState = CursorLockMode.Locked;

        isPulling = true;

        // Don't allow input at start for countdown
        AcceptsInput = false;
        inCountdown = true;
    }

    private void StartAssignHUDObjects()
    {
        Reticle = HUD.transform.Find("Reticle").gameObject;
        Speedometer = HUD.transform.Find("SpeedometerV2").gameObject;
        SpeedometerText = Speedometer.transform.Find("Speed").gameObject;
        SpeedometerNeedle = Speedometer.transform.Find("MainBody").transform.Find("Needle").gameObject;
        PCompass = Speedometer.transform.Find("Compass").gameObject;
        CheckpointMeter = HUD.transform.Find("CheckpointMeter").gameObject;
        CheckpointMeterFill = HUD.transform.Find("CheckpointMeterFill").gameObject;
        PlacementText = HUD.transform.Find("PlacementText").gameObject;
        CheckpointText = CheckpointMeter.transform.Find("CheckpointText").gameObject;
        ModeIndicator = HUD.transform.Find("ModeIndicator").gameObject;
        ObjectiveIndicator = HUD.transform.Find("ObjectiveParent").gameObject;
        PlayerMinimap = HUD.transform.Find("MiniCam").gameObject;

        countdownObject = GameObject.FindGameObjectWithTag("Countdown");
    }

    // Collect variables for jump-off angle when colliding with an object
    void OnCollisionEnter(Collision collision)
    {
        collisionNormal = collision.GetContact(0).normal;

        incidenceVelocity = Vector3.Reflect(airVelocity, collisionNormal);
    }

    private void OnCollisionStay(Collision collision)
    {
        colliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        colliding = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cameraTransform = this.GetComponentInChildren<PlayerCameraController>().transform;

        // Restore input once initial countdown is finished. inCountdown variable 
        // required to allow other events to remove input.
        if (!AcceptsInput && inCountdown && countdownObject.GetComponent<Countdown>().IsFinished)
        {
            inCountdown = false;
            AcceptsInput = true;
        }

        UpdateSpeedometer();

        // In lieu of a GetAxisDown and GetAxisUp function, the next couple of if statements simulate them.
        if (Input.GetAxis(playerFireButton) <= 0 && triggerPressed)
        {
            triggerPressed = false;

            playerStoppedFiring = true;
        }
        else
        {
            playerStoppedFiring = false;
        }

        if (Input.GetAxis(playerFireButton) > 0 && !triggerPressed)
        {
            triggerPressed = true;

            playerFiring = true;
        }
        else
        {
            playerFiring = false;
        }

        UpdateCheckGrapplability();

        if (currentState != State.grappling)
        {
            if (colliding)
            {
                currentState = State.onGround;
            }
            else
            {
                currentState = State.inAir;
            }
        }

        // Change the grapple mode (pull or swing) with the assigned button.
        if (Input.GetButtonDown(playerGSwitchButton) && AcceptsInput)
        {
            isPulling = !isPulling;

            // Change the mode indicator icon to indicate which mode the player is in: swing or pull.
            if (ModeIndicator.GetComponent<Image>().sprite == pullIcon)
            {
                ModeIndicator.GetComponent<Image>().sprite = swingIcon;
            }
            else
            {
                ModeIndicator.GetComponent<Image>().sprite = pullIcon;
            }
        }

        if ((Input.GetButtonDown(playerFireButton) || playerFiring) && AcceptsInput)
        {
            CastGrapple();
        }

        if (grappleCasted)
        {
            currentGrappleTravelTime += Time.deltaTime;

            // Make the grappling hook travel from the player to the grapple point over the time it takes for the grapple to "travel".
            grapplingHook.transform.position = this.transform.position + (GrappleHitPosition - this.transform.position) * (currentGrappleTravelTime / GrappleTravelTime);
        }

        if (currentGrappleTravelTime >= GrappleTravelTime && (Input.GetButton(playerFireButton) || triggerPressed))
        {
            currentGrappleTravelTime = 0f;

            grappleCasted = false;

            currentState = State.grappling;

            grappleStartPosition = this.transform.position;

            grappleDir = Vector3.Normalize(GrappleHitPosition - this.transform.position);

            // create rope sections
            for (int i = 1; i <= 3; i++)
            {
                ropeSections.Add(Instantiate(ropeSectionPrefab, this.transform.position + (((GrappleHitPosition - this.transform.position) / i) * (currentGrappleTravelTime / GrappleTravelTime)), Quaternion.identity, this.transform));

                ropeSections[i - 1].GetComponent<RopeSectionPositioner>().SectionNumber = i;
            }
        }

        if ((Input.GetButtonUp(playerFireButton) || playerStoppedFiring) && AcceptsInput)
        {
            currentState = State.inAir;

            grappleCasted = false;

            Destroy(grapplingHook);
        }

        if (prevState != currentState)
        {
            UpdateOneTimeStateActions();

            prevState = currentState;
        }

        // TODO: Add a "finished" state that zooms camera out and displays player character in third person
        switch (currentState)
        {
            case State.inAir:
                UpdateMoveAir();
                break;
            case State.onGround:
                UpdateMoveGround();
                break;
            case State.grappling:
                UpdateMoveGrappling();
                break;
        }

        // If the player has grappled (is not on ground or in default jump), clamp magnitude by speed limit plus a ratio based on the number of consecutive grapples
        if (consecutiveGrapples > 0 && rb.velocity.y >= 0)
        {
            LerpSpeedToLimit();
        }

        // Reset position if player falls out of the world (the value of RespawnHeight should be set accordingly).
        if (Vector3.Distance(Vector3.zero, this.transform.position) > 100f && this.transform.position.y < RespawnHeight)
        {
            List<GameObject> cpHit = this.GetComponent<CheckpointController>().CheckpointsHit;

            // Reset player to their last hit checkpoint, or to the starting point.
            if (cpHit.Count == 0)
            {
                this.transform.position = GameObject.Find("SpawnManager").GetComponent<SpawnManager>().InitialSpawnPoints[PlayerNumber - 1];
            }
            else
            {
                this.transform.position = cpHit[cpHit.Count - 1].transform.position + (Vector3.up * 3);
            }

            rb.velocity = Vector3.zero;
        }

        // Debug escape to lock/unlock cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Conditional operator: if cursor unlocked, lock cursor, otherwise unlock cursor
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked)? CursorLockMode.None : CursorLockMode.Locked;
        }

        // Get ForceField component and allows player to edit the effects of ForceField Colliders
        if (inWindZone)
        {
               rb.AddForce(windZone.GetComponent<ForceField>().ForceDirection * windZone.GetComponent<ForceField>().ForceStrength);
        }

    }

    void OnTriggerEnter(Collider coll) // start applying force when player enters ForceField's collider
    {
        if(coll.gameObject.tag == "ForceField")
        {
           windZone = coll.gameObject;
           inWindZone = true;
        }
    }

    void OnTriggerExit(Collider coll) // Stops applying force when player leaves ForceField's collider
    {
        if(coll.gameObject.tag == "ForceField")
        {
          inWindZone = false;
        }
    }

    private void UpdateSpeedometer()
    {
        SpeedometerNeedle.GetComponent<SpeedConverter>().ShowSpeed(this.rb.velocity.magnitude, 0f, 100f);

        SpeedometerText.GetComponent<Text>().text = Mathf.RoundToInt(this.rb.velocity.magnitude).ToString();

    }

    private void UpdateOneTimeStateActions()
    {
        switch (currentState)
        {
            case State.inAir:
                {

                    hasAirDashed = false;

                    this.GetComponent<Collider>().material.dynamicFriction = defaultFriction;

                    foreach (GameObject r in ropeSections)
                    {
                        Destroy(r);
                    }

                    ropeSections.Clear();

                    if (slidePLeft.GetComponent<ParticleSystem>().isPlaying)
                    {
                        slidePLeft.GetComponent<ParticleSystem>().Stop();
                        slidePRight.GetComponent<ParticleSystem>().Stop();
                    }

                    break;
                }
            case State.onGround:
                {
                    canWalkOnGround = true;

                    groundTimer = 0f;
                    slideTimer = 0f;

                    consecutiveGrapples = 0;

                    foreach (GameObject r in ropeSections)
                    {
                        Destroy(r);
                    }

                    ropeSections.Clear();

                    break;
                }
            case State.grappling:
                {
                    this.GetComponent<Collider>().material.dynamicFriction = defaultFriction;

                    startOfSwingSpd = this.rb.velocity.magnitude;

                    if (consecutiveGrapples < 4) consecutiveGrapples++;
                    
                    currentGrappleDistance = Vector3.Distance(this.transform.position, GrappleHitPosition);

                    // Set the current grapple's normal, used when swinging, by a cross-product based on 
                    // the direction of the grapple and the player's velocity.
                    currentGrappleNormal = Vector3.Cross(grappleDir, -rb.velocity);

                    // Check if the player is moving slowly and is beneath their grapple point, and make 
                    // sure in that case that moveDir will face down by reversing the normal.
                    if (rb.velocity.magnitude < 8f && rb.velocity.y > 0f && this.transform.position.y < GrappleHitPosition.y )
                    {
                        currentGrappleNormal = Vector3.Cross(grappleDir, rb.velocity);
                    }

                    if (slidePLeft.GetComponent<ParticleSystem>().isPlaying)
                    {
                        slidePLeft.GetComponent<ParticleSystem>().Stop();
                        slidePRight.GetComponent<ParticleSystem>().Stop();
                    }

                    break;
                }
        }
    }

    private void UpdateMoveGrappling()
    {
        Vector3 moveDir = Vector3.up; // Default -- always overwritten

        if (hasGrappledMovableObject)
        {
            GrappleHitPosition = currentGrappledObject.transform.position + (currentGrappledObject.transform.position - GrappleHitPosition);
        }

        if (isPulling)
        {
            // Movement direction is  a spherical interpolation of the forward-facing direction and the direction of the grapple.
            // The last argument in this function determines the relative "strength" of the two Vector3's, with the grapple direction
            // remaining the strongest factor in movement direction.
            moveDir = Vector3.Slerp(cameraTransform.forward, grappleDir, 0.9f);
        }
        else
        {
            // Movement direction in swinging mode is the cross-product of the grapple's direction and the current grapple normal, which is
            // itself a cross-product of the grapple direction and the player's velocity at the start of their grapple.
            moveDir = Vector3.Cross(GrappleHitPosition - this.transform.position, currentGrappleNormal);
        }

        // If player is holding a direction (thumbstick, WASD) while they grapple, it slightly affects the direction of their grapple.
        if (AcceptsInput)
        {
            if (Input.GetAxis(playerHorizontalAxis) < 0)
            {
                moveDir = Vector3.Slerp(moveDir, -cameraTransform.right, .12f);
            }

            if (Input.GetAxis(playerHorizontalAxis) > 0)
            {
                moveDir = Vector3.Slerp(moveDir, cameraTransform.right, .12f);
            }

            if (Input.GetAxis(playerVerticalAxis) > 0)
            {
                moveDir = Vector3.Slerp(moveDir, cameraTransform.up, .12f);
            }

            if (Input.GetAxis(playerVerticalAxis) < 0)
            {
                moveDir = Vector3.Slerp(moveDir, -cameraTransform.up, .12f);
            }
        }

        if (isPulling) // if player is pulling, increase their grapple force based on number of consecutive grapples and distance from grapple point.
        {
            // Add force inverse to the length of the grapple (low length = high force). 
            // If distance is high enough that the force is below 15 * consecutiveGrapples, default to that.
            rb.AddForce(moveDir * Mathf.Max(GrappleForce - Vector3.Distance(GrappleHitPosition, this.transform.position), 15f * (consecutiveGrapples + 1)));

            // If the currently grappled object is movable, pull it towards the player at the same rate.
            if (hasGrappledMovableObject)
            {
                currentGrappledObject.GetComponent<Rigidbody>().AddForce(-moveDir * Mathf.Max(GrappleForce - Vector3.Distance(GrappleHitPosition, this.transform.position), 15f * (consecutiveGrapples + 1)));
            }
        }
        else // if player is not pulling, rotate them around the grapple point at a soft-fixed speed, ala spider man.
        {
            Vector3 swingVelocity = moveDir * GrappleForce * 100f;
            swingVelocity = Vector3.ClampMagnitude(swingVelocity, Mathf.Max(SpeedLimit * 0.6f, startOfSwingSpd * 1.3f));

            rb.velocity = Vector3.Lerp(rb.velocity, swingVelocity, 0.1f);
        }

        // If the player is pulling and has traveled 1.5 times the original length of the grapple (a ways past or away from the grapple point), break off the grapple automatically.
        // If the player is swinging and has traveled 2.6 times original grapple length, same deal.
        if ((Vector3.Distance(this.transform.position, grappleStartPosition) >= Vector3.Distance(GrappleHitPosition, grappleStartPosition) * 1.5f && isPulling) ||
            (Vector3.Distance(this.transform.position, grappleStartPosition) >= Vector3.Distance(GrappleHitPosition, grappleStartPosition) * 2.6f && !isPulling))
        {
            this.currentState = State.inAir;
        }

        // Set canHitGround to true if player is falling
        if (rb.velocity.y <= 0) canWalkOnGround = true;

        LerpSpeedToLimit();

        // Update currentRunSpeed so that when the player hits the ground, they are running relative to how fast they were grappling
        currentRunSpeed = rb.velocity.magnitude * 0.75f;
    }

    private void UpdateMoveAir()
    {
        if (AcceptsInput)
        {
            float hAxis = Input.GetAxisRaw(playerHorizontalAxis);
            float vAxis = Input.GetAxisRaw(playerVerticalAxis);

            // Give the player air control, relative to their current velocity so that it is always noticeable.
            if (Vector3.Angle(new Vector3(rb.velocity.x, 0f, rb.velocity.z), this.transform.TransformDirection(new Vector3(hAxis, 0, vAxis))) > 70)
            {
                rb.AddRelativeForce(new Vector3(hAxis, 0, vAxis) * rb.velocity.magnitude * 0.3f);
            }
            else
            {
                rb.AddRelativeForce(new Vector3(hAxis, 0, vAxis) * rb.velocity.magnitude * 0.02f);
            }

            LerpSpeedToLimit();

            // Assign airVelocity variable, used primarily for checking collision angles
            airVelocity = rb.velocity;

            // Allow the player to fall faster with the slide button.
            // Think of this as "fast-falling", like in Super Smash Bros.
            if (Input.GetButton(playerSlideButton) && rb.velocity.y > -20f)
            {
                rb.AddForce(Vector3.down * 10f);
            }

            // Reassign air velocity if the player jumps, allowing for air jumping (once per inAir-state)
            // The velocity assignment for air-jumping takes current velocity into account, and weighs it according to a Slerp.
            if (Input.GetButtonDown(playerJumpButton) && !hasAirDashed)
            {
                Vector3 airJumpDirection = (transform.forward * vAxis * JumpForce) + (transform.right * hAxis * JumpForce) + (transform.up * JumpForce);

                // Weigh the player's current velocity more if they are attempting to jump in a direction < 120 degrees from their current velocity.
                // Otherwise, clamp the magnitude of velocity as well (unclamped, the player would jump far too strongly).
                if (Vector3.Angle(new Vector3(rb.velocity.x, 0f, rb.velocity.z), airJumpDirection) < 120)
                {
                    rb.velocity += airJumpDirection;

                    if (rb.velocity.y < (JumpForce * 2f)) // Make sure y-velocity is not already higher than overwrite
                    {
                        rb.velocity = new Vector3(rb.velocity.x, JumpForce * 2f, rb.velocity.z); // Force y to a constant value
                    }
                }
                else
                {
                    rb.velocity = Vector3.Lerp(rb.velocity, airJumpDirection, 0.9f);

                    rb.velocity = new Vector3(rb.velocity.x, JumpForce * 2f, rb.velocity.z); // Force y to a constant value

                    rb.velocity = Vector3.ClampMagnitude(rb.velocity, (rb.velocity.magnitude + JumpForce) / 2f);
                }

                hasAirDashed = true;
            }

            if (rb.velocity.y <= 0) canWalkOnGround = true;
        }

        // Update currentRunSpeed so that when the player hits the ground, they are running relative to how fast they were flying
        currentRunSpeed = rb.velocity.magnitude * 0.8f;
    }

    private void UpdateMoveGround()
    {
        groundTimer += Time.deltaTime;

        if (AcceptsInput)
        {
            float hAxis = Input.GetAxisRaw(playerHorizontalAxis);
            float vAxis = Input.GetAxisRaw(playerVerticalAxis);

            if (Input.GetButtonDown(playerSlideButton))
            {
                slideTimer = 0f;
            }

            if (Input.GetButton(playerSlideButton) && slideTimer < slideMaxTime) // Sliding behavior
            {
                if (slidePLeft.GetComponent<ParticleSystem>().isStopped)
                {
                    slidePLeft.GetComponent<ParticleSystem>().Play();
                    slidePRight.GetComponent<ParticleSystem>().Play();
                }

                // Move the camera down to give the player feedback that they are sliding.
                cameraTransform.position = Vector3.Slerp(cameraTransform.position, this.transform.position + (Vector3.down * 0.8f), 0.2f);

                this.GetComponent<Collider>().material.dynamicFriction = slideFriction;

                rb.AddForce(((transform.forward * vAxis) + (transform.right * hAxis)) * 3f);
                rb.AddForce(-transform.up * 3f);

                // If the player is not on a flat surface and they are moving faster than the default run speed,
                // presume they are sliding downhill and refresh the slide timer.
                if (rb.velocity.magnitude > currentRunSpeed && collisionNormal.y != 1)
                {
                    slideTimer = 0f;
                }

                slideTimer += Time.deltaTime;
            }
            else if (canWalkOnGround && Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 2f, ~(1 << 8))) // Normal running behavior
            {
                if (hAxis != 0 || vAxis != 0)
                {
                    // Increase or decrease currentRunSpeed gradually until it matches DefaultRunSpeed.
                    currentRunSpeed = Mathf.Lerp(currentRunSpeed, DefaultRunSpeed, 0.02f); // Lerp towards DefaultRunSpeed.

                    rb.velocity = Vector3.ProjectOnPlane((transform.forward * vAxis * currentRunSpeed) + (transform.right * hAxis * currentRunSpeed), hit.normal);
                }
                else
                {
                    currentRunSpeed = rb.velocity.magnitude;

                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.02f);
                }

                rb.velocity = Vector3.ClampMagnitude(rb.velocity, currentRunSpeed);
            }

            if (Input.GetButtonUp(playerSlideButton) || slideTimer >= slideMaxTime) // What happens when a slide "ends"
            {
                if (slidePLeft.GetComponent<ParticleSystem>().isPlaying)
                {
                    slidePLeft.GetComponent<ParticleSystem>().Stop();
                    slidePRight.GetComponent<ParticleSystem>().Stop();
                }

                this.GetComponent<Collider>().material.dynamicFriction = defaultFriction;

                rb.velocity = Vector3.ClampMagnitude(rb.velocity, currentRunSpeed);

                // Move the camera back to give the player feedback that they are finished sliding.
                //cameraTransform.position = this.transform.position;
            }

            // If the player is not sliding and the camera is not back at default position, return it there.
            if ((slideTimer == 0f || slideTimer > slideMaxTime || !Input.GetButton(playerSlideButton)) && cameraTransform.position != this.transform.position)
            {
                cameraTransform.position = Vector3.Slerp(cameraTransform.position, this.transform.position, 0.3f);
            }

            if (Input.GetButtonDown(playerJumpButton))
            {
                // Set canWalkOnGround to false to get the player out of the ground. 
                // This gets set back to true once the player has moved downwards during a jump or grapple, or has otherwise collided with an object.
                canWalkOnGround = false;

                // To jump off of a surface based on angle and magnitude of incidence, the angle must be straighter than 75 degrees (which is almost flat to the
                //  surface), they must have been against the surface for < angleJumpCooldown seconds, and the velocity of the incidence must be high enough.
                if (Vector3.Angle(incidenceVelocity, collisionNormal) < 75f && groundTimer < angleJumpCooldown && incidenceVelocity.magnitude > 10f)
                {
                    rb.velocity = Vector3.ClampMagnitude(incidenceVelocity, MaxReboundJumpForce);
                }
                // If the previous parameters are not met, jump velocity is given by current velocity alongside the default jump force.
                // Note: When jumping off of a wall, this may feel unnatural as the player will jump straight up instead of jumping "off" the wall.
                else
                {
                    float tempJumpForce = Mathf.Max(JumpForce, Mathf.Min(rb.velocity.magnitude, MaxReboundJumpForce));

                    rb.velocity = (transform.forward * vAxis * tempJumpForce) + (transform.right * hAxis * tempJumpForce) + (transform.up * tempJumpForce);
                }

                this.currentState = State.inAir;
            }
        }
    }

    private void LerpSpeedToLimit()
    {
        // Only do this if ABOVE speed limit
        if (rb.velocity.magnitude > SpeedLimit + (this.SpeedLimit * 0.5f * (consecutiveGrapples - 1)))
        {
            // Soft-clamp speed to 3x speed limit, plus consecutive grapple bonus, to prevent coasting thru the air by holding a direction.
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.ClampMagnitude(rb.velocity, SpeedLimit + (this.SpeedLimit * 0.5f * (consecutiveGrapples - 1))), 0.05f);
        }
    }

    private void CastGrapple()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, cameraTransform.TransformDirection(Vector3.forward) * GrappleDistance, Color.blue, 1f);

        if (Physics.Raycast(this.transform.position, lastGrapplableRaycastPoint - cameraTransform.position, out hit, GrappleDistance, GrapplableMask) &&
            grapplableTimer < grapplableMaxTime)
        {
            currentGrappledObject = hit.collider.gameObject;

            GrappleHitPosition = hit.point;

            grappleCasted = true;

            currentGrappleTravelTime = 0f;

            grapplingHook = Instantiate(grapplingHookPrefab, this.transform.position, this.cameraTransform.rotation);

            if (hit.collider.gameObject.GetComponent<Rigidbody>() != null)
            {
                hasGrappledMovableObject = true;
            }
            else
            {
                hasGrappledMovableObject = false;
            }
        }
    }

    private void UpdateCheckGrapplability()
    {
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, GrappleDistance, GrapplableMask))
        {
            ResetReticlePosition();

            Reticle.GetComponent<Image>().color = new Color(0.2f, 0.6f, 1f);

            lastGrapplableRaycastPoint = hit.point;

            grapplableTimer = 0f;
        }
        else if (grapplableTimer < grapplableMaxTime)
        {
            Reticle.GetComponent<Image>().color = new Color(0.2f, 0.6f, 1f);

            Reticle.transform.position = this.GetComponentInChildren<Camera>().WorldToScreenPoint(lastGrapplableRaycastPoint);

            grapplableTimer += Time.deltaTime;
        }
        else
        {
            ResetReticlePosition();

            Reticle.GetComponent<Image>().color = new Color(0.9f, 0.3f, 0.3f);
        }
    }

    private void StartAssignInputButtons()
    {
        triggerPressed = false;
        playerFiring = true;

        playerFireButton = "P" + PlayerNumber + "Fire1";
        playerJumpButton = "P" + PlayerNumber + "Jump";
        playerSlideButton = "P" + PlayerNumber + "Slide";
        playerGSwitchButton = "P" + PlayerNumber + "GrappleSwitch";
        playerVerticalAxis = "P" + PlayerNumber + "Vertical";
        playerHorizontalAxis = "P" + PlayerNumber + "Horizontal";
    }

    internal void GetCameraPosition(float camX, float camY, float camW, float camH)
    {
        cameraX = camX;
        cameraY = camY;
        cameraW = camW;
        cameraH = camH;
    }

    internal void ResetReticlePosition()
    {
        float reticleX = Screen.width * (cameraW / 2);
        float reticleY = Screen.height * (cameraH / 2); 

        switch (PlayerNumber)
        {
            case 1:
                if (numberOfPlayers > 2) reticleY += (Screen.height / 2);
                break;
            case 2:
                reticleX += (Screen.width / 2);
                if (numberOfPlayers > 2) reticleY += (Screen.height / 2);
                break;
            case 3:

                break;
            case 4:
                reticleX += (Screen.width / 2);
                break;
        }

        Reticle.transform.position = new Vector2(reticleX, reticleY);

        PlacementText.transform.position = new Vector2(reticleX, reticleY);
    }

    internal void StartSetSpeedometerAndIndicatorPositions()
    {
        float spedX = (Screen.width * cameraX) + 20;
        float spedY = (Screen.height * cameraY) + 10;

        Speedometer.transform.position = new Vector2(spedX, spedY);

        //SpeedometerText.transform.position = Speedometer.transform.position;

        if (numberOfPlayers > 2)
        {
            Speedometer.transform.localScale *= 0.6f;
            SpeedometerText.transform.localScale *= 0.6f;
        }

        ModeIndicator.transform.position = new Vector2(spedX + (Speedometer.GetComponent<RectTransform>().rect.width * 2), spedY);

        ModeIndicator.GetComponent<Image>().sprite = pullIcon;

        PCompass.GetComponent<Compass>().playerTransform = this.transform;
    }
    
    internal void StartSetCheckpointMeterPosition()
    {
        float cmX = Screen.width * cameraW;
        float cmY = Screen.height * cameraH / 2;

        switch (PlayerNumber)
        {
            case 1:
                if (numberOfPlayers > 2) cmY += (Screen.height * cameraY);
                break;
            case 2:
                cmX += (Screen.width * cameraX);
                if (numberOfPlayers > 2) cmY += (Screen.height * cameraY);
                break;
            case 3:
                break;
            case 4:
                cmX += (Screen.width * cameraX);
                break;
        }

        CheckpointMeter.transform.position = new Vector2(cmX, cmY);
        CheckpointMeterFill.transform.position = new Vector2(cmX, cmY);

        if (numberOfPlayers < 3)
        {
            CheckpointMeter.transform.localScale *= 0.5f;
            CheckpointMeterFill.transform.localScale *= 0.5f;
        }
        else
        {
            CheckpointMeter.transform.localScale *= 0.3f;
            CheckpointMeterFill.transform.localScale *= 0.3f;
        }
    }

    /*internal void StartSetCompassPosition()
    {
        PCompass.GetComponent<Compass>().playerTransform = this.transform;

        float compX = (Screen.width * cameraW) + (Screen.width * cameraX) - (PCompass.GetComponent<RectTransform>().rect.width);
        float compY = (Screen.height * cameraY) + (PCompass.GetComponent<RectTransform>().rect.height);

        PCompass.transform.position = new Vector2(compX, compY);
    }*/

    internal void StartSetObjectiveReference()
    {
        ObjectiveIndicator.GetComponent<TargetManager>().playerReference = this.gameObject.transform;
    }

    internal void StartSetMinimapReference()
    {
        PlayerMinimap.GetComponent<Minimap>().Player = this.gameObject.transform;
    }

}
