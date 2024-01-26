using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PressurePlateBehavior : MonoBehaviour
{
    float weight = 0f;
    public bool ExactTargetMode = true;
    public float minimumDetectableWeight = 0;
    public float maximumDetectableWeight = 100;
    public float TargetWeight = 1;
    public float TargetWeightErrorMargin = 0.1f;
    public bool triggered = false;
    public GameObject WeightDisplayText;
    List<InterractableObject> ObjOnPlate = new List<InterractableObject>();
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<InterractableObject>() != null) {
            ObjOnPlate.Add(collision.gameObject.GetComponent<InterractableObject>());
        }
        print(weight);
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
        if (weight < minimumDetectableWeight)
        {
            weight = minimumDetectableWeight;
        }
        else if (weight > maximumDetectableWeight)
        {
            weight = maximumDetectableWeight;
        }
        else 
        {
            weight = temp;
        }
        if (ExactTargetMode)
        {
            if (weight <= (TargetWeight + TargetWeightErrorMargin) &&
            weight >= (TargetWeight - TargetWeightErrorMargin))
            {
                triggered = true;
            }
            else
            {
                triggered = false;
            }
        }
        else {
            if (weight >= TargetWeight)
            {
                triggered = true;
            }
            else
            {
                triggered = false;
            }
        }
        if (ExactTargetMode)
        {
            WeightDisplayText.GetComponent<TextMeshPro>().text = ("" + weight + "\nTarget: " + TargetWeight + "+-" + TargetWeightErrorMargin + "; Triggered: " + triggered);
        }
        else {
            WeightDisplayText.GetComponent<TextMeshPro>().text = ("" + weight + "\nTarget: " + TargetWeight + "; Triggered: " + triggered);
        }
    }
}
