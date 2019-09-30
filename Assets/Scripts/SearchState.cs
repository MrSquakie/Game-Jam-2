using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : IState
{
    

    private GameObject player;
    private Transform currentTransform;
    public bool targetFound;
    public SearchState(GameObject player, Transform currentTransform)
    {
        this.player = player;
        this.currentTransform = currentTransform;
    }


    public void Enter()
    {

    }

    public void Execute()
    {
        //look within field of view for player.
        // if player in field of view
            //go to chase state
        //else
            // goto moverandom state. 

        int count = 0;
        Vector3 targetDir = player.transform.position - currentTransform.position;
        float angleToPlayer = (Vector3.Angle(currentTransform.position, currentTransform.forward));
        var rot = Quaternion.AngleAxis(angleToPlayer, Vector3.forward);
        var dir = rot * Vector3.forward;
        Debug.DrawRay(currentTransform.position, dir, Color.red);

        while (!targetFound && count < 5)
        {
            if (angleToPlayer >= -90 && angleToPlayer <= 90) //180 deg FOV
            {
                targetFound = true;
                break;
            }
            else
            {
                currentTransform.rotation = Quaternion.Lerp(currentTransform.rotation,
                    Quaternion.LookRotation(new Vector3(currentTransform.position.x * Random.Range(0, 5), 0f, currentTransform.position.z*Random.Range(0,5))),Time.deltaTime);
                targetFound = false;
            }
        }
        
    }

    public void Exit()
    {

    }
}
