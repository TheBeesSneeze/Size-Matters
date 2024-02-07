using System;
using UnityEngine;

[Serializable]
public class Oscillator
{
    public bool Wrapped { get;  set; }
    public float currentPhase;
    public float currentFrequency = 1f;

    public void Advance(float amt)
    {
        Wrapped = false;
        currentPhase += amt * currentFrequency;
        if (currentPhase > 6.28318548f)
        {
            Wrapped = true;
            currentPhase = 0f;
        }
    }

    public float Eval()
    {
        return Mathf.Sin(currentPhase);
    }
}