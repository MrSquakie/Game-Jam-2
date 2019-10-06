using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Image healthbar;
    public PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        healthbar = GetComponent<Image>();
        GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        healthbar.fillAmount = playerHealth.currentHealth/100;
    }
}
