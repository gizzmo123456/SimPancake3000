using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Serial")]

    [SerializeField]
    private int COM_port = 3;
    [SerializeField]
    private int COM_updateRate = 9600;

    private SerialPort serial;
    [SerializeField]
    [Tooltip("in ms")]
    private int readTimeout = 100;
    private bool useSerial = false;

    [SerializeField]
    private string writeChar = "r";
    [SerializeField]
    private char inputSeperator = ';';
    // input values...

    // static so it is avable to everything :)  // use GetInputs() for access 
    private static InputValues inputValues;

    private void Awake()
    {

        serial = new SerialPort("COM"+COM_port, COM_updateRate);

        // Serial or mouse & keyboard
        try
        {
            serial.Open();
            serial.ReadTimeout = readTimeout;
            useSerial = true;
        }
        catch (System.Exception)
        {
            print("Unable to connect to serial port: COM" + COM_port + " @ "+ COM_updateRate);
        }

    }
    
    void Update()
    {
        // Get Inputs

        if (useSerial)
            SerialInputs();
        else
            MouseKeyboardInputs();

    }

    void MouseKeyboardInputs()
    {
        //TODO...
    }

    void SerialInputs()
    {

        char[] buffer = new char[1];

        string line = string.Empty;

        try
        {
            serial.Write(writeChar);
            line = serial.ReadLine();
        }
        catch
        {
            Debug.LogError("Error: Unable to read line :(");
        }

        string[] inputValues = line.Split(';');

    }


    public static InputValues GetInputs()
    {
        return inputValues;
    }

}

public struct InputValues
{

    public int[] pans;
    public int[] hobs;
    public int[] panDistances;
    public int jug;
    public int whisk;

    public void Init()
    {
        pans = new int[3];
        hobs = new int[3];
        panDistances = new int[3];
        jug = 0;
        whisk = 0;
    }

}