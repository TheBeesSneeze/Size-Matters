using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class CrosshairManager : MonoBehaviour
{
    public enum Mode
    {
        Empty,
        Fill,
        X,
        Hand,
    };

    [Tooltip("Automatically sets the crosshair image")]
    [HideInInspector] public Mode Crosshair
    {
        get { return _crossHair; }
        set { SetCrosshair(value); }
    }
    private Mode _crossHair;

    private UnityEngine.UI.Image UICrosshairImage;
    //public Sprite UICrosshairmage;

    public Sprite CrosshairEmpty;
    public Sprite CrosshairFill;
    public Sprite CrosshairX;
    public Sprite CrosshairHand;
    
    public static CrosshairManager Instance;

    private InterractableObject lookingObject;
    private PickUpController pickup;
    private Transform cam;

    public void SetCrosshair(Mode mode)
    {
        if (_crossHair == mode) return; //this helps me sleep at night

        _crossHair = mode;

        switch(mode)
        {
            case Mode.Empty:
                UICrosshairImage.sprite = CrosshairEmpty;
                break;
            case Mode.Fill:
                UICrosshairImage.sprite = CrosshairFill;
                break;
            case Mode.X:
                UICrosshairImage.sprite = CrosshairX;
                break;
            case Mode.Hand:
                UICrosshairImage.sprite = CrosshairHand;
                break;
        }
    }

    /// <summary>
    /// keeps the little hand crosshair guy there
    /// this code fucking sucks but i cant think of any other way to do this.
    /// </summary>
    public IEnumerator CheckIfPlayerLooking(InterractableObject obj)
    {
        lookingObject = obj;

        while (obj.PlayerLooking && !pickup.CurrentlyHolding)
        {
            UpdateCrosshair();

            yield return null;
        }
    }

    /// <summary>
    /// call while looking at an object
    /// </summary>
    private void UpdateCrosshair()
    {
        float distance = Vector3.Distance(cam.position, lookingObject.transform.position);

        if (CheckIfCanPickup())
        {
            Crosshair = Mode.Hand;
            return;
        }

        Crosshair = Mode.Fill;
    }

    //awesome code to be running every frame
    private bool CheckIfCanPickup()
    {
        Transform raycastOrigin = Camera.main.transform;

        //thanks alec for letting me steal your code
        Ray originPoint =
            raycastOrigin
                ? new Ray(raycastOrigin.position, raycastOrigin.forward)
                : Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(originPoint, out RaycastHit hit, PickUpController.Instance.MaxPickupDistance))
        {
            if (hit.rigidbody == null)
            {
                return false ;
            }

            if (hit.rigidbody.TryGetComponent(out InterractableObject interact))
            {
                //fumbles around and tries to find SOME reason not to do it
                if (interact == null) return false;

                if (!interact.CanBePickedUp) return false;

                if (interact.GetWeight() > PickUpController.Instance.MaxWeight) return false;

                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if(Crosshair == Mode.X && !InputManager.Instance.PickUpPressed() && lookingObject==null)
        {
            Crosshair = Mode.Empty ;
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
    }
    private void Start()
    {
        UICrosshairImage = gameObject.GetComponent<UnityEngine.UI.Image>();
        pickup = PickUpController.Instance;
        cam = Camera.main.transform;
    }
}
