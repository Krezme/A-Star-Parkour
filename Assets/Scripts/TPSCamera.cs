﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCamera : MonoBehaviour {

    public bool lockCursor;
    public float mouseSensitivity = 10;
    public Transform Player;
    public float DistanceFromPlayer = 4;
    public Vector2 MIN_MAX_ANGLE = new Vector2(-40, 85);

    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float MouseX;
    [HideInInspector] public float MouseY;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        MouseX += Input.GetAxis("Mouse X") * mouseSensitivity;

        MouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        MouseY = Mathf.Clamp(MouseY, MIN_MAX_ANGLE.x, MIN_MAX_ANGLE.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(MouseY, MouseX), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;
        transform.position = Player.position - transform.forward * DistanceFromPlayer;
    }
}
