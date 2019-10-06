using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image health;
    public float maxHealth = 1;
    public float currentHealth = 1;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        health.fillAmount = currentHealth;

        if (currentHealth <= 0)
            Destroy(this.gameObject);

        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;

        print(currentHealth);
    }
}
