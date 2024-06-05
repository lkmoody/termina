using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private PlayerControls playerControls;

    [SerializeField]
    private PathfinderTest pathfinderTest;

    private Rigidbody2D playerRigidbody;
    private Vector2 movement;
    private Vector3 mousePosition;


    private List<Vector2> movePath;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerRigidbody = GetComponent<Rigidbody2D>();

        playerControls.MouseClick.Move.performed += context => LeftMouseClick(context);

        movePath = new List<Vector2>();
    }

    private void LeftMouseClick(InputAction.CallbackContext context)
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(playerRigidbody.position);
        List<Vector2> path = pathfinderTest.GetWalkingPath(playerRigidbody.position, mousePosition);
        if (path != null)
        {
            movePath.AddRange(path);
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        MovePathTest();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        //movement = playerControls.Movement.MouseMove.ReadValue<Vector2>();
    }

    private void Move()
    {
        playerRigidbody.MovePosition(playerRigidbody.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void MoveMouse()
    {
        Vector2 newPosition = Vector2.MoveTowards(playerRigidbody.position, mousePosition, moveSpeed * Time.fixedDeltaTime);
        playerRigidbody.MovePosition(newPosition);
    }

    private void MovePathTest()
    {
        if (movePath.Count <= 0) return;

        Vector2 currentPosition = playerRigidbody.position;

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
