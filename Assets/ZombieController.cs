using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private Pathfinding.RichAI ai => GetComponentInParent<Pathfinding.RichAI>();
    private Rigidbody rb => GetComponentInParent<Rigidbody>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ai.acceleration = 0f;
        anim.SetBool("hasDestination", true);
    }

   // private void OnAnimatorMove()
    //{
     //   rb.velocity = rb.deltaPosition / Time.deltaTime;
    //}
}
