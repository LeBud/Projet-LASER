using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Hoverboard hb;

    [Header("Settings")]
    [SerializeField] float mouseSensibility = 1;
    [SerializeField] float camDistance = 5;
    [SerializeField] float camMaxDistance = 15;

    [Header("ObjectRef")]
    public Camera cam;
    [SerializeField] Transform camPos;
    [SerializeField] Transform camFollowPoint;
    [SerializeField] Transform hoverBoard;

    float mouseSensMult = .0015f;
    float mouseX, mouseY;
    [HideInInspector]
    public float xRotation, yRotation;

    float actualDistance;

    private void Start()
    {
        actualDistance = camDistance;
        hb = GetComponent<Hoverboard>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MyInputs();
    }

    private void LateUpdate()
    {
        camFollowPoint.position = hoverBoard.position;

        camPos.position = camFollowPoint.position - camFollowPoint.forward * actualDistance;
        camFollowPoint.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

        cam.transform.position = camPos.position;
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        actualDistance = Mathf.SmoothStep(actualDistance, camDistance + hb.rb.velocity.magnitude * .1f, 10 * Time.deltaTime);
        if (actualDistance > camMaxDistance) actualDistance = camMaxDistance;
    }

    void MyInputs()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * mouseSensibility * mouseSensMult;
        xRotation -= mouseY * mouseSensibility * mouseSensMult;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
    }
}
