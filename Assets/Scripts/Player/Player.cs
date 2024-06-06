using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public enum PlayerState
{
    Idle,
    Moving
}

public enum PlayerInputState
{
    Keyboard,
    Mouse
}

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private PlayerControls playerControls;

    [SerializeField]
    private PathfinderTest pathfinderTest;

    private Rigidbody2D playerRigidbody;
    private Vector3 mousePosition;


    private List<Vector3> movePath;

    private PlayerState currentState = PlayerState.Idle;
    private PlayerInputState currentInputState = PlayerInputState.Mouse;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerRigidbody = GetComponent<Rigidbody2D>();

        playerControls.MouseClick.Move.performed += context => LeftMouseClick(context);

        movePath = new List<Vector3>();
    }

    private void LeftMouseClick(InputAction.CallbackContext context)
    {
        currentInputState = PlayerInputState.Mouse;

        Vector3 playerPosition = playerRigidbody.position;
        if (movePath.Count > 1)
        {
            Vector3 lastPoint = movePath[0];
            movePath.Clear();
            movePath.Add(lastPoint);
            playerPosition = lastPoint;
        }

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        List<Vector3> path = Pathfinding.Instance.FindPath(playerPosition, mousePosition);
        if (path != null)
        {
            movePath.AddRange(path);
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void PlayerInput()
    {
        Vector2 wasdInput = playerControls.Movement.Move.ReadValue<Vector2>() * Pathfinding.Instance.GetGrid().GetCellSize();

        if(wasdInput != Vector2.zero) {
            Vector2 playerPosition = playerRigidbody.position;
            List<Vector3> path = new();

            if(currentInputState == PlayerInputState.Mouse) {
                if (movePath.Count > 1)
                {
                    playerPosition = movePath[0];
                    movePath.Clear();
                    path = Pathfinding.Instance.FindPath(playerPosition, playerPosition + wasdInput);
                    if(path != null) {
                        movePath.AddRange(path);
                    }
                }
            } else if(currentInputState == PlayerInputState.Keyboard && currentState != PlayerState.Moving) {
                Vector2 target = playerPosition + wasdInput;
                path = Pathfinding.Instance.FindPath(playerPosition, target);
                if(path != null) {
                    movePath.Clear();
                    movePath.AddRange(path);
                }
            }

            currentInputState = PlayerInputState.Keyboard;
        }
    }

    private void Move()
    {
        if (movePath.Count <= 0)
        {
            currentState = PlayerState.Idle;
            return;
        }
        else
        {
            currentState = PlayerState.Moving;
        }

        Vector3 currentPosition = playerRigidbody.position;

        if (currentPosition == movePath[0])
        {
            movePath.Remove(movePath[0]);
        }

        if (movePath.Count > 0)
        {
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, movePath[0], moveSpeed * Time.fixedDeltaTime);
            playerRigidbody.MovePosition(newPosition);
        }
    }
}
