using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateBehavior : MonoBehaviour
{
    float weight = 0f;
    List<InterractableObject> ObjOnPlate = new List<InterractableObject>();
    void Start()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<InterractableObject>() != null) {
            ObjOnPlate.Add(collision.gameObject.GetComponent<InterractableObject>());
        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<InterractableObject>() != null)
        {
            ObjOnPlate.Remove(collision.gameObject.GetComponent<InterractableObject>());
        } 
    }
    private void Update()
    {
        float temp = 0;
        for (int i = 0; i < ObjOnPlate.Count; i++) {
            temp += ObjOnPlate[i].GetWeight();
        }
        weight = temp;
        print(weight);
    }
}
