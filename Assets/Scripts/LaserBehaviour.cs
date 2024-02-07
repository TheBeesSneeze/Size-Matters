using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//thank you alec for basically supplying the code for this script
//also thank you youtube tutorials i love you
public class LaserBehaviour : MonoBehaviour
{
    public float maxDistance;
    private LineRenderer lineRenderer;
    private Vector3 endPoint;

    private void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        
    }

    private void Update()
    {
        DetectPlayer();
        DrawLine();
    }

    public void DetectPlayer()
    {
        Ray originPoint =  new Ray(transform.position, transform.forward);

        if (Physics.Raycast(originPoint, out RaycastHit hit, maxDistance))  
        {
            if (hit.rigidbody && hit.rigidbody.TryGetComponent(out PlayerController player))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                return;
            }

            endPoint = hit.point;
        }
        
        else
        {
           endPoint = transform.position + (transform.forward * maxDistance);
        }
    }

    public void DrawLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
    }
}
    
