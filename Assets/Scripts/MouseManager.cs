using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Camera cam;
    public float zoomstep;


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

        // Mousewheel
        Zoom();
    }

    void Zoom()
    {
        Vector3 cam_pos = cam.transform.position - currentzoom * (cam.transform.localRotation * new Vector3(0, 0, zoomstep));
        //mouseScrollDelta.y is the amount of scrolling the mouse wheel forwards/backwards
        currentzoom = Mathf.Min(Mathf.Max(currentzoom + Input.mouseScrollDelta.y, minzoom), maxzoom);


        cam.transform.position = cam_pos + currentzoom * (cam.transform.localRotation * new Vector3(0, 0, zoomstep));
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
