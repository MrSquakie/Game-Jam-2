using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float MaxHealth, health;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag == "Player")
        {
            MaxHealth = 100;
        }
        else if (gameObject.tag == "GenericEnemy")
        {
            MaxHealth = 20;
        }
        else
        {
            //assign health in inspector for dynamic maxHealth
        }
        health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        checkStatus();

        //tempp
        if (Input.GetKeyDown(KeyCode.K))
        {
            Damage(5f);
        }
        //
    }

    public void Damage(float damageAmount)
    {
        health -= damageAmount;
    }
    public void checkStatus()
    {
        if (health <= 0)
        {
            Debug.Log(gameObject.name + " died");
            Destroy(gameObject, 2f * Time.deltaTime);
        }
    }
}
