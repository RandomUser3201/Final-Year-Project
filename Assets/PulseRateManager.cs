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

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 100;
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
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial Port Closed");
        }
    }
}

