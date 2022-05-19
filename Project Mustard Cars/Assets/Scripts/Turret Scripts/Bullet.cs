using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10;
    public int damage = 5;
    public float maxDistance = 10;

    private Vector2 startPosition;
    private float conqueredDistance = 0;
    private Rigidbody2D rb2d;

    CarSfxHandler carSfxHandler;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Initialize()
    {
        startPosition = transform.position;
        rb2d.velocity = transform.up * speed;
    }

    private void Update()
    {
        conqueredDistance = Vector2.Distance(transform.position, startPosition);
        if(conqueredDistance >= maxDistance)
        {
            DisableObject();
        }
    }

    private void DisableObject()
    {
        rb2d.velocity = Vector2.zero;
        gameObject.SetActive(false);
        Destroy(gameObject);


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collider " + collision.name);

        var damagable = collision.GetComponent<Damagable>();
        if(damagable != null)
        {
            damagable.Hit(damage);
            
        }

        DisableObject();
        
    }

}
