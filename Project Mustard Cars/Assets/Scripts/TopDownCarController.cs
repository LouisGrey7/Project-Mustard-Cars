using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 20;
    public Vector2 forwardVelocity;

    [Header("Sprites")]
    public SpriteRenderer carSpriteRenderer;
    public SpriteRenderer carShadowRenderer;


    [Header("Jumping")]
    public AnimationCurve jumpCurve;
    public ParticleSystem landingParticleSystem;


    //Local variables
    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    bool isJumping = false;

    //Components
    public Rigidbody2D carRigidBody2D;
    Collider2D carCollider;
    CarSfxHandler carSfxHandler;

    //Awake is called when the script instance is being loaded
    void Awake()
    {
        carRigidBody2D = GetComponent<Rigidbody2D>();
        carCollider = GetComponentInChildren<Collider2D>();
        carSfxHandler = GetComponent<CarSfxHandler>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        ApplyEngineForce();

        KillOrthonagalVelocity();

        ApplySteering();

    }

    void ApplyEngineForce()
    {

        //Don't let the player brake while in the air, but we still allow some drag so it can be slowed slightly. 
        if (isJumping && accelerationInput < 0)
            accelerationInput = 0;

        //calculate how much "forward we are going in terms of the diorection of our velocity
        velocityVsUp = Vector2.Dot(transform.up, carRigidBody2D.velocity);

        //Limit so we cannot go faster the max speed in the forward direction
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
        {
            return;
        }

        //Limit so we cannot go faster then 50% speed in the "reverse" direction
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
        {
            return;
        }

        //Limit so we cannot go faster in  any direction while accelerating
        if (carRigidBody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 && !isJumping)
        {
            return;
        }


        //create a force for the engine
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        //Apply force and pushes the car forward
        carRigidBody2D.AddForce(engineForceVector, ForceMode2D.Force);

    }

    void ApplySteering()
    {

        //Apply drag if there is no accelerationInput so the car stops when the player lets go of the accelerator
        if (accelerationInput == 0)
        {
            carRigidBody2D.drag = Mathf.Lerp(carRigidBody2D.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else
        {
            carRigidBody2D.drag = 0;

        }

        //Limit the cars ability to turn when moving slowly
        float minSpeedBeforeAllowTurningFactor = (carRigidBody2D.velocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);


        //Update the rotation angle based on input
        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        //Apply steeriong by rotating the car Oobject
        carRigidBody2D.MoveRotation(rotationAngle);

    }

    void KillOrthonagalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidBody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidBody2D.velocity, transform.right);


        carRigidBody2D.velocity = forwardVelocity + rightVelocity * driftFactor;

    }

    float GetLateralVelocity()
    {
        //Returns how fast the car is moving sideways.
        return Vector2.Dot(transform.right, carRigidBody2D.velocity);
    }


    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {

        lateralVelocity = GetLateralVelocity();
        isBraking = false;


        if (isJumping)
        {
            return false;
        }

        //Check if we are moving forward and if the player is hitting the brakes. In that case the tires should screech.
        if (accelerationInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        //If we have a lot of side movement then the tires should be screeching
        if (Mathf.Abs(GetLateralVelocity()) > 4.0f)
        {
            return true;
        }


        return false;

    }


    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public float GetVelocityMagnitude()
    {
        return carRigidBody2D.velocity.magnitude;
    }

    public void Jump(float jumpHeightScale, float jumpPushScale)
    {
        if (!isJumping)
            StartCoroutine(JumpCo(jumpHeightScale, jumpPushScale));
    }

    private IEnumerator JumpCo(float jumpHeightScale, float jumpPushScale)
    {
        isJumping = true;

        float jumpStartTime = Time.time;
        float jumpDuration = carRigidBody2D.velocity.magnitude * 0.05f;

        jumpHeightScale = jumpHeightScale * carRigidBody2D.velocity.magnitude * 0.05f;
        jumpHeightScale = Mathf.Clamp(jumpHeightScale, 0.0f, 1.0f);

        //Disable the car collider so we can perform an overlapped check 
        carCollider.enabled = false;

        carSfxHandler.PlayJumpSfx();

        //Change sorting layer to flying
        carSpriteRenderer.sortingLayerName = "Flying";
        carShadowRenderer.sortingLayerName = "Flying";


        //Push the object forward as we passed a jump
        carRigidBody2D.AddForce(carRigidBody2D.velocity.normalized * jumpPushScale * 10, ForceMode2D.Impulse);

        while (isJumping)
        {
            //Percentage 0 - 1.0 of where we are in the jumping process
            float jumpCompletedPercentage = (Time.time - jumpStartTime) / jumpDuration;
            jumpCompletedPercentage = Mathf.Clamp01(jumpCompletedPercentage);

            //Take the base scale of 1 and add how much we should increase the scale with. 
            carSpriteRenderer.transform.localScale = Vector3.one + Vector3.one * jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale;

            //Change the shadow scale also but make it a bit smaller. In the real world this should be the opposite, the higher an object gets the larger its shadow gets but this looks better in my opinion
            carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale * 0.75f;

            //Offset the shadow a bit. This is not 100% correct either but works good enough in our game. 
            carShadowRenderer.transform.localPosition = new Vector3(1, -1, 0.0f) * 3 * jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale;

            //When we reach 100% we are done
            if (jumpCompletedPercentage == 1.0f)
                break;



            yield return null;
        }

        //Check if landing is ok or not
        if (Physics2D.OverlapCircle(transform.position, 1.5f))
        {
            //Something is below the car so we need to jump again
            isJumping = false;

            //add a small jump and push the car forward a bit
            Jump(0.2f, 0.6f);
        }
        else
        {
            //Handle landing, scale back the object
            carSpriteRenderer.transform.localScale = Vector3.one;

            //reset the shadows position and scale
            carShadowRenderer.transform.localPosition = Vector3.zero;
            carShadowRenderer.transform.localScale = carSpriteRenderer.transform.localScale;

            //We are safe to land, so enable collision
            carCollider.enabled = true;

            //Change sorting layer to regular layer
            carSpriteRenderer.sortingLayerName = "Car";
            carShadowRenderer.sortingLayerName = "Shadow";

            //Play the landing particle system if it is a bigger jump
            if (jumpHeightScale > 0.2f)
            {
                landingParticleSystem.Play();

                carSfxHandler.PlayLandingSfx();
            }

            //Change state
            isJumping = false;
        }

        
    }

    //Detect Jump trigger
    void OnTriggerEnter2D(Collider2D collider2d)
    {
        if (collider2d.CompareTag("Jump"))
        {
            //Get the jump data from the jump
            JumpData jumpData = collider2d.GetComponent<JumpData>();
            Jump(jumpData.jumpHeightScale, jumpData.jumpPushScale);
        }
    }

    public void IncreaseMaxSpeed()
    {
        maxSpeed += 10;
    }
}
