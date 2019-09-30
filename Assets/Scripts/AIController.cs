using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private StateMachine stateMachine = new StateMachine();
    [SerializeField] private LayerMask foodItemslayer;
    [SerializeField] private float ViewRange;
    [SerializeField] private string foodItemsTag;
    private NavMeshAgent navMeshAgent;
    private SearchState searchForState;
    [SerializeField] public GameObject player => GameObject.FindGameObjectWithTag("Player");

    private void Start()
    {
        searchForState = new SearchState(player, transform);
        this.stateMachine.ChangeState(searchForState);
    }

    private void Update()
    {
        print(searchForState.targetFound);
        if (searchForState.targetFound)
        {
            print("Chase");
            this.stateMachine.ChangeState(new ChaseState(player, gameObject));
        }

        if (new ChaseState(player, gameObject).lostSight())
        {
            this.stateMachine.ChangeState(new SearchState(player, transform));
            print("Searching");
        }

        this.stateMachine.ExecuteStateUpdate();


    }


    public void TriggerEating()
    {
    }
}
