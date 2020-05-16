using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Camera cam;
    public float zoomstep = 10;
    public float scrollstep = 100;
    public float rotationSpeed = 1;

    //Set by the gamemanager -> The borders of the map
    public Vector2 camLimit;
    //Limits the vertical rotation of the camera
    public Vector2 vRotLimit = new Vector2(40, 90);

    private float minzoom = 0; 
    private float maxzoom = 10;
    private float currentzoom = 5;
    // Radius is a horizontal radius, used to move the MapBorders
    private float radius;
    //vradius is the radius used for rotating the camera around the focus point on the (y=0)-Plane
    private float vradius;

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
        float mouse_y = Input.GetAxis("Mouse Y") * rotationSpeed;

        // The center of the sphere on which the Camera is placed
        Vector3 center = cam.transform.position + cam.transform.rotation * new Vector3(0, 0, vradius);//Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * new Vector3(0, 0, radius) - new Vector3(0, cam.transform.position.y, 0);
        Vector3 oldEuler = cam.transform.eulerAngles;

        //Orienting and repositioning of the camera
        cam.transform.eulerAngles = new Vector3(Mathf.Clamp(oldEuler.x-mouse_y, vRotLimit.x, vRotLimit.y), oldEuler.y + mouse_x, oldEuler.z);
        cam.transform.position = center - cam.transform.rotation * new Vector3(0, 0, vradius);
        UpdateRadius();
    }

    void MoveCamera()
    {
        Quaternion y_rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0); //CurrentRotation of Camera ignoring the xz-Plane
        Vector3 mouseMovement = new Vector3(Input.GetAxis("Mouse X") * scrollstep, 0, Input.GetAxis("Mouse Y") * scrollstep); //-Movement on xz-Plane
        cam.transform.position -=  y_rotation * mouseMovement;
    }


    //This Method makes the Players view zooming, by moving the Camera in the Direction it currently is focused on.
    // Because we zoom like this we can change the FieldOfView as we wish without an impact on the zoom mechanic
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
        vradius = Mathf.Sqrt(Mathf.Pow(cam.transform.position.y, 2) + Mathf.Pow(radius, 2));
    }

    void SelectObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitted = hit.collider.gameObject;
            // If the decoration of a Tile is hit, it has a parent, so the parentobject is choosen instead
            if (hitted.transform.parent)
            {
                hitted = hitted.transform.parent.gameObject;
            }
            Debug.Log(hitted.name);
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
