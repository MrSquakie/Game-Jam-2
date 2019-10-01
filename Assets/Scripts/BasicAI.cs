using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BasicAI : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator => GetComponent<Animator>();
    private NavMeshAgent agent => GetComponent<NavMeshAgent>();
    [SerializeField] private GameObject player;
    [SerializeField] private float extraRotationSpeed;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Goto(player.transform);
        extraRotation();
    }

    public void Goto(Transform target)
    {
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
            animator.SetFloat("VelX", Mathf.Abs(agent.velocity.x*10));
            animator.SetFloat("VelY", Mathf.Abs(agent.velocity.y*10));
        }
    }

    void extraRotation()
    {
        Vector3 lookrotation = agent.steeringTarget - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), extraRotationSpeed * Time.deltaTime);

    }
}
