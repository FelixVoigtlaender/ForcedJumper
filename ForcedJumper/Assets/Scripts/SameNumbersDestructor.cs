using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class SameNumbersDestructor : MonoBehaviour {
    Health health;
    public LayerMask layer;
    public float delay = 0.1f;

    void Start () {
        health = GetComponent<Health>();
        health.onKill += OnDeath;
	}
	
    void OnDeath()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        float distance = transform.lossyScale.x;
        //Up
        hits.AddRange(Physics2D.RaycastAll(transform.position, Vector2.up, distance, layer));
        //Right
        hits.AddRange(Physics2D.RaycastAll(transform.position, Vector2.right, distance, layer));
        //Down
        hits.AddRange(Physics2D.RaycastAll(transform.position, Vector2.down, distance, layer));
        //Left
        hits.AddRange(Physics2D.RaycastAll(transform.position, Vector2.left, distance, layer));

        foreach(RaycastHit2D hit in hits)
        {
            Health hitHealth = hit.transform.gameObject.GetComponent<Health>();
            if (hitHealth == health)
                continue;
            if (hitHealth.health == health.health)
                hitHealth.Invoke("Kill", delay);
        }
    }
}
