using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CrosshairManager : MonoBehaviour
{
    public enum CrosshairMode
    {
        Empty,
        Fill,
        X,
        Hand,
    };

    [Tooltip("Automatically sets the crosshair image")]
    [HideInInspector] public CrosshairMode Crosshair
    {
        get { return _crossHair; }
        set { SetCrosshair(value); }
    }
    private CrosshairMode _crossHair;

    private Image UICrosshairImage;

    public Texture CrosshairEmpty;
    public Texture CrosshairFill;
    public Texture CrosshairX;
    public Texture CrosshairHand;
    
    public static CrosshairManager Instance;

    public void SetCrosshair(CrosshairMode mode)
    {
        _crossHair = mode;

        switch(mode)
        {
            case CrosshairMode.Empty:
                UICrosshairImage.image = CrosshairEmpty;
                break;
            case CrosshairMode.Fill:
                UICrosshairImage.image = CrosshairFill;
                break;
            case CrosshairMode.X:
                UICrosshairImage.image = CrosshairX;
                break;
            case CrosshairMode.Hand:
                UICrosshairImage.image = CrosshairHand;
                break;
        }
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        UICrosshairImage = GetComponent<Image>();
    }
}
