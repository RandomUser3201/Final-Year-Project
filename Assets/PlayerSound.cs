using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    // [References]
    public AudioSource audioSource; 
    public AudioClip footsteps;
    private PlayerMovement playerMovement;

    // [Speed Control]
    public float normalSpeed = 6f;
    public float sneakSpeed = 3f;

    // [Volume Control]
    public float volumeIncrease = 0.10f;
    public float volumeDecrease = 0.10f;
    public float maxVolume = 1f;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        audioSource.volume = 0f;

        if (playerMovement == null)
        {
            Debug.Log("Movement script not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isSneaking = Input.GetKey(KeyCode.LeftShift);

        // Display volume, speed and sneaking information for debuggging.
        Debug.LogWarning("Volume: [" + audioSource.volume + "] Player Speed: [" + playerMovement.speed + "] Sneaking [" + isSneaking + "]");

        // Shift key activates sneak function 
        if (isSneaking)
        {
            Sneak();
            Debug.Log("Sneak Mode Activated");
        }
        else
        {
            RestoreSpeed();
        }

        // Begins footstep logic if player is moving and not sneaking. 
        if (IsPlayerMoving() && !isSneaking)
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
        // If there is no audio playing, plays footstep sound, and loops.
        if (!audioSource.isPlaying)
        {
            audioSource.clip = footsteps;
            audioSource.loop = true;
            audioSource.Play();
            Debug.Log("Footsteps now playing");
        }

        // Increases volume while the function is active
        audioSource.volume = Mathf.Clamp(audioSource.volume + volumeIncrease * Time.deltaTime, 0, maxVolume);
        Debug.LogWarning("Footsteps volume increasing: " + audioSource.volume);
    }

    void StopFootsteps()
    {
        // If player stops moving then volume is set to 0 and audio stops
        if (!IsPlayerMoving())
        {
            audioSource.Stop();
            audioSource.volume = 0f;
            Debug.LogWarning("Footsteps stopped");
        }
    }

    void Sneak()
    {
        // Decreases volume while function is active
        audioSource.volume = Mathf.Clamp(audioSource.volume - (volumeDecrease * Time.deltaTime), 0, maxVolume);
        Debug.LogWarning("Footsteps volume decreasing: " + audioSource.volume);

        // If audio is below 0, player is moving and audio is playing then stops the audio.
        if (audioSource.volume <= 0 && !IsPlayerMoving() && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.LogWarning("Sneaking has reduced volume to 0 - audio stopped");
        }

        // Reduces speed to sneakSpeed
        if (playerMovement != null)
        {
            playerMovement.speed = sneakSpeed;
        }
        Debug.Log("Sneak Mode Active: Speed reduced");
    }

    void RestoreSpeed()
    {
        // Restores speed from sneakSpeed to original
        if (playerMovement != null)
        {
            playerMovement.speed = normalSpeed;
        }

        Debug.Log("Speed Restored");
    }

    // Bool used to detect if movement input keys are entered.
    bool IsPlayerMoving()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }
}
