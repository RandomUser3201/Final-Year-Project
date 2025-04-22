using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

public class PulseRateManager : MonoBehaviour
{
    SerialPort serialPort;
    public string portName = "COM5";
    public int baudRate = 9600;
    public int heartRate = 0;
    public Text bpmText;

    public bool IsVisible = true;
    private EnemyAI _enemyAI;
    private float _threshold = 0f;

    private string _filePath;
    private float _lastUpdateTime = 0f;
    private float _updateInterval = 5f;
    [SerializeField] private int _minBufferZone = 15;
    [SerializeField] private int _maxBufferZone = 30;
    private bool _sensorStabilized = false;
    void Start()
    {
        _enemyAI = FindObjectOfType<EnemyAI>();
        
        if (_enemyAI == null)
        {
            Debug.LogWarning("Enemy AI null");
        }

        try
        {
            // Checks to see if the Arduino device is connected.
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 500;
            Debug.Log("Serial Port Opened");
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not open serial port: {e.Message}");
        }

        // Creates headings in the .csv file - ready for the data
        _filePath = Application.persistentDataPath + "/HeartRateLog.csv";
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "Timestamp,HeartRate,EnemyState\n");
        }
        Debug.LogError($"Persistent data path: {Application.persistentDataPath}");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _lastUpdateTime >= _updateInterval)
        {

            _lastUpdateTime = Time.time;

            if (serialPort != null && serialPort.IsOpen)
            {
                string data = serialPort.ReadLine().Trim();
                int _parsedHeartRate;
                // heartRate = int.Parse(data);
                Debug.Log($"Heart Rate Outside Parse: {heartRate}");
                
                if (int.TryParse(data, out _parsedHeartRate))
                {
                    // Obtains  the Minimum and Maximum Heart Rate according to data in dictionary
                    int MinHR = HeartRateData.Instance.MinHR;
                    int MaxHR = HeartRateData.Instance.MaxHR;

                    // Checks if the heart rate is outside of the max and min heart rate 
                    // This stops any innacurate readings from being processed
                    if (_parsedHeartRate >= MinHR && _parsedHeartRate <= MaxHR)
                    {
                        heartRate = _parsedHeartRate;
                        _sensorStabilized = true;

                        // Allows for some leeway with regards to the MinHR & MaxHR 
                        if (heartRate < MinHR - 10 || heartRate > MaxHR + 10)
                        {
                            // Only registers the heartRate for functions if met above conditions
                            // Decides whether or not the heart rate makes the player visible or not
                            SaveHeartRateData(heartRate);

                            Debug.LogWarning($"Heart rate {heartRate} outside normal range ({MinHR} {MaxHR}) for  {HeartRateData.Instance.PlayerAge} year old");
                        }
                    }

                    Debug.Log($"Heart Rate - Try Parse:  {heartRate}");
                }
                
                else
                {
                    Debug.LogError($"Invalid heart rate data received: {data}");
                }
            }       

        }

        //-- Quick Testing --
        // if (Input.GetKey(KeyCode.E))
        // {
        //     heartRate = 210;
        //     _sensorStabilized = true;
        //     Debug.Log($"Current HR: {heartRate} - Test High");
        // }
        // if (Input.GetKey(KeyCode.Z))
        // {
        //     heartRate = 137;
        //     _sensorStabilized = true;
        //     Debug.Log($"Current HR: {heartRate} - Test Mid");
        // }
        // if (Input.GetKey(KeyCode.Q))
        // {
        //     heartRate = 110;
        //     _sensorStabilized = true;
        //     Debug.Log($"Current HR: {heartRate} - Test Low");
        // }

        if (_sensorStabilized)
        {
            ToggleVisibility();
        }
        
        bpmText.text = "BPM: " + heartRate;
    }

    void SaveHeartRateData(int heartRate)
    {
        // Exports the player data under the correct headings to .csv file
        // Timestamp / HeartRate / Enemy Current State
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"{timestamp},{heartRate},{_enemyAI.GetCurrentState()}\n";
        Debug.Log($"Saving heart rate data: {logEntry}"); 

        File.AppendAllText(_filePath, logEntry);
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial Port Closed");
        }
    }

    public void ToggleVisibility()
    {
        // Default values
        float tsightRange = 24f;
        float tsoundRange = 40f;

        // Obtains min and max HR, based on players age, from the table.
        int MinHR = HeartRateData.Instance.MinHR;
        int MaxHR = HeartRateData.Instance.MaxHR;

        if (heartRate < MinHR + _minBufferZone)
        {
            IsVisible = false;
            Debug.LogWarning($"Low Heart Rate - Visibility: {IsVisible}");
        }

        else if (heartRate > MaxHR - _maxBufferZone)
        {
            // If heart rate is within buffer zone, player is visible
            tsightRange *= 3f;
            tsoundRange *= 3f;

            // If player is within the buffer for 6 seconds, the enemy speed will increase
            _threshold += Time.deltaTime;

            if (_threshold >= 6f)
            {
                _enemyAI.Agent.speed = 10;
                _threshold = 0f;
                Debug.LogWarning($"High Heart Rate (For 6 Seconds) - Enemy Speed: {_enemyAI.Agent.speed}");
            }

            IsVisible = true;
            Debug.LogWarning($"High Heart Rate - Visibility: {IsVisible}");
        }
        else
        {
            // Set default ranges if its neither next to min or max HR - in the middle
            tsightRange = 24f;
            tsoundRange = 40f; 
            IsVisible = true;
            Debug.LogWarning($"Normal Heart Rate - Visibility: {IsVisible}");
        }

        if (IsVisible == true)
        {   
            // If player is visible - sight & sound range, change to new value gradually
            _enemyAI.SightRange = Mathf.Lerp(_enemyAI.SightRange, tsightRange, Time.deltaTime * 0.5f);
            _enemyAI.SoundRange = Mathf.Lerp(_enemyAI.SoundRange, tsoundRange, Time.deltaTime * 0.5f);
            Debug.LogWarning($"Heart Rate Within Max Buffer Zone - Visibility: {IsVisible} - Sound/Sight Range: {tsightRange}, {tsoundRange}");
        }

        // If player is invisible - sight & sound range, change to new value rapidly
        else if (IsVisible == false)
        {
            // If heart rate is within the buffer zone, player is invisible (almost)
            tsightRange = 3f; 
            tsoundRange = 3f;
            _enemyAI.SightRange = Mathf.Lerp(_enemyAI.SightRange, tsightRange, Time.deltaTime * 10f);
            _enemyAI.SoundRange = Mathf.Lerp(_enemyAI.SoundRange, tsoundRange, Time.deltaTime * 10f);
            Debug.LogWarning($"Heart Rate Within Min Buffer Zone - Visibility: {IsVisible} - Sound/Sight Range: {tsightRange}, {tsoundRange}");
        }
    }

}

// batts (2022) Updating the Unity_SerialPort Script [online] Available at: https://dyadica.co.uk/blog/updating-the-unity_serialport-script/ [Accessed 22 Mar 2025]
// Cleveland Clinic (2024) Heart Rate [online] Available at: https://my.clevelandclinic.org/health/diagnostics/heart-rate [Accessed 20 Mar 2025]