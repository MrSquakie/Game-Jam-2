using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ChaseState : IState
{
    public GameObject target;

    public GameObject currentObject;

    public ChaseState(GameObject target, GameObject currentObject)
    {
        //current object = enemy transform
        this.target = target;
        this.currentObject = currentObject;
    }

    public void Enter()
    {

    }

    public void Execute()
    {

        currentObject.transform.rotation = Quaternion.Lerp(currentObject.transform.rotation, Quaternion.LookRotation(target.transform.position - currentObject.transform.position), Time.deltaTime*5f);
        //currentObject.transform.LookAt(target.transform);
        currentObject.transform.Translate(Vector3.forward *Time.deltaTime);
        if (lostSight())
        {

        }
    }

    public bool lostSight()
    {
        Vector3 targetDir = target.transform.position - currentObject.transform.position;
        float angleToPlayer = (Vector3.Angle(currentObject.transform.position, currentObject.transform.forward));
        if (angleToPlayer >= -90 && angleToPlayer <= 90)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void Exit()
    {

    }

}
