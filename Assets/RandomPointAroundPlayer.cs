using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPointAroundPlayer : MonoBehaviour
{
    public Transform PlayerPos;
    public Vector3 RandomPoint;

    public void Start()
    {

    }

    public Vector3 GetRandomPositon()

    {
        PlayerPos = GameObject.FindGameObjectWithTag("Player").transform;
        RandomPoint = PlayerPos.position + Random.insideUnitSphere * 50;
        return RandomPoint;
    }
}
