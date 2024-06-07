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
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private PlayerControls playerControls;

    private Rigidbody2D playerRigidbody;
    private Vector3 mousePosition;
    // A list of points on the grid that the player moves along
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

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void LeftMouseClick(InputAction.CallbackContext context)
    {
        Vector3 playerPosition = playerRigidbody.position;
        // If the player is already moving we remove the rest of the path and let them reach the center of the grid they were moving towards.
        if (movePath.Count > 0)
        {
            movePath.RemoveRange(1, movePath.Count - 1);
            playerPosition = movePath[0];
        }

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        List<Vector3> path = Pathfinding.Instance.FindPath(playerPosition, mousePosition);
        if (path != null)
        {
            movePath.AddRange(path);
        }

        currentInputState = PlayerInputState.Mouse;
    }

    private void PlayerInput()
    {
        Vector2 direction = GetKeyPressDirection();

        direction *= Pathfinding.Instance.GetGrid().GetCellSize();

        if (direction != Vector2.zero)
        {
            if (currentInputState == PlayerInputState.Mouse)
            {
                if (movePath.Count > 1)
                { 
                    Vector2 target = (Vector2)movePath[0] + direction;
                    if (Pathfinding.Instance.GetGrid().GetGridObject(target).isWalkable)
                    {
                        movePath.Clear();
                        movePath.Add(target);
                    }
                }
            }
            else if (currentInputState == PlayerInputState.Keyboard && currentState != PlayerState.Moving)
            {
                Vector2 target = playerRigidbody.position + direction;
                if (Pathfinding.Instance.GetGrid().GetGridObject(target).isWalkable)
                {
                    movePath.Clear();
                    movePath.Add(target);
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

    private Vector2 GetKeyPressDirection() {
        Vector2 direction = Vector2.zero;
        if(playerControls.GridMovement.North.ReadValue<float>() > 0) {
            direction += Vector2.up;
        }

        if(playerControls.GridMovement.East.ReadValue<float>() > 0) {
            direction += Vector2.right;
        }

        if(playerControls.GridMovement.South.ReadValue<float>() > 0) {
            direction += Vector2.down;
        }

        if(playerControls.GridMovement.West.ReadValue<float>() > 0) {
            direction += Vector2.left;
        }

        return direction;
    }
}
