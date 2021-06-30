using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARPlacment : MonoBehaviour
{
    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    private GameObject spawnedObject;

    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;
    private bool placementPoseIsValid = false;


    private Transform startMarker;
    private Transform endMarker;

    public float speed = 0.0001f;
    private float startTime;
    private float journeyLength;

    private Vector3 planeNormal;

    public LineRenderer laserLineRenderer;
    public float laserWidth = 0.1f;
    public float laserMaxLength = 5f;

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
            showToast("Hello", 2);
        }
        if(spawnedObject == null){
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }else{
             placementIndicator.SetActive(false);
             updateSpawnRandomMove();
             DetectingIsObjectHit();
        }

    }


    private float q = 0.0f;
    void DetectingIsObjectHit(){
          

          if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
             
             var touch = Input.GetTouch(0);     
              Ray ray = Camera.current.ScreenPointToRay(touch.position);
              RaycastHit hit;
                 
              if (Physics.Raycast(ray, out hit)){
                  

                        Vector3 incomingVec = hit.point - Camera.current.transform.position;
                        // Use the point's normal to calculate the reflection vector.
                        Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);
                        Debug.DrawLine(Camera.current.transform.position, hit.point, Color.red);
                        Debug.DrawRay(hit.point, reflectVec, Color.green);
                       
                        showToast(hit.transform.parent.gameObject.tag, 2);     
    
                  
              }
          }

    }

   void ShootLaserFromTargetPosition( Vector3 targetPosition, Vector3 direction, float length )
     {
         Ray ray = new Ray( targetPosition, direction );
         RaycastHit raycastHit;
         Vector3 endPosition = targetPosition + ( length * direction );
 
         if( Physics.Raycast( ray, out raycastHit, length ) ) {
             endPosition = raycastHit.point;
         }
 
         laserLineRenderer.SetPosition( 0, targetPosition );
         laserLineRenderer.SetPosition( 1, endPosition );
     }


    public Text txt;
    void showToast(string text,
        int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text,
        int duration)
    {
        Color orginalColor = Color.white;

        txt.text = text;
        txt.enabled = true;

        //Fade in
        yield return fadeInAndOut(txt, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(txt, false, 0.5f);

        txt.enabled = false;
        txt.color = orginalColor;
    }

    IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color =  Color.white;
            yield return null;
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
             
             /*
                journeyLength =  Vector3.Distance(startMarker.position, endMarker.position);
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                spawnedObject.transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
            */
               
    }
}

