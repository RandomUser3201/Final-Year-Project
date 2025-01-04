using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // [Movement and Camera]
    public float speed = 5f;
    public Transform cameraTransform;
    public float mouseSensitivity = 200f;
    public float distanceFromPlayer = 5f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    private Rigidbody rb;

    // [Game Win Management]
    public Transform goalPoint;
    public Text winMessage;
    private bool gameFinished = false;
    public Text timerText;
    public float totalTime = 60f;
    private float remainingTime;

    void Start()
    {
        // Lock cursor for better control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        winMessage.gameObject.SetActive(false);
        remainingTime = totalTime;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing on the Player.");
        }

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            if (cameraTransform == null)
            {
                Debug.LogError("No camera assigned, and no Main Camera found in the scene!");
            }
        }
    }

    void Update()
    {
        if (gameFinished)
        {
            return;
        }

        // Update timer
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else
        {
            remainingTime = 0;
            FinishGame("Time's Up!");
        }

        // Update the timer display in minutes and seconds
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Handle mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        Debug.Log($"MouseX: {mouseX}, MouseY: {mouseY}");

        // Update rotation values
        rotationY += mouseX;
        rotationX -= mouseY;

        // Clamped vertical rotation to prevent flipping
        rotationX = Mathf.Clamp(rotationX, -35f, 60f);

        // Apply rotation to the camera
        cameraTransform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // Updated the camera position relative to the player
        cameraTransform.position = transform.position - cameraTransform.forward * distanceFromPlayer + Vector3.up * 1.5f;

        // Detect when the player reaches the goal point
        if (Vector3.Distance(transform.position, goalPoint.position) < 5f)
        {
            FinishGame("You Win!");
        }
    }

    void FixedUpdate()
    {
        if (gameFinished) return;

        // Get input from WASD or arrow keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal == 0f && vertical == 0f)
            return;

        // Get the forward and right directions from the camera
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten directions on the horizontal plane
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction
        Vector3 movement = (cameraForward * vertical + cameraRight * horizontal).normalized;

        // Move the player
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

        // Rotate the player to face the movement direction
        if (movement.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }

        // Debug movement direction
        Debug.DrawRay(transform.position, movement * 5f, Color.green);
    }

    private void FinishGame(string message)
    {
        // Game End function
        gameFinished = true;
        winMessage.text = message;
        winMessage.gameObject.SetActive(true);
        Debug.Log(message);
    }
}

// References:
// [Camera]
// All Things Game Dev (2022). How To Make An FPS Player In Under A Minute - Unity Tutorial. [online] Available at: https://www.youtube.com/watch?v=qQLvcS9FxnY. [Accessed 3 Jan 2025].

