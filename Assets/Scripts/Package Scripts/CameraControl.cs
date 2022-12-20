using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Vector3 touchStart;
    public float zoomOutMin = 1;
    public float zoomOutMax = 8;
    float speed = 5f;
    [SerializeField] PolygonCollider2D CameraConfinerBox;
    //[SerializeField] BoxCollider2D CameraConfinerBox;
    float minX, minY, maxX, maxY;
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        GetBounds();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 0.01f);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);


            Debug.Log($"Here is the direction that it will move {direction}");
            //Camera.main.transform.position += direction * speed;//<-- speed

            this.transform.position =  Vector3.Slerp(transform.position, transform.position + direction, speed * Time.deltaTime);
            float newX = Mathf.Clamp(this.transform.position.x, minX, maxX);
            float newY = Mathf.Clamp(this.transform.position.y, minY, maxY);
            transform.position = new Vector3(newX, newY, -10f);

            //1.77777777778

        }
        zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

    void GetBounds()
    {
        Vector2 minBounds = new Vector2(CameraConfinerBox.transform.position.x - CameraConfinerBox.bounds.size.x / 2f, CameraConfinerBox.transform.position.y - CameraConfinerBox.bounds.size.y / 2f);
        Vector2 maxBounds = new Vector2(CameraConfinerBox.transform.position.x + CameraConfinerBox.bounds.size.x / 2f, CameraConfinerBox.transform.position.y + CameraConfinerBox.bounds.size.y / 2f);

        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        minX = minBounds.x + camWidth;
        maxX = maxBounds.x - camWidth;
        minY = minBounds.y + camHeight;
        maxY = maxBounds.y - camHeight;
    }

    //refresh x and y in case you zoom
    private void LateUpdate()
    {
        GetBounds();
    }
}


