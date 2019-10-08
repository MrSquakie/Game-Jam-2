using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashlighttoggle : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject light;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            light.active = !light.active;
        }
    }
}
