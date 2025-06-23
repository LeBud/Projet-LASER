using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverboard : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    public float height;
    public float groundHeight = 2;

    public float moveForce;
    public float turnSpeed;
    public float dashForce;
    public float jumpForce;

    public float gravity;

    public Transform[] anchors = new Transform[4];
    RaycastHit[] hits = new RaycastHit[4];

    Vector2 inputs;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        inputs.y = Input.GetAxisRaw("Vertical");
        inputs.x = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonDown("Fire1"))
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        if (Input.GetButtonDown("Fire3"))
            rb.AddForce(transform.forward * dashForce, ForceMode.VelocityChange);

        if(Input.GetButtonDown("Jump") && Grounded())
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        Debug.Log("Is grounded + " + Grounded());
    }

    private void FixedUpdate()
    {
        if (!Grounded())
            rb.AddForce(Vector3.down * gravity, ForceMode.Force);

        DoRaycast();
        Move();
    }

    void Move()
    {
        rb.AddForce(transform.forward * inputs.y * moveForce, ForceMode.Acceleration);
        rb.AddTorque(transform.up * inputs.x * turnSpeed, ForceMode.Acceleration);
    }

    void DoRaycast()
    {
        for (int i = 0; i < hits.Length; i++)
            ApplyForce(anchors[i], hits[i]);
    }

    void ApplyForce(Transform anchor, RaycastHit hit)
    {
        if (Physics.Raycast(anchor.position, -anchor.up, out hit, groundHeight))
        {
            float force = 0;
            force = Mathf.Abs(1 / (hit.point.y - anchor.position.y));
            rb.AddForceAtPosition(transform.up * force * height, anchor.position, ForceMode.Acceleration);
        }
    }

    bool Grounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, groundHeight);
        return hit.collider == null ? false : true;
    }
}