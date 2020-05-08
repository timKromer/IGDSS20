using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Camera cam;
    public float zoomstep = 10;
    public float scrollstep = 100;
    public float rotateSpeed = 1;

    //Set by the gamemanager -> The borders of the map
    public Vector2 camLimit;

    private float minzoom = 0; 
    private float maxzoom = 10;
    private float currentzoom = 5;
    private float firstRadius = 0;
    private float radius;

    // This is a plane marking y = 0 for rotating the camera
    private Plane zeroPlane = new Plane(Vector3.up, Vector3.zero);


    // Start is called before the first frame update
    void Start()
    {

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        float distance = 0;
        if (zeroPlane.Raycast(ray, out distance))
        {
            firstRadius = Mathf.Sqrt(Mathf.Pow(distance, 2) - Mathf.Pow(cam.transform.position.y, 2));
            radius = firstRadius;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Leftklick
        if (Input.GetMouseButtonDown(0))
        {
            SelectObject();
        }
        // Rightclick
        if (Input.GetMouseButton(1))
        {
            MoveCamera();
        }
        else if (Input.GetMouseButton(2))
        {
            RotateCamera();
        }
        // Mousewheel
        Zoom();
        //Limits the CameraMovementSpace - The middle of the screen isnt allowed to got further than the last tile
        Vector3 scrollfactor = Quaternion.Euler(0, cam.transform.eulerAngles.y,0) * new Vector3(0, 0, radius);
        cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, -camLimit.x -scrollfactor.x, camLimit.x - scrollfactor.x), cam.transform.position.y, Mathf.Clamp(cam.transform.position.z, -camLimit.y - scrollfactor.z, camLimit.y - scrollfactor.z));
    }

    /*
     * This Method rotates the camera by an y-axis which runs throug the point, where the camera center hits y=0
     */
    void RotateCamera()
    {
            //distance mod is a multiplier used for having the same camera-movement on the circle, independent of the circle distance
            //TODO CHECK IF NECASSARY
            //float distance_mod = (firstRadius / firstRadius);
            float mouse_x = Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed;// * distance_mod;
            
            //center of rotation is on distance of radius in front of camera-middle
            Vector3 center = cam.transform.position + Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * new Vector3(0, 0, radius);
            cam.transform.eulerAngles += new Vector3(0, mouse_x, 0);
            cam.transform.position = center - Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * new Vector3(0, 0, radius);
        
    }

    void MoveCamera()
    {
        //TODO -> Feel of drag along the tiles -> Mouse should allways lie on the same tile while draging
        Quaternion y_rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
        Vector3 mouseMovement = new Vector3(Input.GetAxis("Mouse X") * Time.deltaTime * scrollstep, 0, Input.GetAxis("Mouse Y") * Time.deltaTime * scrollstep);
        cam.transform.position -=  y_rotation * mouseMovement;
    }

    void Zoom()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        float distance = 0;
        if (zeroPlane.Raycast(ray, out distance))
        {
            radius = Mathf.Sqrt(Mathf.Pow(distance, 2) - Mathf.Pow(cam.transform.position.y, 2));
        }
        float lastzoom = currentzoom;
        //mouseScrollDelta.y is the amount of scrolling the mouse wheel forwards/backwards
        //TODO -> Boundaries for camera movement
        currentzoom = Mathf.Clamp(currentzoom +Input.mouseScrollDelta.y, minzoom, maxzoom);// Mathf.Min(Mathf.Max(currentzoom + Input.mouseScrollDelta.y, minzoom), maxzoom);
        cam.transform.position += (currentzoom - lastzoom) * (cam.transform.localRotation * new Vector3(0, 0, zoomstep));
    }

    void SelectObject()
    {
        
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
    }
}
