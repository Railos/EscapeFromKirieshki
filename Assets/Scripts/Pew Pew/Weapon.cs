using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float shootSpeed;
    public float shootingRange;
    public float bulletsInTime;
    public bool isAutomatic;
    public float timeBetweenShooting;
    public Vector2 spread;
    private bool shootingInput;

    private void Update()
    {
        Inputing();
        ChecksForShooting();
    }
    
    private void Inputing()
    {
        if (isAutomatic) shootingInput = Input.GetMouseButton(0);
        else shootingInput = Input.GetMouseButtonDown(0);
    }

    private void ChecksForShooting()
    {
        if (!shootingInput) return;
        Shoot();
    }

    private void Shoot()
    {
        Debug.Log("pew pew");
    }
    
}
