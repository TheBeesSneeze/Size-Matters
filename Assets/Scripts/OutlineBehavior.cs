using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineBehavior : MonoBehaviour
{
    [Header("Colors")]
    public Color NoLookColor; //todo
    public Color IdleColor;
    public Color GrowColor;
    public Color ShrinkColor;
    public Color AtLimitColor; //todo

    [Header("Sounds")]
    public AudioClip GrowingSound;
    public AudioClip ShrinkingSound;
    public AudioClip BoundsSound;

    [Header("Unity")]
    private Outline outline;
    private LineRenderer lineRenderer;
    private GrowthGun.ResizingState lastState;
    
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

            if (GrowingSound != null && lastState != GrowthGun.ResizingState.Growing)
            {
                AudioSource.PlayClipAtPoint(GrowingSound, transform.position);
            }
        }

        if (GrowthGun.Instance.ResizeState == GrowthGun.ResizingState.Shrinking)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, gunPt);
            outline.OutlineColor = GrowColor;
            outline.OutlineColor = ShrinkColor;

            if (ShrinkingSound != null && lastState != GrowthGun.ResizingState.Shrinking)
            {
                AudioSource.PlayClipAtPoint(ShrinkingSound, transform.position);
            }
        }

        if (GrowthGun.Instance.ResizeState == GrowthGun.ResizingState.Bounds)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, gunPt);
            outline.OutlineColor = GrowColor;
            outline.OutlineColor = AtLimitColor;

            if (BoundsSound != null && lastState != GrowthGun.ResizingState.Bounds)
            {
                AudioSource.PlayClipAtPoint(BoundsSound, transform.position);
            }
        }

        lastState = GrowthGun.Instance.ResizeState;
    }

    public void StopOutlining()
    {
        ResetLineRenderer();
        outline.OutlineColor = NoLookColor;
    }
}
