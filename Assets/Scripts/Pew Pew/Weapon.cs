using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public AudioClip shootSound;
    public float shootSpeed;
    public float shootingRange;
    public float bulletsInTime;
    public bool isAutomatic;
    public float timeBetweenShooting;
    public Vector2 spread;
    private bool shootingInput;
    private bool canShoot = true;
    public GameObject cam;

    private void Update()
    {
        Inputing();
        ChecksForShooting();
    }
    
    private void Inputing()
    {
        if (!canShoot) return;
        if (isAutomatic) shootingInput = Input.GetMouseButton(0);
        else shootingInput = Input.GetMouseButtonDown(0);
    }

    private void ChecksForShooting()
    {
        if (!canShoot) return;
        if (!shootingInput) return;
        Shoot();
    }

    private void Shoot()
    {
        canShoot = false;
        GetComponent<GunRecoil>().Fire();
        cam.GetComponent<CameraRecoil>().Fire();
        Debug.Log("pew pew");
        GetComponent<AudioSource>().PlayOneShot(shootSound);
        Invoke(nameof(ResetShot), timeBetweenShooting);
    }

    private void ResetShot()
    {
        canShoot = true;
    }

}
