using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacment : MonoBehaviour
{
    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    private GameObject spawnedObject;

    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;
    private bool placementPoseIsValid = false;


    public Transform startMarker;
    public Transform endMarker;

    public float speed = 0.001f;
    private float startTime;
    private float journeyLength;

    private Vector3 planeNormal;


    // Start is called before the first frame update
    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        aRPlaneManager = FindObjectOfType<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceObject();
        }
        if(spawnedObject == null){
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }else{
             placementIndicator.SetActive(false);
             updateSpawnRandomMove();
        }
    }
    void UpdatePlacementIndicator(){
         if(spawnedObject == null  && placementPoseIsValid){
             placementIndicator.SetActive(true);
             placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
         }
    }
    void UpdatePlacementPose(){
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f,0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if(placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;

            ARPlane plane = aRPlaneManager.GetPlane(hits[0].trackableId);
            planeNormal = plane.normal ;
            planeNormal = Quaternion.Euler(-90, 0, 0) * planeNormal;
        }

    }

    void ARPlaceObject()
    {
        spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, PlacementPose.rotation);
        

        GameObject emptyGO = new GameObject();
        startMarker = emptyGO.transform;
        startMarker.position = new Vector3 (PlacementPose.position.x ,  PlacementPose.position.y , PlacementPose.position.z);
        startMarker.rotation = PlacementPose.rotation;
        GameObject emptyGO2 = new GameObject();
        endMarker = emptyGO2.transform;
        endMarker.rotation = PlacementPose.rotation;
        endMarker.position  = new Vector3 (PlacementPose.position.x ,  PlacementPose.position.y , PlacementPose.position.z)+ (planeNormal * 1.5f);
   
        startTime = Time.time;

    }


 

    void updateSpawnRandomMove(){
             
                journeyLength =  Vector3.Distance(startMarker.position, endMarker.position);
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                spawnedObject.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
            
               
       //  spawnedObject.transform.position =   endMarker.position  ;
    }
}
