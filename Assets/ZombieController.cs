using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();

    private Rigidbody rb => GetComponentInParent<Rigidbody>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (rb.velocity.magnitude > 0.2f)
        //{
            anim.SetFloat("VelX", rb.velocity.x * 5f);
            anim.SetFloat("VelY", rb.velocity.y * 5f);
       // }
    }
}
