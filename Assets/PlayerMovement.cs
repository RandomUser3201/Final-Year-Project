using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    // [Movement]
    public float speed = 5f;
    public Transform cameraTransform;
    public float mouseSensitivity = 100f;
    public float distanceFromPlayer = 5f;

    // [Camera]
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    // [Game Win Mangement]
    public float timer = 30f;
    public Transform goalPoint;
    public float debugLineLength = 10f;
    public Text winMessage;
    private bool gameFinished = false;
    public Text timerText;
    public float totalTime = 60f;
    private float remainingTime;

    void Start()
    {
        // Lock the cursor for better camera control.
        Cursor.lockState = CursorLockMode.Locked;

        winMessage.gameObject.SetActive(false);
        remainingTime = totalTime;
    }

    void Update()
    {
        if (gameFinished)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            Debug.Log("Time's up!"); 
        }

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

        // Handle horizontal and vertical movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        // Handle camera rotation with mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -35f, 60f);

        // Update the camera's position and rotation
        cameraTransform.position = transform.position - cameraTransform.forward * distanceFromPlayer;
        cameraTransform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // Draw a debug line to visualize the forward direction
        cameraTransform.position = transform.position - cameraTransform.forward * distanceFromPlayer;

        // Draw a debug line to visualize the forward direction
        Debug.DrawRay(transform.position, transform.forward * debugLineLength, Color.red);

        // Detect when the player reaches the goal point.
        if (Vector3.Distance(transform.position, goalPoint.position) < 1f)
        {
            FinishGame("You Win!");
        }
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



