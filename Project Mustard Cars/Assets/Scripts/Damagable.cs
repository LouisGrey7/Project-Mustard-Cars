using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damagable : MonoBehaviour
{

    private GameObject player;
    LevelSystem playerLevel;
    public int MaxHealth = 100;
    [SerializeField]
    private int health;
    public bool deadObject = false;
    public AudioSource bulletHitSFX;
    public GameObject deathEffect;

    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            OnHealthChange?.Invoke((float)Health / MaxHealth);
        }

    }

    public UnityEvent OnDead;
    public UnityEvent<float> OnHealthChange;
    public UnityEvent OnHit, OnHeal;


    private void Awake()
    {
        bulletHitSFX = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        Health = MaxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        playerLevel = player.GetComponent<LevelSystem>();
        

    }

    internal void Hit(int damagePoints)
    {
        bulletHitSFX.Play();
        Health -= damagePoints;
        if (health <= 0)
        {

            DeathEffect();
            OnDead?.Invoke();

            playerLevel.GainExperienceScalable(2, playerLevel.level);
            deadObject = true;
        }
        else
        {

            OnHit?.Invoke();
        }
    }

    public void Heal(int healthBoost)
    {
        Health += healthBoost;
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        OnHeal?.Invoke();
    }

    private void DeathEffect()
    {
        if(deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }

}
