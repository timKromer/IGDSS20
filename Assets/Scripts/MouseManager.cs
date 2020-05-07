using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Camera cam;
    public float zoomstep;
    public float scrollstep;

    private float minzoom = 0; 
    private float maxzoom = 10;
    private float currentzoom = 5;

    
    
    // Start is called before the first frame update
    void Start()
    {

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
        // Mousewheel
        Zoom();
    }

    void MoveCamera()
    {
        Quaternion y_rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
        Vector3 mouseMovement = new Vector3(Input.GetAxis("Mouse X") * Time.deltaTime * scrollstep, 0, Input.GetAxis("Mouse Y") * Time.deltaTime * scrollstep);
        cam.transform.position -= y_rotation * mouseMovement;
    }

    void Zoom()
    {
        float lastzoom = currentzoom;
        //mouseScrollDelta.y is the amount of scrolling the mouse wheel forwards/backwards
        currentzoom = Mathf.Min(Mathf.Max(currentzoom + Input.mouseScrollDelta.y, minzoom), maxzoom);
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
