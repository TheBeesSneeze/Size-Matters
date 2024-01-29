using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlateBehavior : MonoBehaviour
{
    float weight = 0f;
    public bool ExactTargetMode = true; // True: the pressure plate will only activate if the weight is exactly the target
                                        // False: the pressure plate will trigger if the weight is equal or above the target
    
    public float minimumDetectableWeight = 0; // if the weight is less than this then the pressure plate will not detect it
    public float maximumDetectableWeight = 100; // the weight the pressure plate can detect cannot exceed this number

    public float TargetWeight = 1; // the target weight
    public float TargetWeightErrorMargin = 0.1f; // pressure plate will still tigger when the weight is +- this value.
                                                 // this value is only important when exact target mode is active
    
    public bool triggered = false; 
    public GameObject WeightDisplayText;
    
    List<InterractableObject> ObjOnPlate = new List<InterractableObject>(); //list of objs on pressure plate
    public UnityEvent PressurePlateTriggered;
    public UnityEvent PressurePlateNotTriggered;
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
                PressurePlateTriggered.Invoke();
            }
            else
            {
                triggered = false;
                PressurePlateNotTriggered.Invoke();
            }
        }
        else {
            if (weight >= TargetWeight)
            {
                triggered = true;
                PressurePlateTriggered.Invoke();
            }
            else
            {
                triggered = false;
                PressurePlateNotTriggered.Invoke();
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
