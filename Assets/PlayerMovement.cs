using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{

    public float speed = 5f;  
    public Transform cameraTransform;  
    public float mouseSensitivity = 100f;  
    public float distanceFromPlayer = 5f;  

    private float rotationX = 0f;  
    private float rotationY = 0f;

    public float debugLineLength = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -35f, 60f);

        cameraTransform.position = transform.position - cameraTransform.forward * distanceFromPlayer;
        cameraTransform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);

        cameraTransform.position = transform.position - cameraTransform.forward * distanceFromPlayer;

        Debug.Log("drag rayline");
        Debug.DrawRay(transform.position, transform.forward * debugLineLength, Color.red);

    }
}



