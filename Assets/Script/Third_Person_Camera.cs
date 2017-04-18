using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Third_Person_Camera : MonoBehaviour {

    public Transform lookAt;
    public Transform camTransform;

    private Camera cam;

    private float distance = 40f;
    private float currentX = -3.0f;
    private float currentY =43.0f;


    // Use this for initialization
	void Start () {

        camTransform = transform;
        cam = Camera.main;

	}

    private void LateUpdate()
    {
        Vector3 dir = new Vector3((float)-1.3, (float)2.5,-distance);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        camTransform.position = lookAt.position + rotation * dir;

        camTransform.LookAt(lookAt.position);

    }
    // Update is called once per frame
    void Update () {

        /*currentX += Input.GetAxis("Mouse X");
        currentY += Input.GetAxis("Mouse Y");

        currentY =Mathf.Clamp(currentY,Y_ANGLE_MAX)
        */
    }
}
