using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayerSound : MonoBehaviour
{
    public static PlayerSound Instance { get; private set; }
    
    [Header("References")]
    private EnemyAI _enemyAI;
    private ThirdPersonController _thirdPersonController;
    private HeartRateData _heartRateData;
    private PulseRateManager _pulseRateManager;

    [SerializeField] private EventReference _heartbeatRef;
    [SerializeField] private EventInstance _heartbeatInstance;
    
    [Header("Speed Control")]
    public float NormalSpeed = 6f;
    public float SneakSpeed = 3f;

    [Header("Volume Control")]
    
    [SerializeField] private float _volumeIncrease = 0.1f;
    [SerializeField] private float _volumeDecrease = 0.1f;
    [SerializeField] private float _maxVolume = 1f;

    [SerializeField] private float _time = 0f;
    [SerializeField] private float _rate = 0.45f;
    [SerializeField] public float CurrentVolume;
    [SerializeField] private bool _isHeartbeatPlaying = false;
    private bool _showDebugInfo;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _thirdPersonController = GetComponent<ThirdPersonController>();
        _enemyAI = FindObjectOfType<EnemyAI>();
        _heartRateData = FindObjectOfType<HeartRateData>();
        _pulseRateManager = FindObjectOfType<PulseRateManager>();
    }

    void Start()
    {
        if (!_isHeartbeatPlaying)
        {
            _heartbeatInstance = RuntimeManager.CreateInstance(_heartbeatRef);
            _heartbeatInstance.start();
            _isHeartbeatPlaying = true;
            Debug.LogWarning($"Start() isHeartbeatPlaying{_isHeartbeatPlaying}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _heartbeatInstance.setVolume(CurrentVolume);
        // Debug.LogError(_heartbeatInstance);
        // Debug.LogError($"Update() is heartbeatplaying {_isHeartbeatPlaying}");

        PLAYBACK_STATE playbackState;
        _heartbeatInstance.getPlaybackState(out playbackState);
        // Debug.LogWarning($"Playback State: {playbackState}");
        // Debug.LogWarning($"Out Current Vol: {_heartbeatInstance.getVolume(out CurrentVolume)}");

        // Debug.LogWarning($"Current volume: {CurrentVolume}");

        bool isSneaking = Input.GetKey(KeyCode.C);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _showDebugInfo = !_showDebugInfo;
        }

        // // Shift key activates sneak function
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
        if (_thirdPersonController.IsMoving == true && !isSneaking)
        {
            PlayHeartbeatSound();
        }
        else
        {
            StopHeartbeatSound();
        }

        if (_thirdPersonController.IsMoving == true)
        {
            _time += Time.deltaTime;
            //Debug.Log("Walking: " + isWalking);

            // Rate, stops the footstep sounds from playing all at once. Controls _time.
            if (_time >= _rate)
            {
                //Debug.Log("Time: " + _time + " Rate: " + rate);
                PlayFootsteps();
                _time = 0f;
            }
        }
    }

    void PlayHeartbeatSound()
    {
        // Increases volume while the function is active
        CurrentVolume = Mathf.Clamp(CurrentVolume + _volumeIncrease * Time.deltaTime, 0, _maxVolume);
        Debug.Log($"Footsteps volume increasing: {_heartbeatInstance}");

        if(!_isHeartbeatPlaying)
        {
            Debug.LogWarning("Heartbeat started");
        }
    }

    void StopHeartbeatSound()
    {
        // If player stops moving then volume is set to 0 and audio stops
        if (!_thirdPersonController.IsMoving && !_enemyAI.IsPlayerInSightRange)
        {
            _isHeartbeatPlaying = false;
            //_heartbeatInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            //_heartbeatInstance.setVolume(0);
            Debug.Log("Footsteps stopped");
        }
    }

    void Sneak()
    {
        // Decreases volume while function is active
        CurrentVolume = Mathf.Clamp(CurrentVolume - (_volumeDecrease * Time.deltaTime), 0, _maxVolume);
        Debug.Log($"Footsteps volume decreasing: {CurrentVolume}");

        // If audio is below 0, player is moving and audio is playing then stops the audio.
        if (CurrentVolume <= 0 && !_thirdPersonController.IsMoving && _isHeartbeatPlaying)
        {
            // _heartbeatInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Debug.Log("Sneaking has reduced volume to 0 - audio stopped");
        }

        // Reduces speed to SneakSpeed
        _thirdPersonController.MoveSpeed = SneakSpeed;
    }

    void RestoreSpeed()
    {
        // Restores speed from SneakSpeed to original
        if (_thirdPersonController != null)
        {
            _thirdPersonController.MoveSpeed = NormalSpeed;
        }
    }

    public void PlayFootsteps()
    {
        RuntimeManager.PlayOneShot("event:/Footsteps");
    }

    private void OnGUI()
    {
        if (_showDebugInfo)
        {
            GUIStyle customStyle = new GUIStyle();
            customStyle.fontSize = 30;
            customStyle.normal.textColor = Color.white;

            // Display information on the screen
            GUI.Label(new Rect(10, 10, 500, 30), $"Volume: [{CurrentVolume}]", customStyle);
            GUI.Label(new Rect(10, 40, 500, 30), $"Player Speed: [{_thirdPersonController.MoveSpeed}]", customStyle);
            GUI.Label(new Rect(10, 70, 500, 30), $"Player Sneaking: [{Input.GetKey(KeyCode.C)}]", customStyle);
            GUI.Label(new Rect(10, 100, 500, 30), $"Player Moving:  [{_thirdPersonController.IsMoving}]", customStyle);
            GUI.Label(new Rect(10, 130, 500, 30), $"Player Health: [{_enemyAI.PlayerHealth}]", customStyle);

            GUI.Label(new Rect(10, 180, 500, 30), $"Enemy State: [{_enemyAI.GetCurrentState()}]", customStyle);
            GUI.Label(new Rect(10, 210, 500, 30), $"PlayerAge: [{_heartRateData.PlayerAge}] ({_heartRateData.MinHR}, {_heartRateData.MaxHR})", customStyle);
            GUI.Label(new Rect(10, 240, 500, 30), $"Current Heart Rate: [{_pulseRateManager.heartRate}]", customStyle);
        }
    }

}

/* References:
[Enemy AI & State Machines]
Code Monkey (2021). Simple Enemy AI in Unity (State Machine, Find Target, Chase, Attack). [online] Available at: https://www.youtube.com/watch?v=db0KWYaWfeM [Accessed 3 Jan 2025].

Dave / Game Development (2020). FULL 3D ENEMY AI in 6 MINUTES! || Unity Tutorial. [online] Available at: https://youtu.be/UjkSFoLxesw. [Accessed 3 Jan 2025].

git-amend (2023). EASY Unity Enemy AI using a State Machine. [online] YouTube. Available at: https://www.youtube.com/watch?v=eR-AGr5nKEU [Accessed 3 Jan. 2025].

This is GameDev (2023). How to code SMARTER A.I. enemies | Unity Tutorial. [online] YouTube. Available at: https://www.youtube.com/watch?v=rs7xUi9BqjE [Accessed 3 Jan 2025]. */