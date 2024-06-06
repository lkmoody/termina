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


    private List<Vector3> movePath;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerRigidbody = GetComponent<Rigidbody2D>();

        playerControls.MouseClick.Move.performed += context => LeftMouseClick(context);

        movePath = new List<Vector3>();
    }

    private void LeftMouseClick(InputAction.CallbackContext context)
    {
        Vector3 playerPosition = playerRigidbody.position;
        if(movePath.Count > 1) {
            Vector3 lastPoint = movePath[0];
            movePath.Clear();
            movePath.Add(lastPoint);
            playerPosition = lastPoint;
        }

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        List<Vector3> path = Pathfinding.Instance.FindPath(playerPosition, mousePosition);
        if(path != null) {
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
        Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
    }

    private void Move()
    {
        if (movePath.Count <= 0) return;

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
