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
    private bool inADS;
    
    public Transform ADSPos;
    public Transform normalPos;

    public Vector3 normalRecoil;
    public Vector3 adsRecoil;

    public float normalRecoilPos;
    public float adsRecoilPos;

    private void Start()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, shootingRange, LayerMask.NameToLayer("Player")))
            targetPoint = hit.point;
        else targetPoint = ray.GetPoint(75f);
        Vector3 bulletDirection = targetPoint - transform.position;
        transform.forward = bulletDirection.normalized;
        
    }

    private void Update()
    {
        Inputing();
        ChecksForShooting();
        ADS();
    }
    
    private void Inputing()
    {
        if (isAutomatic) shootingInput = Player.Instance.controls.Land.ShootWeapon.inProgress;
        else shootingInput = Player.Instance.controls.Land.ShootWeapon.triggered;

        if (Player.Instance.controls.Land.ADS.inProgress) inADS = true;
        else inADS = false;
    }

    private void ADS()
    {
        if (inADS)
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, ADSPos.position, .05f);
            GetComponent<GunRecoil>().RecoilRotation = adsRecoil;
            GetComponent<GunRecoil>().RecoilPositionX = adsRecoilPos;
            return;
        }
        transform.parent.position = Vector3.Lerp(transform.parent.position, normalPos.position, .05f);
        GetComponent<GunRecoil>().RecoilRotation = normalRecoil;
        GetComponent<GunRecoil>().RecoilPositionX = normalRecoilPos;
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
        
        float x = Random.Range(-spread.x, spread.x);
        float y = Random.Range(-spread.y, spread.y);

        Vector3 bulletDirection = attackPoint.forward + new Vector3(x, y, 0);
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
