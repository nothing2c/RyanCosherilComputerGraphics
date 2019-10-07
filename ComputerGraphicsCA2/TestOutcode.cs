using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOutcode : MonoBehaviour
{
    void Start()
    {
        Outcode a = new Outcode(new Vector2(3, 0.4f));

        Outcode b = new Outcode(new Vector2(0.4f, 0.7f));
        Outcode inViewPort = new Outcode();
        if ((a == inViewPort) && (b == inViewPort))
        {
            Debug.Log("Trivially Accept");
        }

        if(a * b != inViewPort)
        {
            Debug.Log("Trivially Rejected");
        }

        if((a + b) == inViewPort)
        {
            Debug.Log("Trivially Accept");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
