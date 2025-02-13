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

    public bool visibility = true;
    private EnemyAI enemyai;


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
    }

    // Update is called once per frame
    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            string data = serialPort.ReadLine().Trim();
            heartRate = int.Parse(data);
            Debug.Log("Heart Rate: " + heartRate);
        }       

        bpmText.text = "BPM: " + heartRate;
        Debug.Log("BPM Cooldown: ");

        ToggleVisibility();
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
        if (heartRate <= 120)
        {
            enemyai.sightRange = 5f;
            enemyai.soundRange = 5f;
            visibility = false;
            Debug.LogWarning("HEART RATE BELOW 120, INVISIBLE.");
        }

        else 
        {
            enemyai.sightRange = 24f;
            enemyai.soundRange = 40f;
            visibility = true;
            Debug.LogWarning("HEART RATE above 120.");
        }
    }
}

