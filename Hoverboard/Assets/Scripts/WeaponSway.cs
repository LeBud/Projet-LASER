using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    public static WeaponSway Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] float amount = .1f;
    [SerializeField] float maxAmount = .3f;
    [SerializeField] float smoothAmount = 4;

    [Space]
    [SerializeField] float rotAmount = 2;
    [SerializeField] float maxRotAmount = 15;
    [SerializeField] float smoothRot = 5;

    float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }
    [Header("Bobbing")]
    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.015f;
    Vector3 bobPos;

    [Header("Bob Rot")]
    public Vector3 multplier;
    Vector3 bobEulerRot;

    Vector3 initPosition;
    Quaternion initRotation;

    Vector3 newCalculatePos;
    Quaternion newCalculateRot;

    Vector3 dashPos;

    float inputX;
    float inputY;
    float horImput;
    float verImput;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        initPosition = transform.localPosition;
        initRotation = transform.localRotation;
    }

    private void Update()
    {
        MyInputs();
        SwayAndBobbing();
    }

    private void LateUpdate()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, newCalculatePos, Time.deltaTime * smoothAmount);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, newCalculateRot, Time.deltaTime * smoothRot);
    }

    void MyInputs()
    {
        inputX = InputsBrain.Instance.mouse.ReadValue<Vector2>().x;
        inputY = -InputsBrain.Instance.mouse.ReadValue<Vector2>().y;

        horImput = -InputsBrain.Instance.move.ReadValue<Vector2>().x;
        verImput = -InputsBrain.Instance.move.ReadValue<Vector2>().y;
    }

    void SwayAndBobbing()
    {
        //sway && bob move position
        float moveX = Mathf.Clamp(-inputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(inputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        float moveXInput = Mathf.Clamp(horImput * amount, -maxAmount, maxAmount);
        float moveZInput = Mathf.Clamp(verImput * amount, -maxAmount, maxAmount);
        float moveYInput = Mathf.Clamp(Mathf.Clamp(-PlayerController.Instance.rb.velocity.y, -1, 1) * amount, -maxAmount, maxAmount);

        Vector3 finalPositionInput = new Vector3(moveXInput, moveYInput, moveZInput);

        bobPos.x = (curveCos * bobLimit.x * (PlayerController.Instance.IsGrounded() ? 1 : 0)) - (horImput * travelLimit.x);
        bobPos.y = (curveSin * bobLimit.y) - Mathf.Clamp(PlayerController.Instance.rb.velocity.y, -1, 1) * travelLimit.y;
        bobPos.z = -(verImput * travelLimit.z);

        newCalculatePos = finalPosition + finalPositionInput + bobPos + initPosition + dashPos;

        //Sway && bob rotation
        float tiltX = Mathf.Clamp(inputY * rotAmount, -maxRotAmount, maxRotAmount);
        float tiltY = Mathf.Clamp(inputX * rotAmount, -maxRotAmount, maxRotAmount);

        Quaternion finalRotation = Quaternion.Euler(new Vector3(-tiltX, 0f, tiltY));

        speedCurve += Time.deltaTime * (PlayerController.Instance.IsGrounded() ? PlayerController.Instance.rb.velocity.magnitude / 2 : 1f) + 0.01f;

        bobEulerRot.x = InputsBrain.Instance.move.ReadValue<Vector2>() != Vector2.zero ? multplier.x * Mathf.Sin(2 * speedCurve) : multplier.x * (Mathf.Sin(2 * speedCurve) / 2);
        bobEulerRot.y = InputsBrain.Instance.move.ReadValue<Vector2>() != Vector2.zero ? multplier.y * curveCos : 0;
        bobEulerRot.z = InputsBrain.Instance.move.ReadValue<Vector2>() != Vector2.zero ? multplier.z * curveCos * horImput : 0;

        newCalculateRot = finalRotation * Quaternion.Euler(bobEulerRot) * initRotation;
    }

    

}
