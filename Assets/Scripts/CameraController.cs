using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    void SetCameraTransfomation(Transform tra)
    {
        gameObject.transform.position = tra.position;
        gameObject.transform.rotation = tra.rotation;
   
    }
}
