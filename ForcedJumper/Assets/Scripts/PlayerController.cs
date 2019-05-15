using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class PlayerController : MonoBehaviour {

    public static PlayerController instance;

    Vector2 input;

    public float maxSpeed = 10;
    public float speedTime = 0.1f;
    Vector2 dampVelocity;
    public float gravity;
    public float bounceSpeed;
    public float recoilSpeed;
    public LayerMask obstacleLayer;

    public float rotationVel;
    public float currentRotation;
    public float maxX;

    public GameObject bulletPrefab;
    public float angleOffset;
    public float bulletSpeed;


    int bullets = 4;

    Vector2 velocity;
    //UI
    public RawImage[] bulletHealthSprites; 


    Health health;

    public void Start()
    {
        instance = this;
        health = GetComponent<Health>();
        health.health = 0;
        //UI
        bullets = bulletHealthSprites.Length;
    }


    public void UpdateUI()
    {
        for(int i=0;i< bulletHealthSprites.Length;i++)
        {
            bulletHealthSprites[i].enabled = i < bullets;
        }
    }

    public void Update()
    {


        if (Input.GetButtonDown("Fire1") )
        {
            Shoot();
        }

        Vector2 position = transform.position;
        velocity.y -= gravity * Time.deltaTime;
        input.x = Input.acceleration.x*2f;
        if(Application.isEditor)
            input.x = Input.GetAxis("Horizontal");
        velocity.x = Mathf.SmoothDamp(velocity.x, maxSpeed * input.x, ref dampVelocity.x, speedTime);

        CheckPhysics();



        if (Mathf.Abs(transform.position.x) > maxX)
        {
            position.x = Mathf.Sign(transform.position.x) * maxX;
        }

        currentRotation += rotationVel * Time.deltaTime;
        transform.rotation = Quaternion.Euler(Vector3.forward * currentRotation);
        transform.position = velocity * Time.deltaTime + position;

        //UI
        UpdateUI();
    }


    public void SetBullets(int value)
    {
        bullets = value;
    }

    public void CheckPhysics()
    {
        Vector2 dir = new Vector2(Mathf.Sign(velocity.x), Mathf.Sign(velocity.y));
        float width = 0.5f;
        int steps = 2;
        float skinWidth = 0.02f;
        float stepSize = (width - 2 * skinWidth) / steps;
        float shake = 0f;
        //Vertical
        List<RaycastHit2D> verticalHits = new List<RaycastHit2D>();
        Vector2 left = (width - 2 * skinWidth) / 2 * Vector2.left + (Vector2)transform.position;
        left += (dir.y < 0 ? Vector2.down : Vector2.up) * (width) / 2;
        for (int i = 0; i <= steps; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(left + Vector2.right * stepSize * i, Vector2.up * dir.y, Mathf.Abs(velocity.y * Time.deltaTime) , obstacleLayer);
            if (hit)
            {
                verticalHits.Add(hit);

                velocity.y = dir.y < 0 ? bounceSpeed : -bounceSpeed;

                shake = 0.1f;

                rotationVel = dir.x < 0 ? 180 : -180;
            }
        }
        CameraController.Shake(shake);
        //Horizontal
        List<RaycastHit2D> horizontalHits = new List<RaycastHit2D>();
        Vector2 bottom = (width - 2 * skinWidth) / 2 * Vector2.down + (Vector2)transform.position;
        bottom += (dir.x * Vector2.right) * (width) / 2;
        for (int i = 0; i <= steps; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(bottom + Vector2.up * stepSize * i, Vector2.right * dir.x, Mathf.Abs(velocity.x * Time.deltaTime) , obstacleLayer);
            if (hit)
            {
                horizontalHits.Add(hit);
                velocity.x = 0;
            }
        }

        bool countChanged = false;
        countChanged = HandleEnemyHits(verticalHits);
        if (!countChanged)
        {
            countChanged = HandleEnemyHits(horizontalHits);
        }

        /*//Damage: Hit bottom&horizontal but no health -> Damage
        if (verticalHits.Count > 0 && dir.y < 0 && !countChanged)
            bullets--;
        */
        //Death
        if (bullets < 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    //Returns if blocks had same health
    public bool HandleEnemyHits(List<RaycastHit2D> hits, bool canDamage = false)
    {
        bool countChanged = false;
        bool wrongHit = false;
        foreach (RaycastHit2D hit in hits)
        {
            DestroyManager enemyDes = hit.transform.gameObject.GetComponent<DestroyManager>();
            Health enemyHealth = hit.transform.gameObject.GetComponent<Health>();
            if (enemyHealth && enemyHealth != health && !countChanged)
            {
                if (enemyHealth.health == health.health)
                {
                    enemyHealth.Kill();
                    bullets++;

                    countChanged = true;
                    health.SetHealth((health.health + 1) % 5);
                }
                else
                {
                    wrongHit = true;
                }
            }
        }
        bullets = Mathf.Clamp(bullets, 0, bulletHealthSprites.Length);

        return countChanged;
    }

    public void Shoot()
    {
        if (bullets <= 0)
        {
            return;
        }
        /*for(int i = 0; i < bullets; i++)
        {
            GameObject ob_bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(Vector2.zero));
            Bullet bullet = ob_bullet.GetComponent<Bullet>();
            float yDir = i % 2 == 0 ? -1 : 1;
            float angle = angleOffset * i * yDir;
            Vector2 dir = (Quaternion.Euler(0, 0, angle ) * Vector2.down);
            bullet.velocity = dir.normalized * bulletSpeed;
        }*/
        GameObject ob_bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(Vector2.zero));

        velocity.y = recoilSpeed;
        velocity.x = 0;
        CameraController.Shake(0.1f);

        bullets = Mathf.Clamp(bullets - 1, 0, 2);
        rotationVel *= -1;

    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector2.up * transform.position.y - Vector2.right * maxX, Vector2.up * transform.position.y + Vector2.right * maxX);
    }

    public void Clone(GameObject box)
    {
        Health boxHealth = box.GetComponent<Health>();

        transform.position = box.transform.position;
        velocity.y = -bounceSpeed;

        health.SetHealth(boxHealth.health);
        boxHealth.Kill();

    }
}
