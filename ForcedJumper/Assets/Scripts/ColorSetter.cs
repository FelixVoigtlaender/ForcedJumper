using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class ColorSetter : MonoBehaviour {
    Health health;

    public Color colorStart, colorEnd;
    public int maxHealth;
    SpriteRenderer sprite;

    public void Start()
    {
        health = GetComponent<Health>();
        health.onDeltaHealth += UpdateColor;
        sprite = GetComponent<SpriteRenderer>();


        UpdateColor();
    }



    public void UpdateColor(int delta = 0)
    {
        print("Color Changed");
        sprite.color = Color.Lerp(colorStart, colorEnd, (float)health.health / (float)maxHealth);
    }
}
