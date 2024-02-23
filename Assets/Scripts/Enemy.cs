using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float Health
    {
        get => health;
        set
        {
            health = value;
            GetDamage();
        } 
    }

    private void GetDamage()
    {
        if (health <= 0f) Destroy(gameObject);
    }
}
