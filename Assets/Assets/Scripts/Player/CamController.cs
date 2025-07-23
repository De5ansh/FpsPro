using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform playerTransform;
    public float senstivity = 2f;
    public float minXheight = -30f;
    public float maxXheight = 30f;
    public float minYheight = -360f;
    public float maxYheight = 360f;
    public float smoothSpd = 10f;
    private float rotationX = 0f;
    private float rotationY = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * senstivity;
        float mouseY = Input.GetAxis("Mouse Y") * senstivity;
        rotationX -= mouseY;
        rotationY += mouseX;
        rotationX = Mathf.Clamp(rotationX, minXheight, maxXheight);
        rotationY = Mathf.Clamp(rotationY, minYheight, maxYheight);
        Quaternion targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, smoothSpd * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpd * Time.deltaTime);
    }
}
