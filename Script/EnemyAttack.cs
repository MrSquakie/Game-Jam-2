using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This is similar to PickUpObject script

public class EnemyAttack : MonoBehaviour
{
    public float damage = 0.2f;
    public PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    void Update()
    {

    }

    void OnTriggerStay()
    {
        print("hello");
        playerHealth.currentHealth -= damage;
    }
}
