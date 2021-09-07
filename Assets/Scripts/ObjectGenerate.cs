using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectGenerate : MonoBehaviour
{
    public GameObject SpawnedObject;

    private ARRaycastManager _ARRaycastManager;
    private List<ARRaycastHit> hits;
    void Start()
    {
        _ARRaycastManager = FindObjectOfType<ARRaycastManager>();
        hits = new List<ARRaycastHit>();
    }
    
    void Update()
    {
        SpawnObject();
    }

    public void SpawnObject()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _ARRaycastManager.Raycast(touch.position, hits, TrackableType.Planes);
                Instantiate(SpawnedObject, hits[0].pose.position, SpawnedObject.transform.rotation);
            }
        }
    }
}
