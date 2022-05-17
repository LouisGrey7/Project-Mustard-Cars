using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    //public Rigidbody2D rb2d;

    private Vector2 movementVector;
    public AimTurret aimTurret;
    public Turret[] turrets;

    //public Transform turretParent;

    private void Awake()
    {
        //rb2d = GetComponent<Rigidbody2D>();

        if (aimTurret == null)
            aimTurret = GetComponentInChildren<AimTurret>();

        if (turrets == null || turrets.Length == 0)
        {
            turrets = GetComponentsInChildren<Turret>();
        }

    }

    public void HandleShoot()
    {
        foreach (var turret in turrets)
        {
            turret.Shoot();
        }
    }

    public void HandleMoveBody(Vector2 movementVector)
    {
        this.movementVector = movementVector;
    }

    public void HandleTurretMovement(Vector2 pointerPosition)
    {
        aimTurret.Aim(pointerPosition);
    }



}
