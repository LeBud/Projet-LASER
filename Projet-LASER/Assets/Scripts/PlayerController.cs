using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [HideInInspector]
    public Rigidbody rb;

    [Header("Player Camera")]
    [SerializeField] float mouseSensibility = 1;
    public Transform playerCamera;
    [SerializeField] Transform cameraPos;
    public Transform playerDir;
    public float camTilt = 1.6f;
    public float camTiltTime = 5f;
    public float tilt { get; private set; }

    [Header("Player Movement On Ground")]
    [SerializeField] float playerSpeed;
    [SerializeField] float movementMult;
    [SerializeField] float groundDrag;
    [SerializeField] float jumpForce;

    [Header("Player Movement In Air")]
    [SerializeField] float playerAirSpeed;
    [SerializeField] float airMovementMult;
    [SerializeField] float airDrag;

    [Header("Player Jump Settings")]
    [SerializeField] float jumpBufferTime;
    [SerializeField] float coyoteTime;

    [Header("Fall Settings")]
    [SerializeField] float maxFallSpeed = 40;
    [SerializeField] float fallSpeedAccel = 35;

    [Header("Ground Settings")]
    [SerializeField] Transform feetPos;
    [SerializeField] Vector3 feetSize;
    [SerializeField] LayerMask groundLayer;

    [Header("Slope Settings")]
    [SerializeField] float downForceOnSlope = 80;
    [SerializeField] float maxSlopeAngle = 45;

    [Header("Other Settings")]
    [SerializeField] float playerHeight;


    //Cam Float
    float mouseSensMult = .0015f;
    float mouseX;
    float mouseY;
    float xRotation;
    float yRotation;

    float onGround;
    float lastJumpPressed;
    float fallSpeed;

    bool canJump = true;
    bool isSlaming = false;

    Vector2 inputsValue = new Vector2();

    [HideInInspector]
    public Vector3 playerMoveDir = new Vector3();
    Vector3 playerSlopeMoveDir = new Vector3();

    [HideInInspector]
    public RaycastHit slopeHit;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (PauseMenu.IsPaused) return;

        MyInputs();
        CheckMethods();
        ControlDrag();
    }

    void LateUpdate()
    {
        playerDir.rotation = Quaternion.Euler(0, yRotation, 0);
        playerCamera.localRotation = Quaternion.Euler(xRotation, yRotation, tilt);
    }

    void CheckMethods()
    {
        onGround -= Time.deltaTime;
        lastJumpPressed -= Time.deltaTime;

        if (IsGrounded())
        {
            canJump = true;
            onGround = coyoteTime;
            fallSpeed = 0;
        }
        else if (!IsGrounded())
        {
            fallSpeed = Mathf.SmoothStep(fallSpeed, maxFallSpeed, fallSpeedAccel * Time.deltaTime);
        }

        rb.useGravity = !OnSlope();

        playerMoveDir = playerDir.forward * inputsValue.y + playerDir.right * inputsValue.x;
        playerSlopeMoveDir = Vector3.ProjectOnPlane(playerMoveDir, slopeHit.normal);

        if (lastJumpPressed > 0 && onGround > 0 && canJump)
            Jump();
    }

    void MyInputs()
    {
        //Get movement inputs values (WASD)
        inputsValue = InputsBrain.Instance.move.ReadValue<Vector2>();

        //Camera Inputs
        mouseX = InputsBrain.Instance.mouse.ReadValue<Vector2>().x;
        mouseY = InputsBrain.Instance.mouse.ReadValue<Vector2>().y;

        yRotation += mouseX * mouseSensibility * mouseSensMult;
        xRotation -= mouseY * mouseSensibility * mouseSensMult;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Camera Tilt
        if (inputsValue.x < 0)
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
        else if (inputsValue.x > 0)
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else
            tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);

        //Jump input
        if (InputsBrain.Instance.jump.WasPressedThisFrame())
            lastJumpPressed = jumpBufferTime;
    }

    void Jump()
    {
        canJump = false;
        onGround = 0;
        lastJumpPressed = 0;

        float force = jumpForce;

        if(rb.linearVelocity.y < 0)
            force -= rb.linearVelocity.y;
        
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (!IsGrounded())
            rb.AddForce(Vector3.down * fallSpeed, ForceMode.Acceleration);
    }

    void ControlDrag()
    {
        if (IsGrounded())
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = airDrag;
    }

    void MovePlayer()
    {
        if (!IsGrounded())
            rb.AddForce(playerMoveDir.normalized * playerAirSpeed * movementMult * airMovementMult, ForceMode.Acceleration);
        else if (IsGrounded() && !OnSlope())
            rb.AddForce(playerMoveDir.normalized * playerSpeed * movementMult, ForceMode.Acceleration);
        else if (IsGrounded() && OnSlope())
            rb.AddForce(playerSlopeMoveDir.normalized * playerSpeed * movementMult, ForceMode.Acceleration);
        
    }

    public bool IsGrounded()
    {
        float angle = 0;
        if (slopeHit.normal != Vector3.up)
            angle = Vector3.Angle(Vector3.up, slopeHit.normal);

        return Physics.CheckBox(feetPos.position, feetSize, Quaternion.identity, groundLayer) && angle < maxSlopeAngle;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }
            else
                return false;
        }
        return false;
    }
}