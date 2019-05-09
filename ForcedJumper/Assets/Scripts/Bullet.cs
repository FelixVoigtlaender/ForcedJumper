using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Vector2 velocity;
    public LayerMask layer;
    public float radius;

    public bool isBomb;
    public int boomParticles = 10;

    public void Start()
    {
        Invoke("Kill", 2f);
    }

    public void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity * Time.deltaTime, (velocity * Time.deltaTime).magnitude, layer);
        if (hit)
        {
            transform.position = hit.point;
            hit.transform.GetComponent<Health>().DeltaHealth(-1);
            //PlayerController.instance.Clone(hit.transform.gameObject);           

            Kill();
        }
        transform.position += (Vector3)(Time.deltaTime * velocity);
        

    }

    public void Kill()
    {
        if (isBomb)
        {
            for (int i = 0; i < boomParticles; i++)
            {
                Vector2 dir = (Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.right);
                GameObject ob_Bullet = Instantiate(gameObject, transform.position, transform.rotation);
                Bullet bullet = ob_Bullet.GetComponent<Bullet>();
                bullet.isBomb = false;
                bullet.velocity = dir.normalized * Random.Range(1f - 0.5f, 1f + 0.5f) * 10;
            }

            CameraController.Shake(0.2f);
        }
        Destroy(gameObject);
    }
}
