using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private int panCount = 3;   //TODO: move into Static Game Class once exist

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
    private int inputCount = 15;
    [SerializeField]
    private int inputLength = 5;
	private bool hasReadLine = false;
	// input values...

    // static so it is avable to everything :)  // use GetInputs() for access 
    private static InputValues inputValues;

    private void Start()
    {

        inputValues.Init();
        serial = new SerialPort("COM" + COM_port, COM_updateRate);
        serial.ReadTimeout = readTimeout;

        // Serial or mouse & keyboard
        try
        {
            serial.Open();
            useSerial = true;
            
        }
        catch (System.Exception e)
        {
            print("Unable to connect to serial port: COM" + COM_port + " @ "+ COM_updateRate + " ("+e+")");
        }

    }

    void Update()
    {
		// Get Inputs

		if ( Input.GetKeyDown( "n" ) )
			NormalizeInputs();

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

        string line = string.Empty;

            try
            {
				if ( !hasReadLine )
					NormalizeInputs();

                serial.WriteLine(writeChar);
                serial.BaseStream.Flush();
                line = serial.ReadLine();

                UpdateInputValuesFromSerialLine(line);

				hasReadLine = true;

            }
            catch (System.Exception e)
            {
                Debug.LogError("Error: Unable to read line :( ( " + e + " )");
            }

    }

    void UpdateInputValuesFromSerialLine(string line)
    {
        string[] inputVals = SplitSerialLine(line);
        int currentValIndex = 0;

        // parse the pans, hob and distance into the input values
        for (int i = 0; i < panCount; i++)
        {
            inputValues.pans_x[i] = int.Parse( inputVals[currentValIndex] );
            currentValIndex++;

            inputValues.pans_y[i] = int.Parse( inputVals[currentValIndex] );
            currentValIndex++;

            inputValues.panDistances[i] = int.Parse(inputVals[currentValIndex]);
            currentValIndex++;

            inputValues.hobs[i] = int.Parse( inputVals[currentValIndex] );
            currentValIndex++;

        }

        // parse the jug and whisk :)
        inputValues.jug = int.Parse(inputVals[currentValIndex]);
        currentValIndex++;

        inputValues.whisk = int.Parse(inputVals[currentValIndex]);
		currentValIndex++;

		inputValues.panToggle = int.Parse( inputVals[ currentValIndex ] );

	}

    //Splits string into array of checks of 'inputLength'
    string[] SplitSerialLine(string line)
    {
        string[] inputs = new string[inputCount];

        int currentInputId = 0;
        int currentStrPosition = 0;
        
        // Get each input from line.
        // since each input is the same size we can extract each on in checks of 'inputLength' (5)
        while( currentInputId < inputCount )
        {
            inputs[currentInputId] = line.Substring(currentStrPosition, inputLength);
            currentInputId++;
            currentStrPosition += inputLength;
        }

        return inputs;
    }

    private void NormalizeInputs()
	{
		serial.WriteLine( "N" );
		serial.BaseStream.Flush();
	}

    private void OnDestroy()
    {
        if(useSerial)   // TODO: check if its open?? 
            serial.Close();
    }

    public static InputValues GetInputs()
    {
        return inputValues;
    }

}

public struct InputValues	// this might be better off being a dict :)
{

    public int[] pans_x;
    public int[] pans_y;
    public int[] hobs;
    public int[] panDistances;
    public int jug;
    public int whisk;
	public int panToggle;	//TEMP, this is only until i find a better solution to detect jug position :)

    public void Init()
    {
        pans_x = new int[3];
        pans_y = new int[3];
        hobs = new int[3];
        panDistances = new int[3];
        jug = 0;
        whisk = 0;
		panToggle = 0;
    }

}