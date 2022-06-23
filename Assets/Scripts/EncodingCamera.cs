using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class EncodingCamera : MonoBehaviour
{
    private Camera camera;
    void Awake()
    {
        this.camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
