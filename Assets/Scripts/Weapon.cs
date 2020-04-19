using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public float fireRate = 0f;
    public int Damage = 10;
    public LayerMask whatToHit;
    public Transform BulletTrailPrefab;
    public Transform HitPrefab;
    public Transform MuzzleFlashPrefab;

    public float camShakeAmt = 0.1f;
    public float camShakeLength = 0.2f;
    CameraShake camShake;

    public string weaponShootSound = "DefaultShoot";

    float timeToSpawnEffect = 0f;
    float effectSpawnTime = 10f;
    float timeToFire = 0f;
    Transform FirePoint;

    AudioManager audioManager;

    // Start is called before the first frame update
    void Awake()
    {
        //We searching for our transformation of game object if exist.

        FirePoint = transform.Find("FirePoint");
        if (FirePoint == null)
        {
            Debug.LogError("There is no point.");
        }
    }

    void Start()
    {
       camShake = GameMaster.gm.GetComponent<CameraShake>();
        if(camShake == null)
        {
            Debug.LogError("Mamy problem.");
        }
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("Mamy problem.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If fire rate equals 0 we have single shoot weapon.

        if (fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }

        else
        {
            //Method of shooting full auto with correct fire rate.

            if (Input.GetButton("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + (1 / fireRate);
                Shoot();
            }
        }
    }


    void Shoot()
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);  //mouse position in real time
        Vector2 firePointPosition = new Vector2(FirePoint.position.x, FirePoint.position.y);        //position of object from where are bullets shoot in real time.

        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100, whatToHit);     //creates a line that interacts with colliders, if hit it's true otherwise false.

        Debug.DrawLine(firePointPosition, (mousePosition - firePointPosition)*100, Color.cyan);     //Drawing the line of Raycast.

        if (hit.collider != null)           //We checking if hit some colliders
        {
            Debug.DrawLine(firePointPosition, hit.point, Color.red);    //We drawing Rycast line from fire point to the obstacle
            Enemy enemy = hit.collider.GetComponent<Enemy>();           //Apply that we hitting only collider of enemy


            if (enemy != null)       //Checking if we hit an enemy
            {
                enemy.DamageEnemy(Damage);      //We calling function to damage enemy by value Damage.               
            }
        }

        if (Time.time >= timeToSpawnEffect)         //Function to stop spwaning to many clones at once.
        {
            Vector3 hitPos;                         //new variable of transform of bullet trail.
            Vector3 hitNormal;

            if (hit.collider == null)
            {
                hitPos = (mousePosition - firePointPosition) * 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }

            else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }

            Effect(hitPos, hitNormal);
            timeToSpawnEffect = Time.time + 1 / effectSpawnTime;
        }
    }

    void Effect(Vector3 hitPos, Vector3 hitNormal)
    {
        Transform line = Instantiate(BulletTrailPrefab, FirePoint.position, FirePoint.rotation) as Transform;      //Bullet trail effect spwaning. 
        LineRenderer lr = line.GetComponent<LineRenderer>();
        Transform clone = Instantiate(MuzzleFlashPrefab, FirePoint.position, FirePoint.rotation) as Transform;

        if(lr != null)
        {
           lr.SetPosition(0, FirePoint.position);
           lr.SetPosition(1, hitPos);
        }

        Destroy(line.gameObject, 0.04f);

        if(hitNormal != new Vector3(9999, 9999, 9999))
        {
            Transform hitParticle = Instantiate(HitPrefab, hitPos, Quaternion.FromToRotation(Vector3.right, hitNormal)) as Transform;
            Destroy(hitParticle.gameObject, 1f);
        }

        clone.parent = FirePoint;
        float size = Random.Range(0.6f, 0.9f);
        clone.localScale = new Vector3(size, size, size);
        Destroy(clone.gameObject, 0.02f);

        camShake.Shake(camShakeAmt, camShakeLength);

        audioManager.PlaySound(weaponShootSound);
    }
}
