using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackBallCamera : MonoBehaviour
{

    public float distance = 100f;
    public float virtualTrackballDistance = 1f;
    public GameObject target;
    private Vector3 center;
    private Vector3? lastMousePosition;
    private Quaternion rotation;
    private Vector3 vecPos;
    private Vector3 lastPos;

    [HideInInspector]


    private void Start()
    {
        var terSize = target.GetComponent<Collider>().bounds.size;
        center = new Vector3(terSize.x / 2, terSize.y, terSize.z / 2) + target.transform.position;
        var startPos = (this.transform.position - center).normalized * distance;
        lastPos = startPos;
        var position = startPos + center;
        transform.position = position;
        transform.LookAt(center);
    }


    private void LateUpdate()
    {
        var mousePos = Input.mousePosition;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > 1)
        {
            // Scroll camera inwards
            distance--;
        }

        // Get mouse scrollwheel backwards || optional code
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            // Scrolling Backwards
            distance++;
        }

        if (Input.GetMouseButton(0))
        {
            if (lastMousePosition.HasValue)
            {
                // We are moving from here.
                lastPos = this.transform.position;
                rotation = FigureOutAxisAngleRotation(lastMousePosition.Value, mousePos);
            }
            lastMousePosition = mousePos;
        }
        else
        {
            lastMousePosition = null;
        }      
        vecPos = (center - lastPos).normalized * -distance;
        this.transform.position = rotation * vecPos + center;
        this.transform.LookAt(center);
    }

    private Quaternion FigureOutAxisAngleRotation(Vector3 lastMousePos, Vector3 mousePos)
    {
        if (lastMousePos.x == mousePos.x && lastMousePos.y == mousePos.y)
        {
            return Quaternion.identity;
        }

        var near = new Vector3(0, 0, Camera.main.nearClipPlane);
        Vector3 p1 = Camera.main.ScreenToWorldPoint(lastMousePos + near);
        Vector3 p2 = Camera.main.ScreenToWorldPoint(mousePos + near);

        //WriteLine("## {0} {1}", p1,p2);
        var axisOfRotation = Vector3.Cross(p2, p1);
        var twist = (p2 - p1).magnitude / (2.0f * virtualTrackballDistance);

        if (twist > 1.0f)
        {
            twist = 1.0f;
        }

        if (twist < -1.0f)
        {
            twist = -1.0f;
        }

        var phi = (2.0f * Mathf.Asin(twist)) * 180 / Mathf.PI;
        //WriteLine("AA: {0} angle: {1}",axisOfRotation, phi);

        return Quaternion.AngleAxis(phi, axisOfRotation);
    }
}

