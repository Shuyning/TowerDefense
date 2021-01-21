using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCam : MonoBehaviour
{
    Vector3 touch;
    [SerializeField] float zoomMin = 1f;
    [SerializeField] float zoomMax = 14f;
    [SerializeField] float stepH;
    [SerializeField] float stepW;
    [SerializeField] float cofMove = 0.01f;


    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float cofH = stepH * (zoomMax - Camera.main.orthographicSize);
        float cofW = stepW * (zoomMax - Camera.main.orthographicSize);

        float camPosY = Camera.main.transform.position.y;
        float camPosX = Camera.main.transform.position.x;

        if(Input.GetMouseButtonDown(0))
        {
            touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroLastPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOneLastPos = touchOne.position - touchOne.deltaPosition;

            float distTouch = (touchZeroLastPos - touchOneLastPos).magnitude;
            float currentDistTouch = (touchZero.position - touchOne.position).magnitude;

            float difference = currentDistTouch - distTouch;

            if(cofH >= camPosY && cofW >= camPosX && -cofH <= camPosY && -cofW <= camPosX)
            {
                Zoom(difference * 0.01f, cofW, cofH);
            }
        }
        else if(Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 posDir = Camera.main.transform.position + touch - mousePos;

            Camera.main.transform.position = new Vector3(Mathf.Clamp(posDir.x, -cofW, cofW), Mathf.Clamp(posDir.y, -cofH, cofH), Camera.main.transform.position.z);
        }

        if(cofH >= camPosY && cofW >= camPosX && -cofH <= camPosY && -cofW <= camPosX)
        {
            Zoom(Input.GetAxis("Mouse ScrollWheel"), cofW, cofH);
        }
    }

    void Zoom(float increment, float cofW, float cofH)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
        Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, -cofW, cofW), Mathf.Clamp(Camera.main.transform.position.y, -cofH, cofH), Camera.main.transform.position.z);
    }
}
