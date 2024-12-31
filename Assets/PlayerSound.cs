using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip footsteps;

    private PlayerMovement playerMovement;

    public float normalSpeed = 6f;
    public float sneakSpeed = 3f;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.Log("Movement script not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isSneaking = Input.GetKey(KeyCode.LeftShift);

        // If any of movement keys and shift (sneak)
        if (isSneaking)
        {
            Sneak();
            Debug.Log("Sneak Mode Activated");
        }
        else
        {
            RestoreSpeed();
        }

        if (!isSneaking && IsPlayerMoving())
        {
            Footsteps();
        }
        else
        {
            StopFootsteps();
        }
    }

    void Footsteps()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = footsteps;
            audioSource.volume = 1f;
            audioSource.loop = true;
            audioSource.Play();
            Debug.Log("Footsteps now playing");
        }
    }

    void StopFootsteps()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Footsteps stopped");
        }
    }

    void Sneak() 
    {
        //audioSource.volume = 0f;
        //audioSource.Play();

        //playerMovement.speed = 100f;

        if (playerMovement != null)
        {
            playerMovement.speed = sneakSpeed;
        }
        Debug.Log("Sneak Mode Active: Speed reduced");
    }

    void RestoreSpeed()
    {
        if(playerMovement != null)
        {
            playerMovement.speed = normalSpeed;
        }

        Debug.Log("Speed Restored");
    }

    bool IsPlayerMoving()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }

}
