using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateBehavior : MonoBehaviour
{
    InterractableObject objScript;
    float weight = 0f;
    void Start()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<InterractableObject>() != null) {
            objScript = collision.gameObject.GetComponent<InterractableObject>();
            weight += objScript.GetWeight();
        }
        print(weight);
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<InterractableObject>() != null)
        {
            objScript = collision.gameObject.GetComponent<InterractableObject>();
            weight -= objScript.GetWeight();
            print(weight);
        }
    }

}
