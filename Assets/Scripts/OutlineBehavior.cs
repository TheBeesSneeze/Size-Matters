using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineBehavior : MonoBehaviour
{
    public Color NoLookColor; //todo
    public Color IdleColor;
    public Color GrowColor;
    public Color ShrinkColor;
    public Color AtLimitColor; //todo

    private Outline outline;

    public void Start()
    {
        outline  = GetComponent<Outline>();
    }

    public void CorrectOutline()
    {
        if (GrowthGun.Instance.ResizeState == GrowthGun.ResizingState.Idle)
        {
            outline.OutlineColor = IdleColor;
        }

        if (GrowthGun.Instance.ResizeState == GrowthGun.ResizingState.Growing)
        {
            outline.OutlineColor = GrowColor;
        }

        if (GrowthGun.Instance.ResizeState == GrowthGun.ResizingState.Shrinking)
        {
            outline.OutlineColor = ShrinkColor;
        }
    }
}
