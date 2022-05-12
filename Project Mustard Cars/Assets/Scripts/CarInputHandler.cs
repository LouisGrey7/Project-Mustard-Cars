using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{

    TopDownCarController topDownCarController;


    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        topDownCarController = GetComponent<TopDownCarController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        //Get input from unity's input system
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        //Send the input to the car controller
        topDownCarController.SetInputVector(inputVector);

        if (Input.GetButtonDown("Jump"))
        {
            topDownCarController.Jump(1.0f, 0.0f);
        }

    }
}
