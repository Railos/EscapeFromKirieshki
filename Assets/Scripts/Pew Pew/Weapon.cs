using System;
using System.Runtime.InteropServices.ComTypes;
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
    
    public Transform normalPos;
    public Transform clippingPos;

    public Vector3 normalRecoil;
    public Vector3 adsRecoil;

    public float normalRecoilPos;
    public float adsRecoilPos;
    public Vector3 normalPosHolder;

    private bool clipping;

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
        WeaponClippingFix();
    }

    private void WeaponClippingFix()
    {
        RaycastHit _out;
        if (Physics.SphereCast(clippingPos.position, 0.2f, attackPoint.forward, out _out, 1.1f,
            ~LayerMask.GetMask("Player")))
        {
            transform.parent.position = Vector3.Lerp(transform.parent.position, transform.parent.position + transform.right * 0.2f, 20f * Time.deltaTime);
            clipping = true;
        }
            
        else if (!Physics.SphereCast(clippingPos.position, 0.2f, attackPoint.forward, out _out, 1.2f, ~LayerMask.GetMask("Player")))
        {
            transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, normalPosHolder, 3f * Time.deltaTime);
            clipping = false;
        }
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
            transform.parent.parent.position = Vector3.Lerp(transform.parent.parent.position, Camera.main.ScreenToWorldPoint( new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane)) + transform.parent.parent.parent.parent.forward * 0.5f, 3f * Time.deltaTime);
            GetComponent<GunRecoil>().RecoilRotation = adsRecoil;
            GetComponent<GunRecoil>().RecoilPositionX = adsRecoilPos;
            return;
        }
        transform.parent.parent.position = Vector3.Lerp(transform.parent.parent.position, normalPos.position, 3f * Time.deltaTime);
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
        
        float x = Random.Range(-spread.x, spread.x);
        float y = Random.Range(-spread.y, spread.y);

        Vector3 bulletDirection = attackPoint.forward + new Vector3(x, y, 0);
        var curBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        curBullet.transform.forward = bulletDirection.normalized;
        curBullet.GetComponent<Rigidbody>().AddForce(bulletDirection.normalized * 220f, ForceMode.Impulse);
        Destroy(curBullet, 5f);
        
        GetComponent<GunRecoil>().Fire();
        camRecoil.GetComponent<CameraRecoil>().Fire();
        
        GetComponent<AudioSource>().PlayOneShot(shootSound);
        Invoke(nameof(ResetShot), shootSpeed);
    }

    private void ResetShot()
    {
        canShoot = true;
    }
}
