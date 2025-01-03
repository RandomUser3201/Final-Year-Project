using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    // [References]
    public AudioSource audioSource;
    public AudioClip footsteps;
    private PlayerMovement playerMovement;
    private EnemyAI enemyAI;

    // [Speed Control]
    public float normalSpeed = 6f;
    public float sneakSpeed = 3f;

    // [Volume Control]
    public float volumeIncrease = 0.10f;
    public float volumeDecrease = 0.10f;
    public float maxVolume = 1f;

    private bool showDebugInfo;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        enemyAI = GameObject.Find("Enemy").GetComponent<EnemyAI>();
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            showDebugInfo = !showDebugInfo;
        }

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
        Debug.Log("Footsteps volume increasing: " + audioSource.volume);
    }

    void StopFootsteps()
    {
        // If player stops moving then volume is set to 0 and audio stops
        if (!IsPlayerMoving() && !enemyAI.playerInSightRange)
        {
            audioSource.Stop();
            audioSource.volume = 0f;
            Debug.Log("Footsteps stopped");
        }
    }

    void Sneak()
    {
        // Decreases volume while function is active
        audioSource.volume = Mathf.Clamp(audioSource.volume - (volumeDecrease * Time.deltaTime), 0, maxVolume);
        Debug.Log("Footsteps volume decreasing: " + audioSource.volume);

        // If audio is below 0, player is moving and audio is playing then stops the audio.
        if (audioSource.volume <= 0 && !IsPlayerMoving() && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("Sneaking has reduced volume to 0 - audio stopped");
        }

        // Reduces speed to sneakSpeed
        if (playerMovement != null)
        {
            playerMovement.speed = sneakSpeed;
        }
    }

    void RestoreSpeed()
    {
        // Restores speed from sneakSpeed to original
        if (playerMovement != null)
        {
            playerMovement.speed = normalSpeed;
        }
    }

    // Bool used to detect if movement input keys are entered.
    bool IsPlayerMoving()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }

    private void OnGUI()
    {
        if (showDebugInfo)
        {
            GUIStyle customStyle = new GUIStyle();
            customStyle.fontSize = 30;
            customStyle.normal.textColor = Color.red;

            // Display information on the screen
            GUI.Label(new Rect(10, 10, 500, 30), "Volume: [" + audioSource.volume + "]", customStyle);
            GUI.Label(new Rect(10, 40, 500, 30), "Player Speed: [" + playerMovement.speed + "]", customStyle);
            GUI.Label(new Rect(10, 70, 500, 30), "Player Sneaking: [" + Input.GetKey(KeyCode.LeftShift) + "]", customStyle);
            GUI.Label(new Rect(10, 100, 500, 30), "Player Moving: [" + IsPlayerMoving() + "]", customStyle);
            GUI.Label(new Rect(10, 130, 500, 30), "Player Health: [" + enemyAI.playerHealth + "]", customStyle);

            GUI.Label(new Rect(10, 180, 500, 30), "Enemy State: [" + enemyAI.GetCurrentState() + "]", customStyle);
        }
    }
}

/* References:
[Enemy AI & State Machines]
Code Monkey (2021). Simple Enemy AI in Unity (State Machine, Find Target, Chase, Attack). [online] Available at: https://www.youtube.com/watch?v=db0KWYaWfeM [Accessed 3 Jan 2025].

Dave / Game Development (2020). FULL 3D ENEMY AI in 6 MINUTES! || Unity Tutorial. [online] Available at: https://youtu.be/UjkSFoLxesw. [Accessed 3 Jan 2025].

git-amend (2023). EASY Unity Enemy AI using a State Machine. [online] YouTube. Available at: https://www.youtube.com/watch?v=eR-AGr5nKEU [Accessed 3 Jan. 2025].

This is GameDev (2023). How to code SMARTER A.I. enemies | Unity Tutorial. [online] YouTube. Available at: https://www.youtube.com/watch?v=rs7xUi9BqjE [Accessed 3 Jan 2025]. */