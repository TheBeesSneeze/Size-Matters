using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    public void open() {
        gameObject.SetActive(false);
    }
    public void close()
    {
        gameObject.SetActive(true);
    }
}
