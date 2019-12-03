using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    public static CameraControl inst;
    private Transform startingcameraTransform;
    private GameObject terrain;

    void Awake()
    {
        if (inst != null)
            return;
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

        // Update is called once per frame
        void Update()
    {
        
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
}
