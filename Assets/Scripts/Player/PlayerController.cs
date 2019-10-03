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
    public float SpeedLimit = 30f; 
    [Tooltip("The force with which the grappling hook pulls the player.")]
    public float GrappleForce = 30f; 
    [Tooltip("The default speed at which the player can move while on the ground. Sliding and landing while moving quickly temporarily circumvent this.")]
    public float DefaultRunSpeed = 7f;
    [Tooltip("The force with which the player jumps.")]
    public float JumpForce = 10f; 
    [Tooltip("The maximum force with which the player can jump, applicable only to \"rebound\" jumps.")]
    public float MaxJumpForce = 30f; 
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
    //public GameObject Reticle, Speedometer, CheckpointMeter, CheckpointMeterFill;

    public GameObject Reticle, Speedometer, SpeedometerText, 
                      SpeedometerNeedle, PCompass, CheckpointMeter, 
                      CheckpointMeterFill, PlacementText, CheckpointText,
                      ModeIndicator, ObjectiveIndicator;

    [SerializeField]
    private Sprite pullIcon, swingIcon;

    [SerializeField]
    public int PlayerNumber;

    [SerializeField]
    public LayerMask GrapplableMask;

    public bool AcceptsInput = true;

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

    private Vector3 grappleDir;

    private float currentGrappleDistance;

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

    private bool hasAirDashed;

    private bool grappleCasted;

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
    }

    private void StartAssignHUDObjects()
    {
        Reticle = HUD.transform.Find("Reticle").gameObject;
        Speedometer = HUD.transform.Find("Speedometer").gameObject;
        SpeedometerText = HUD.transform.Find("Speedometer (UI)").gameObject;
        SpeedometerNeedle = Speedometer.transform.Find("Needle").gameObject;
        PCompass = HUD.transform.Find("Compass").gameObject;
        CheckpointMeter = HUD.transform.Find("CheckpointMeter").gameObject;
        CheckpointMeterFill = HUD.transform.Find("CheckpointMeterFill").gameObject;
        PlacementText = HUD.transform.Find("PlacementText").gameObject;
        CheckpointText = CheckpointMeter.transform.Find("CheckpointText").gameObject;
        ModeIndicator = HUD.transform.Find("ModeIndicator").gameObject;
        ObjectiveIndicator = HUD.transform.Find("ObjectiveParent").gameObject;
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
    void Update()
    {
        cameraTransform = this.GetComponentInChildren<PlayerCameraController>().transform;

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

                    break;
                }
            case State.onGround:
                {
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

                    if (consecutiveGrapples < 4) consecutiveGrapples++;
                    
                    currentGrappleDistance = Vector3.Distance(this.transform.position, GrappleHitPosition);

                    // Change the current grapple's normal based on whether the player is high enough above their grapple,
                    // as well as more vertical than horizontal to their grapple.
                    if ((this.transform.position.y - GrappleHitPosition.y >= 8f) &&
                        (this.transform.position.y - GrappleHitPosition.y >= (new Vector2(this.transform.position.x, this.transform.position.z) -
                                                                              new Vector2(GrappleHitPosition.x, GrappleHitPosition.z)).magnitude))
                    {
                        currentGrappleNormal = cameraTransform.right;
                    }
                    else
                    {
                        currentGrappleNormal = -cameraTransform.right;
                    }

                    break;
                }
        }
    }

    private void UpdateMoveGrappling()
    {
        Vector3 moveDir = Vector3.up; // Default -- always overwritten

        if (isPulling)
        {
            // Movement direction is  a spherical interpolation of the forward-facing direction and the direction of the grapple.
            // The last argument in this function determines the relative "strength" of the two Vector3's, with the grapple direction
            // remaining the strongest factor in movement direction.
            moveDir = Vector3.Slerp(cameraTransform.forward, grappleDir, 0.9f);
        }
        else
        {
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
        }
        else if (this.transform.position.y <= GrappleHitPosition.y) // if player is not pulling, rotate them around the grapple point at a soft-fixed speed, ala spider man.
        {
            rb.AddForce(moveDir * GrappleForce * 0.1f);

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, GrappleForce * 2f);
        }

        // If the player is pulling and has traveled 1.5 times the original length of the grapple (a ways past or away from the grapple point), break off the grapple automatically.
        // If the player is swinging and has traveled 2.4 times original grapple length, same deal.
        if ((Vector3.Distance(this.transform.position, grappleStartPosition) >= Vector3.Distance(GrappleHitPosition, grappleStartPosition) * 1.5f && isPulling) ||
            (Vector3.Distance(this.transform.position, grappleStartPosition) >= Vector3.Distance(GrappleHitPosition, grappleStartPosition) * 2.4f && !isPulling))
        {
            this.currentState = State.inAir;

            LerpSpeedToLimit();
        }

        // Update currentRunSpeed so that when the player hits the ground, they are running relative to how fast they were grappling
        currentRunSpeed = rb.velocity.magnitude * 0.7f;
    }

    private void UpdateMoveAir()
    {
        if (AcceptsInput)
        {
            float hAxis = Input.GetAxisRaw(playerHorizontalAxis);
            float vAxis = Input.GetAxisRaw(playerVerticalAxis);

            rb.AddRelativeForce(new Vector3(hAxis, 0, vAxis) * 2f);

            // Assign airVelocity variable, used primarily for checking collision angles
            airVelocity = rb.velocity;

            // Allow the player to fall faster with the slide button.
            // Think of this as "fast-falling", like in Super Smash Bros.
            if (Input.GetButton(playerSlideButton) && rb.velocity.y > -20f)
            {
                rb.AddForce(Vector3.down * 10f);
            }

            // Reassign air velocity if the player jumps, allowing for air jumping (once per inAir-state)
            // The velocity assignment for air-jumping takes current velocity into account, albeit not weighing it heavily.
            if (Input.GetButtonDown(playerJumpButton) && !hasAirDashed)
            {
                rb.velocity = Vector3.Slerp(rb.velocity, (transform.forward * vAxis * JumpForce) + (transform.right * hAxis * JumpForce) + (transform.up * JumpForce), 0.9f);

                hasAirDashed = true;
            }
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
            else if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 2f, ~(1 << 8))) // Normal running behavior
            {
                // Decrease currentRunSpeed gradually until it matches DefaultRunSpeed.
                currentRunSpeed = Mathf.Lerp(currentRunSpeed, DefaultRunSpeed, 0.02f); // Lerp towards DefaultRunSpeed.
                currentRunSpeed = Mathf.Max(currentRunSpeed, DefaultRunSpeed); // Mathf.Max to ensure [currentRunSpeed >= DefaultRunSpeed]

                rb.velocity = Vector3.ProjectOnPlane((transform.forward * vAxis * currentRunSpeed) + (transform.right * hAxis * currentRunSpeed), hit.normal);

                rb.velocity = Vector3.ClampMagnitude(rb.velocity, currentRunSpeed);
            }

            if (Input.GetButtonUp(playerSlideButton) || slideTimer >= slideMaxTime) // What happens when a slide "ends"
            {
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
                this.transform.position += Vector3.up; // Messy brute-force to get player out of the ground --
                                                       // jumping does not work correctly if player is colliding with the ground when the next line is executed.

                // To jump off of a surface based on angle and magnitude of incidence, the angle must be straighter than 75 degrees (which is almost flat to the
                //  surface), they must have been against the surface for < angleJumpCooldown seconds, and the velocity of the incidence must be high enough.
                if (Vector3.Angle(incidenceVelocity, collisionNormal) < 75f && groundTimer < angleJumpCooldown && incidenceVelocity.magnitude > 10f)
                {
                    rb.velocity = Vector3.ClampMagnitude(incidenceVelocity, MaxJumpForce);
                }
                // If the previous parameters are not met, jump velocity is given by current velocity alongside the default jump force.
                // Note: When jumping off of a wall, this may feel unnatural as the player will jump straight up instead of jumping "off" the wall.
                else
                {
                    float tempJumpForce = Mathf.Max(JumpForce, Mathf.Min(rb.velocity.magnitude, MaxJumpForce));

                    rb.velocity = (transform.forward * vAxis * tempJumpForce) + (transform.right * hAxis * tempJumpForce) + (transform.up * tempJumpForce * 0.75f);
                }

                this.currentState = State.inAir;
            }
        }
    }

    private void LerpSpeedToLimit()
    {
        rb.velocity = Vector3.Slerp(rb.velocity, Vector3.ClampMagnitude(rb.velocity, this.SpeedLimit + (this.SpeedLimit * 0.5f * (consecutiveGrapples - 1))), 0.4f);
    }

    private void CastGrapple()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, cameraTransform.TransformDirection(Vector3.forward) * GrappleDistance, Color.blue, 1f);

        if (Physics.Raycast(this.transform.position, lastGrapplableRaycastPoint - cameraTransform.position, out hit, GrappleDistance, GrapplableMask) &&
            grapplableTimer < grapplableMaxTime)
        {
            GrappleHitPosition = hit.point;

            grappleCasted = true;

            currentGrappleTravelTime = 0f;

            grapplingHook = Instantiate(grapplingHookPrefab, this.transform.position, this.cameraTransform.rotation);
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
        float spedX = (Screen.width * cameraX);
        float spedY = (Screen.height * cameraY);

        Speedometer.transform.position = new Vector2(spedX, spedY);

        SpeedometerText.transform.position = Speedometer.transform.position;

        if (numberOfPlayers > 2)
        {
            Speedometer.transform.localScale *= 0.6f;
            SpeedometerText.transform.localScale *= 0.6f;
        }

        ModeIndicator.transform.position = new Vector2(spedX + (Speedometer.GetComponent<RectTransform>().rect.width * 2), spedY);

        ModeIndicator.GetComponent<Image>().sprite = pullIcon;
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

    internal void StartSetCompassPosition()
    {
        PCompass.GetComponent<Compass>().playerTransform = this.transform;

        float compX = (Screen.width * cameraW) + (Screen.width * cameraX) - (PCompass.GetComponent<RectTransform>().rect.width);
        float compY = (Screen.height * cameraY) + (PCompass.GetComponent<RectTransform>().rect.height);

        PCompass.transform.position = new Vector2(compX, compY);
    }

    internal void StartSetObjectiveReference()
    {
        ObjectiveIndicator.GetComponent<TargetManager>().playerReference = this.gameObject.transform;
    }
}
