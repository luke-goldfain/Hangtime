using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float GrappleDistance = 30f; // limit on the distance of the grappling hook.
    public float SpeedLimit = 30f; // SOFT speed limit, multiplied by number of consecutive grapples in the air.
    public float GrappleForce = 30f; // the force with which the grappling hook pulls the player.
    public float RunSpeed = 7f; // the maximum speed at which the player can move while on the ground (barring sliding from a jump).
    public float JumpForce = 10f; // The force with which the player jumps. Self-explanatory.

    [SerializeField]
    private GameObject ropeSectionPrefab;

    [SerializeField]
    public GameObject Reticle;

    [SerializeField]
    public GameObject Speedometer;

    [SerializeField]
    public int PlayerNumber;

    private int numberOfPlayers;
    
    private string playerFireButton;
    private string playerJumpButton;
    private string playerSlideButton;
    private string playerVerticalAxis;
    private string playerHorizontalAxis;

    private List<GameObject> ropeSections;

    private Rigidbody rb;

    public Vector3 GrappleHitPosition;

    private Vector3 grappleStartPosition;

    private Vector3 grappleDir;

    private Transform cameraTransform;

    private Vector3 airVelocity;
    private Vector3 incidenceVelocity;
    private Vector3 collisionNormal;

    private float groundTimer; // The active timer determining how long the player has been on the ground.
    private readonly float angleJumpCooldown = 0.25f; // The amount of time in seconds the player has to jump angularly out of a collision.

    private float slideTimer;
    private readonly float slideMaxTime = 1f; // The amount of time in seconds the player may slide for.

    private readonly float defaultFriction = 1f;
    private readonly float slideFriction = 0.05f;

    private bool hasAirDashed;

    // consecutiveGrapples keeps track of how many times the player has
    // grappled since they touched a surface. Max Speed increases multiplicatively  
    // with each one up to a maximum of 4.
    private int consecutiveGrapples;

    // bool necessary because there is no method for no collision.
    private bool colliding;

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

        StartSetReticlePosition();
        StartSetSpeedometerPosition();

        rb = this.GetComponent<Rigidbody>();

        ropeSections = new List<GameObject>();

        Cursor.lockState = CursorLockMode.Locked;
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

        Speedometer.GetComponent<Text>().text = Mathf.RoundToInt(this.rb.velocity.magnitude).ToString();

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

        if (Input.GetButtonDown(playerFireButton))
        {
            CastGrapple();
        }

        if (Input.GetButtonUp(playerFireButton))
        {
            currentState = State.inAir;
        }

        if (prevState != currentState)
        {
            UpdateOneTimeStateActions();

            prevState = currentState;
        }

        //Debug.Log(currentState);

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
            ClampSpeedToLimit();
        }

        // debug reset position
        if (Vector3.Distance(Vector3.zero, this.transform.position) > 100f && this.transform.position.y < -20)
        {
            this.transform.position = Vector3.zero;
            rb.velocity = Vector3.zero;
        }

        // Debug escape to lock/unlock cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Conditional operator: if cursor unlocked, lock cursor, otherwise unlock cursor
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked)? CursorLockMode.None : CursorLockMode.Locked;
        }
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

                    break;
                }
        }
    }

    private void UpdateMoveGrappling()
    {
        // Movement direction is  a spherical interpolation of the forward-facing direction and the direction of the grapple.
        // The last argument in this function determines the relative "strength" of the two Vector3's, with the grapple direction
        // remaining the strongest factor in movement direction.
        Vector3 moveDir = Vector3.Slerp(cameraTransform.forward, grappleDir, 0.9f);

        // If player is holding a direction (thumbstick, WASD) while they grapple, it slightly affects the direction of their grapple.
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

        // Add force inverse to the length of the grapple (low length = high force). 
        // If distance is high enough that the force is below 15 * consecutiveGrapples, default to that.
        rb.AddForce(moveDir * Mathf.Max(GrappleForce - Vector3.Distance(GrappleHitPosition, this.transform.position), 15f * (consecutiveGrapples + 1)));
        
        // If the player has traveled 1.5 times the original length of the grapple (a ways past or away from the grapple point), break off the grapple automatically.
        if (Vector3.Distance(this.transform.position, grappleStartPosition) >= Vector3.Distance(GrappleHitPosition, grappleStartPosition) * 1.5)
        {
            this.currentState = State.inAir;

            ClampSpeedToLimit();
        }
    }

    private void UpdateMoveAir()
    {
        float hAxis = Input.GetAxisRaw(playerHorizontalAxis);
        float vAxis = Input.GetAxisRaw(playerVerticalAxis);

        rb.AddRelativeForce(new Vector3(hAxis, 0, vAxis) * 2f);

        // Assign airVelocity variable, used primarily for checking collision angles
        airVelocity = rb.velocity;

        // Reassign air velocity if the player jumps, allowing for air jumping (once per inAir-state)
        // The velocity assignment for air-jumping takes current velocity into account, albeit not weighing it heavily.
        if (Input.GetButtonDown(playerJumpButton) && !hasAirDashed)
        {
            rb.velocity = Vector3.Slerp(rb.velocity, (transform.forward * vAxis * JumpForce) + (transform.right * hAxis * JumpForce) + (transform.up * JumpForce), 0.9f);

            hasAirDashed = true;
        }
    }

    private void UpdateMoveGround()
    {
        groundTimer += Time.deltaTime;

        float hAxis = Input.GetAxisRaw(playerHorizontalAxis);
        float vAxis = Input.GetAxisRaw(playerVerticalAxis);
        
        if (Input.GetButtonDown(playerSlideButton))
        {
            slideTimer = 0f;
        }

        if (Input.GetButton(playerSlideButton) && slideTimer < slideMaxTime) // Sliding behavior
        {
            this.GetComponent<Collider>().material.dynamicFriction = slideFriction;

            rb.AddForce(((transform.forward * vAxis) + (transform.right * hAxis)) * 3f);
            rb.AddForce(-transform.up * 3f);

            // If the player is not on a flat surface and they are moving faster than the default run speed,
            // presume they are sliding downhill and refresh the slide timer.
            if (rb.velocity.magnitude > RunSpeed && collisionNormal.y != 1)
            {
                slideTimer = 0f;
            }

            slideTimer += Time.deltaTime;
        }
        else if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 2f, ~(1 << 8))) // Normal running behavior
        {
            rb.velocity = Vector3.ProjectOnPlane((transform.forward * vAxis * RunSpeed) + (transform.right * hAxis * RunSpeed), hit.normal);

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, RunSpeed);
        }

        if (Input.GetButtonUp(playerSlideButton) || slideTimer >= slideMaxTime) // What happens when a slide "ends"
        {
            this.GetComponent<Collider>().material.dynamicFriction = defaultFriction;

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, RunSpeed);
        }


        if (Input.GetButtonDown(playerJumpButton))
        {
            this.transform.position += Vector3.up; // Messy brute-force to get player out of the ground --
                                                   // jumping does not work correctly if player is colliding with the ground when the next line is executed.

            // To jump off of a surface based on angle and magnitude of incidence, the angle must be straighter than 75 degrees (which is almost flat to the
            //  surface), they must have been against the surface for < angleJumpCooldown seconds, and the velocity of the incidence must be high enough.
            if (Vector3.Angle(incidenceVelocity, collisionNormal) < 75f && groundTimer < angleJumpCooldown && incidenceVelocity.magnitude > 10f)
            {
                rb.velocity = Vector3.ClampMagnitude(incidenceVelocity, SpeedLimit);
            }
            // If the previous parameters are not met, jump velocity is given by current velocity alongside the default jump force.
            else
            {
                

                rb.velocity += (transform.forward * vAxis * JumpForce) + (transform.right * hAxis * JumpForce) + (transform.up * JumpForce);
            }

            this.currentState = State.inAir;
        }
    }

    private void ClampSpeedToLimit()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, this.SpeedLimit + (this.SpeedLimit * 0.5f * (consecutiveGrapples - 1)));
    }

    private void CastGrapple()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, cameraTransform.TransformDirection(Vector3.forward) * GrappleDistance, Color.blue, 1f);

        if (Physics.Raycast(this.transform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, GrappleDistance, ~(1 << 8)))
        {
            currentState = State.grappling;

            GrappleHitPosition = hit.point;

            grappleStartPosition = this.transform.position;

            grappleDir = Vector3.Normalize(hit.point - this.transform.position);

            for(int i = 1; i <= 3; i++)
            {
                ropeSections.Add(Instantiate(ropeSectionPrefab, this.transform.position + ((hit.point - this.transform.position) / i), Quaternion.identity, this.transform));

                ropeSections[i-1].GetComponent<RopeSectionPositioner>().SectionNumber = i;
            }
        }
    }

    private void UpdateCheckGrapplability()
    {
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, GrappleDistance, ~(1 << 8)))
        {
            Reticle.GetComponent<Image>().color = new Color(0.2f, 0.6f, 1f);
        }
        else
        {
            Reticle.GetComponent<Image>().color = new Color(0.9f, 0.3f, 0.3f);
        }
    }

    private void StartAssignInputButtons()
    {
        playerFireButton = "P" + PlayerNumber + "Fire1";
        playerJumpButton = "P" + PlayerNumber + "Jump";
        playerSlideButton = "P" + PlayerNumber + "Slide";
        playerVerticalAxis = "P" + PlayerNumber + "Vertical";
        playerHorizontalAxis = "P" + PlayerNumber + "Horizontal";
    }

    private void StartSetReticlePosition()
    {
        float reticleX = Screen.width / 2;
        float reticleY = Screen.height / 2; 

        switch (PlayerNumber)
        {
            case 1:
                if (numberOfPlayers == 1) reticleX = Screen.width / 2;
                else reticleX = Screen.width / 4;
                if (numberOfPlayers < 3) reticleY = Screen.height / 2;
                else reticleY = Screen.height * (3f/4f);
                break;
            case 2:
                reticleX = Screen.width * (3f/4f);
                if (numberOfPlayers < 3) reticleY = Screen.height / 2;
                else reticleY = Screen.height * (3f / 4f);
                break;
            case 3:
                reticleX = Screen.width / 4;
                reticleY = Screen.height / 4;
                break;
            case 4:
                reticleX = Screen.width * (3f/4f);
                reticleY = Screen.height / 4;
                break;
        }

        Reticle.transform.position = new Vector2(reticleX, reticleY);
    }

    private void StartSetSpeedometerPosition()
    {
        float spedX = Screen.width / 8;
        float spedY = Screen.height / 8;

        switch (PlayerNumber)
        {
            case 1:
                spedX = Screen.width / 8;
                if (numberOfPlayers < 3) spedY = Screen.height / 8;
                else spedY = Screen.height * (5f/8f);
                break;
            case 2:
                spedX = Screen.width * (5f / 8f);
                if (numberOfPlayers < 3) spedY = Screen.height / 8;
                else spedY = Screen.height * (5f / 8f);
                break;
            case 3:
                spedX = Screen.width / 8;
                spedY = Screen.height / 8;
                break;
            case 4:
                spedX = Screen.width * (5f / 8f);
                spedY = Screen.height / 8;
                break;
        }

        Speedometer.transform.position = new Vector2(spedX, spedY);
    }
}
