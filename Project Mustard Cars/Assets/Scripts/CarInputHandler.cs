using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarInputHandler : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;


    
    public UnityEvent OnShoot = new UnityEvent();
    public UnityEvent<Vector2> OnMoveTurret = new UnityEvent<Vector2>();

    TopDownCarController topDownCarController;


    //Awake is called when the script instance is being loaded.
    void Awake()
    {
        topDownCarController = GetComponent<TopDownCarController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetTurretMovement();
        GetShootingInput();
        
        Vector2 inputVector = Vector2.zero;

        //Get input from unity's input system
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        //Send the input to the car controller
        topDownCarController.SetInputVector(inputVector);


    }

    private void GetTurretMovement()
    {
        OnMoveTurret?.Invoke(GetMousePosition());
    }

    private void GetShootingInput()
    {
        if(Input.GetMouseButton(0))
        {
            OnShoot?.Invoke();
        }
    }

    private Vector2 GetMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.nearClipPlane;
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        return mouseWorldPosition;

    }
}
