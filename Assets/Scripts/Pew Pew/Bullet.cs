using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float damage;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            other.gameObject.GetComponent<Enemy>().Health -= damage;
            Destroy(gameObject);
        }
    }
}
