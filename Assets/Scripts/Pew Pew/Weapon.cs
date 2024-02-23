using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    public AudioClip shootSound;
    public float shootSpeed;
    public float shootingRange;
    public float bulletsInTime;
    public bool isAutomatic;
    public Vector2 spread;
    private bool shootingInput;
    private bool canShoot = true;
    public GameObject camRecoil;
    public Camera cam;
    public Transform attackPoint;
    public GameObject bullet;

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
        if (Player.Instance.GetComponent<PauseMenuManager>().isPaused) return;
        if (Player.Instance.GetComponent<PlayerInventory>().inStash) return;
        if (!canShoot) return;
        if (!shootingInput) return;
        Shoot();
    }

    private void Shoot()
    {
        canShoot = false;
        GetComponent<GunRecoil>().Fire();
        camRecoil.GetComponent<CameraRecoil>().Fire();
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, shootingRange, LayerMask.NameToLayer("Player")))
            targetPoint = hit.point;
        else targetPoint = ray.GetPoint(75f);
        Vector3 bulletDirection = targetPoint - attackPoint.position;

        float x = Random.Range(-spread.x, spread.x);
        float y = Random.Range(-spread.y, spread.y);

        bulletDirection = bulletDirection + new Vector3(x, y, 0);
        var curBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        curBullet.transform.forward = bulletDirection.normalized;
        curBullet.GetComponent<Rigidbody>().AddForce(bulletDirection.normalized * 30f, ForceMode.Impulse);
        Destroy(curBullet, 5f);
        
        GetComponent<AudioSource>().PlayOneShot(shootSound);
        Invoke(nameof(ResetShot), shootSpeed);
    }

    private void ResetShot()
    {
        canShoot = true;
    }

}
