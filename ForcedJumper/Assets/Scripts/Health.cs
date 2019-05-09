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
            value = 5 + value;
        }
        value = value % 5;
        print(value);
        if (health != value)
        {
            if (onDeltaHealth != null)
                onDeltaHealth(value - health);
        }


        health = value;
        SetText(health + "");
    }

    public void Kill()
    {
        SetHealth(-1);
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
