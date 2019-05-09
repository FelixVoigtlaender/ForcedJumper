using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class DestroyManager : MonoBehaviour {
    Collider2D myCollider;
    Vector2 startScale;
    SpriteRenderer sprite;
    Color startColor;
    Color endColor;
    float disapearTime = 0.2f;
    float timeLeft;

    public Text textHealth;

    public bool isDisapear = false;

    public Health health;//penis

    public void Start()
    {
        myCollider = GetComponent<Collider2D>();
        startScale = transform.localScale;
        sprite = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        health.onKill += Kill;


        timeLeft = disapearTime;


        SetText(health + "");
    }

    public void Update()
    {
        if (isDisapear)
        {
            float percent = timeLeft / disapearTime;
            myCollider.enabled = false;
            timeLeft -= Time.deltaTime;
            transform.localScale = startScale * (1 - percent) * 5f;
            sprite.color = Color.Lerp(endColor, startColor, percent);


            if (timeLeft < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Kill()
    {
        isDisapear = true;
        //Color
        startColor = sprite.color;
        endColor = Color.white;
        endColor.a = 0;
    }

    void SetText(string text)
    {
        if (textHealth)
        {
            textHealth.text = text;
        }
    }
}
