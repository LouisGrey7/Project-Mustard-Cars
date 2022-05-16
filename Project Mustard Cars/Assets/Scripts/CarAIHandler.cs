using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAIHandler : MonoBehaviour
{
    public enum AIMode { followPlayer, followpoints};

    [Header("AI Settings")]
    AIMode aiMode;

    //local variables
    Vector3 targetPosition = Vector3.zero;
    Transform targetTransform = null;


    //Components
    TopDownCarController topDownCarController;

    void Awake()
    {
        topDownCarController = GetComponent<TopDownCarController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 inputVector = Vector2.zero;

        switch(aiMode)
        {
            case AIMode.followPlayer:
                FollowPlayer();

                break;
        }

        inputVector.x = TurnTowardTarget();
        inputVector.y = 1.0f;

        //Send the input to the car controller.
        topDownCarController.SetInputVector(inputVector);
    }

    void FollowPlayer()
    {
        if (targetTransform == null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
        }

    }
        float TurnTowardTarget()
        {
            Vector2 vectorToTarget = targetPosition - transform.position;
            vectorToTarget.Normalize();

            //Calculate an angle towards the target
            float angleToTarget = Vector2.SignedAngle(transform.up, vectorToTarget);
            angleToTarget *= -1;

            //We want the car to turn as much as possible if the angle is greater than 45 degress and if the angle is smaller it will make smootherr smaller turn
            float steerAmount = angleToTarget / 45.0f;

            //Clamp steering to between -1 and 1
            steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);

            return steerAmount;

        }
}
