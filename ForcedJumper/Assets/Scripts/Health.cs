using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Health : MonoBehaviour {

    public int health;
    public Text textHealth;
    public event Action onKill;
    public event Action<int> onDeltaHealth;

    private void Start()
    {
        SetHealth(health);
    }

    public void DeltaHealth(int delta)
    {
        SetHealth(health + delta);
    }
    public void SetHealth(int value)
    {
        while (value < 0)
        {
            value = WorldManager.modulo + value;
        }
        value = value % WorldManager.modulo;
        print(value);
        bool healthChanged = value != health;


        health = value;
        if (healthChanged && onDeltaHealth != null)
            onDeltaHealth(value - health);
        SetText(health + "");
    }

    public void Kill()
    {
        if (onKill!=null)
            onKill();
    }

    void SetText(string text)
    {
        if (textHealth)
        {
            textHealth.text = text;
        }
    }
}
