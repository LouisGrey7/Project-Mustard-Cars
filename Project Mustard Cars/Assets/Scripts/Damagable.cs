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

    
    // Start is called before the first frame update
    private void Start()
    {
        Health = MaxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        playerLevel = player.GetComponent<LevelSystem>();
    }

    internal void Hit(int damagePoints)
    {
        Health -= damagePoints;
        if (health <= 0)
        {
            
            OnDead?.Invoke();
            Destroy(gameObject);
            playerLevel.GainExperienceFlatRate(20);
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

}
