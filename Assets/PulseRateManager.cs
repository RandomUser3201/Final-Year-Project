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

    void Start()
    {
        _enemyAI = FindObjectOfType<EnemyAI>();
        if (_enemyAI == null)
        {
            Debug.LogWarning("Enemy AI null");
        }
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 500;
            Debug.Log("Serial Port Opened");
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not open serial port: {e.Message}");
        }

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
                int parsedHeartRate;
                // heartRate = int.Parse(data);
                Debug.Log($"Heart Rate Outside Parse: {heartRate}");
                
                if (int.TryParse(data, out parsedHeartRate))
                {
                    heartRate = parsedHeartRate;

                    int MinHR = HeartRateData.Instance.MinHR;
                    int MaxHR = HeartRateData.Instance.MaxHR;

                    if (heartRate < MinHR || heartRate > MaxHR)
                    {
                        Debug.LogWarning($"Heart rate {heartRate} outside normal range ({MinHR} {MaxHR}) for  {HeartRateData.Instance.PlayerAge} year old");
                    }

                    Debug.Log($"Heart Rate - Try Parse:  {heartRate}");
                }
                
                else
                {
                    Debug.LogError($"Invalid heart rate data received: {data}");
                }
            }       

            SaveHeartRateData(heartRate);
            ToggleVisibility();
        }
        
        bpmText.text = "BPM: " + heartRate;
        
    }

    void SaveHeartRateData(int heartRate)
    {
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
        float tsoundRange, tsightRange;
        int MinHR = HeartRateData.Instance.MinHR;
        int MaxHR = HeartRateData.Instance.MaxHR;

        if (heartRate < MinHR)
        {
            tsightRange = 5f;
            tsoundRange = 5f;
            IsVisible = false;
            Debug.LogWarning($"Low Heart Rate - Visibility: {IsVisible}");
        }

        else if (heartRate > MaxHR)
        {
            tsightRange = 24f;
            tsoundRange = 40f;
            IsVisible = true;

            _threshold = Time.deltaTime;

            if (_threshold >= 6)
            {
                _enemyAI.Agent.speed = 100;
            }

            Debug.LogWarning($"High Heart Rate - Enemy Speed: {_enemyAI.Agent.speed} Visibility: {IsVisible}");
        }
        else
        {
            tsightRange = 12f;
            tsoundRange = 20f;
            IsVisible = true;
        }

        _enemyAI.SightRange = Mathf.Lerp(_enemyAI.SightRange, tsightRange, Time.deltaTime * 1.5f);
        _enemyAI.SoundRange = Mathf.Lerp(_enemyAI.SoundRange, tsoundRange, Time.deltaTime * 1.5f);

        // Research into the heart, average heart rate, decrease it
        // Find a way for arduino to get the average heart rate
        // Find a way to save the heart rate data while the user is playing the game.

    }

}

// https://dyadica.co.uk/blog/unity3d-serialport-script/