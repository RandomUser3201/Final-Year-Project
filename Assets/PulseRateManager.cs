using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PulseRateManager : MonoBehaviour
{
    SerialPort serialPort;
    public string portName = "COM5";
    public int baudRate = 9600;
    public int heartRate = 0;
    public Text bpmText;

    public bool visibility = true;
    private EnemyAI enemyai;
    private float  threshold = 0f;

    private string filePath;

    private float lastUpdateTime = 0f;
    private float updateInterval = 5f;

    void Start()
    {
        enemyai = FindObjectOfType<EnemyAI>();
        if (enemyai == null)
        {
            Debug.LogWarning("enemy ai null");
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
            Debug.LogError("Could not open serial port: " + e.Message);
        }

        filePath = Application.persistentDataPath + "/HeartRateLog.csv";
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Timestamp,HeartRate,EnemyState\n");
        }
        Debug.LogError("Persistent data path: " + Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {

            lastUpdateTime = Time.time;

            if (serialPort != null && serialPort.IsOpen)
            {
                string data = serialPort.ReadLine().Trim();
                int parsedHeartRate;
                // heartRate = int.Parse(data);
                Debug.Log("Heart Rate Outside Parse: " + heartRate);
                
                if (int.TryParse(data, out parsedHeartRate))
                {
                    heartRate = parsedHeartRate;

                    int minHR = HeartRateData.Instance.minHR;
                    int maxHR = HeartRateData.Instance.maxHR;

                    if (heartRate < minHR || heartRate > maxHR)
                    {
                        Debug.LogWarning($"Heart rate {heartRate} outside normal range ({minHR} {maxHR}) for  {HeartRateData.Instance.playerAge} year old");
                    }

                    Debug.Log("Heart Rate In Try Parse: " + heartRate);
                }
                
                else
                {
                    Debug.LogError("Invalid heart rate data received: " + data);
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
        string logEntry = $"{timestamp},{heartRate},{enemyai.GetCurrentState()}\n";
        Debug.Log($"Saving heart rate data: {logEntry}"); 

        File.AppendAllText(filePath, logEntry);
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
        int minHR = HeartRateData.Instance.minHR;
        int maxHR = HeartRateData.Instance.maxHR;

        if (heartRate < minHR)
        {
            tsightRange = 5f;
            tsoundRange = 5f;
            visibility = false;
            Debug.LogWarning("Low Heart Rate - Visibility: " + visibility);
        }

        else if (heartRate > maxHR)
        {
            tsightRange = 24f;
            tsoundRange = 40f;
            visibility = true;

            threshold = Time.deltaTime;

            if (threshold >= 6)
            {
                enemyai.agent.speed = 100;
            }

            Debug.LogWarning("High Heart Rate - Enemy Speed: " + enemyai.agent.speed + " Visibility: " + visibility);
        }
        else
        {
            tsightRange = 12f;
            tsoundRange = 20f;
            visibility = true;
        }

        enemyai.sightRange = Mathf.Lerp(enemyai.sightRange, tsightRange, Time.deltaTime * 1.5f);
        enemyai.soundRange = Mathf.Lerp(enemyai.soundRange, tsoundRange, Time.deltaTime * 1.5f);

        // Research into the heart, average heart rate, decrease it
        // Find a way for arduino to get the average heart rate
        // Find a way to save the heart rate data while the user is playing the game.

    }

}

// https://dyadica.co.uk/blog/unity3d-serialport-script/