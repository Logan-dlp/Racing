using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    Vector2 moveDirection;
    PlayerInput input;

    public float MaxSpeed = 5;
    float currentSpeed;
    float currentAcceleration;
    public float MaxRotationSpeed = 50;

    public CinemachineBrain Brain;
    public CinemachineVirtualCamera Camera1;
    public CinemachineVirtualCamera Camera2;

    private void Start()
    {
        input = GetComponent<PlayerInput>();

        InputAction move = input.actions["Move"];

        move.started += OnMoveStarted;
        move.performed += OnMovePerformed;
        move.canceled += OnMoveCanceled;

        InputAction camera = input.actions["ChangeCamera"];

        camera.performed += OnChangeCamera;
    }

    private void OnChangeCamera(InputAction.CallbackContext _obj)
    {
        var _currentCamera = Brain.ActiveVirtualCamera as CinemachineVirtualCamera;

        if (_currentCamera == Camera1)
        {
            Camera1.Priority = 0;
            Camera2.Priority = 10;
        }
        else
        {
            Camera1.Priority = 10;
            Camera2.Priority = 0;
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext _obj)
    {
        moveDirection = Vector2.zero;
    }

    private void Update()
    {
        float _rotation = moveDirection.x;
        float _acceleration = moveDirection.y;

        if (_acceleration == 0)
        {
            currentAcceleration = Mathf.Lerp(currentAcceleration, 0, Time.deltaTime);
        }else if (_acceleration < 0)
        {
            if (currentAcceleration > 0)
            {
                currentAcceleration -= Time.deltaTime;
            }
            else
            {
                currentAcceleration += _acceleration * Time.deltaTime;
            }
        }
        else
        {
            currentAcceleration += _acceleration * Time.deltaTime;
        }

        currentAcceleration = Mathf.Clamp(currentAcceleration, -1, 1);

        if (currentAcceleration >= 0)
        {
            currentSpeed = Mathf.Lerp(0, MaxSpeed, currentAcceleration);
        }
        else
        {
            currentSpeed = Mathf.Lerp(0, -MaxSpeed, -currentAcceleration);
        }

        _rotation = _rotation * currentAcceleration * MaxRotationSpeed * Time.deltaTime;
        
        transform.Rotate(0, _rotation, 0);
        transform.position = transform.position + transform.forward * (currentSpeed * Time.deltaTime);
    }

    private void OnMovePerformed(InputAction.CallbackContext _obj)
    {
        moveDirection = _obj.ReadValue<Vector2>();
    }

    private void OnMoveStarted(InputAction.CallbackContext _obj)
    {
        moveDirection = _obj.ReadValue<Vector2>();
    }
}
