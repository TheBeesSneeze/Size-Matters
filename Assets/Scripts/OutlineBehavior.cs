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
    private LineRenderer lineRenderer;
    
    public void Start()
    {
        outline  = GetComponent<Outline>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = outline.SharedMat;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        ResetLineRenderer();
    }

    private void ResetLineRenderer()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    public void CorrectOutline()
    {
        Vector3 gunPt = GrowthGun.Instance.FirePoint.position;
        if (GrowthGun.Instance.ResizeState == GrowthGun.ResizingState.Idle)
        {
            ResetLineRenderer();
            outline.OutlineColor = IdleColor;
        }

        if (GrowthGun.Instance.ResizeState == GrowthGun.ResizingState.Growing)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, gunPt);
            outline.OutlineColor = GrowColor;
        }

        if (GrowthGun.Instance.ResizeState == GrowthGun.ResizingState.Shrinking)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, gunPt);
            outline.OutlineColor = GrowColor;
            outline.OutlineColor = ShrinkColor;
        }

        if (GrowthGun.Instance.ResizeState == GrowthGun.ResizingState.Bounds)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, gunPt);
            outline.OutlineColor = GrowColor;
            outline.OutlineColor = AtLimitColor;
        }
    }

    public void StopOutlining()
    {
        outline.OutlineColor = NoLookColor;
    }
}
