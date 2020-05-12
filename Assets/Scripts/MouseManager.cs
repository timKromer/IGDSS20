using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Camera cam;
    public float zoomstep = 10;
    public float scrollstep = 100;
    public float rotationSpeed = 1;

    //Set by the gamemanager -> The borders of the map
    public Vector2 camLimit;

    private float minzoom = 0; 
    private float maxzoom = 10;
    private float currentzoom = 5;
    private float radius;


    // Start is called before the first frame update
    void Start()
    {
        UpdateRadius();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouse();
        //Allways Call this at the END of the update
        CorrectCameraPosition();
    }

    void HandleMouse()
    {
        Zoom();
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
    }


    /*
     * This Method rotates the camera by an y-axis which runs throug the point, where the camera center hits y=0
     * The Speed of the Rotation depends on the movement of the mouse and  rotationSpeed
     */
    void RotateCamera()
    {
        float mouse_x = Input.GetAxis("Mouse X") * rotationSpeed;
            
        // Rotation around y-Axis
        // Center is the Centerpoint of the Circle, the Camera is moving on
        Vector3 center = cam.transform.position + Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * new Vector3(0, 0, radius);
        cam.transform.eulerAngles += new Vector3(0, mouse_x, 0); //Angle Offset
        // Position on the Circle depends on the new Y-Angle
        cam.transform.position = center - Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * new Vector3(0, 0, radius);
    }

    void MoveCamera()
    {
        Quaternion y_rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0); //CurrentRotation of Camera ignoring the xz-Plane
        Vector3 mouseMovement = new Vector3(Input.GetAxis("Mouse X") * scrollstep, 0, Input.GetAxis("Mouse Y") * scrollstep); //-Movement on xz-Plane
        cam.transform.position -=  y_rotation * mouseMovement;
    }

    void Zoom()
    {
        float lastzoom = currentzoom;
        currentzoom = Mathf.Clamp(currentzoom +Input.mouseScrollDelta.y, minzoom, maxzoom);
        //                          DeltaZoom              *            Zoomspeed in current Camera-direction
        cam.transform.position += (currentzoom - lastzoom) * (cam.transform.localRotation * new Vector3(0, 0, zoomstep));
        UpdateRadius();

    }

    void UpdateRadius()
    {
        // Calculates the Radius of the circle on which the camera moves. It is calculated as follows: radius = tan(alpha) * height
        // The Hypothenuse is a line from the camera to the focus point on y=0 of the camera
        radius = Mathf.Tan(Mathf.Deg2Rad * (90 - cam.transform.eulerAngles.x)) * cam.transform.position.y;
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

    void CorrectCameraPosition()
    {
        // Limits the CameraMovementSpace. The Camera can go over the border of half of the current radius
        Vector3 scrollfactor = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * new Vector3(0, 0, radius);
        float xCorrection = Mathf.Clamp(cam.transform.position.x, -camLimit.x - scrollfactor.x + radius / 2f, camLimit.x - scrollfactor.x - radius / 2f);
        float zCorrection = Mathf.Clamp(cam.transform.position.z, -camLimit.y - scrollfactor.z + radius / 2f, camLimit.y - scrollfactor.z - radius / 2f);
        cam.transform.position = new Vector3(xCorrection, cam.transform.position.y, zCorrection);
    }
}
