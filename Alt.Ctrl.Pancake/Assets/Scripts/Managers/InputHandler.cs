using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO.Ports;		// Access to serial
using System.Threading;		// create a thread for the serial commication. (otherwise it slow as)


/// <summary>
/// handles both sreial and mouse/keyboard inputs
/// </summary>
public class InputHandler : MonoBehaviour
{

	// Input Data :)
	private Dictionary<string, int> inputValues = new Dictionary<string, int>();

	//Mouse/Keyboard inputs

	// Serial inputs
	private bool usingSerial = false;                   // auto detected, if we are able to get a connect to the serial device
	[Header( "Serial" )]
	[SerializeField] private int portNumber = 1;
	[SerializeField] private int baudRate = 9600;
	[SerializeField] private int readTimeout = 100;     // ms
	[SerializeField] private int inputLength = 5;  


	private SerialPort serialConnection;

	private Thread serial_thread;
	private Queue serial_outputQueue;
	private Queue serial_inputQueue;
	private const int serial_maxQueueSize = 1;

	private bool serial_isRunning = false;
	private bool SerialThread_isRunning {	// Thread Safe
		get {
			lock(this){
				return serial_isRunning;
			}
		}
		set {
			lock(this)
			{
				serial_isRunning = value;
			}
		}
	}

	// Start is called before the first frame update
	void Start()
    {

		usingSerial = ConnectToSerial();

		// Setup the thread and queue if we have a serial conection
		if ( usingSerial )
		{
			SetupThread();
			Serial_queueWriteLine( "N" ); //Normlize the serial device :)
		}


    }

	private void SetupThread()
	{
		// On your marks...
		serial_inputQueue = Queue.Synchronized( new Queue(serial_maxQueueSize) );	// there no point is being any larger than maxQueue.
		serial_outputQueue = Queue.Synchronized( new Queue() );
		// ...Get set...
		serial_thread = new Thread( SerialThread );
		// GO!
		SerialThread_isRunning = true;
		serial_thread.Start();
	}

    // Update is called once per frame
    void Update()
    {
		//Manually normlize the serial device
		if ( usingSerial && Input.GetKeyDown( "n" ))
			Serial_queueWriteLine( "N" );


		if ( !usingSerial )
			MouseAndKeyboardInputs();
		else
			SerialInputs();
    }

	private void MouseAndKeyboardInputs()
	{

	}

	private void SerialInputs()
	{
		print( serial_inputQueue.Count );
		if ( serial_inputQueue.Count == 0 ) return;

		//Right, let make them serial inputs available to the rest of the game :)
		int[] inputVal = Serial_splitInputs( (string)serial_inputQueue.Dequeue() );
		int currentInputId = 0;

		for ( int i = 0; i < GameGlobals.fryingpanCount; i++ )
		{

			UpdateInputValue( "panX_" + i, inputVal[currentInputId] );
			currentInputId++;

			UpdateInputValue( "panY_" + i, inputVal[ currentInputId ] );
			currentInputId++;

			UpdateInputValue( "panDistance_" + i, inputVal[ currentInputId ] );
			currentInputId++;

			UpdateInputValue( "panHob_" + i, inputVal[ currentInputId ] );
			currentInputId++;
		}

		UpdateInputValue( "jug", inputVal[ currentInputId ] );
		currentInputId++;

		UpdateInputValue( "whisk", inputVal[ currentInputId ] );
		currentInputId++;

		UpdateInputValue( "panToggle", inputVal[ currentInputId ] );
		currentInputId++;

		// request the next inputs, ready for the next update

		print( inputValues[ "panX_0" ] );

	}

	/// <summary>
	/// Attempts to connect to serial returns true if seccessful
	/// </summary>
	private bool ConnectToSerial()
	{

		serialConnection = new SerialPort( "COM" + portNumber, baudRate );
		serialConnection.ReadTimeout = readTimeout;

		try
		{
			serialConnection.Open();
		}
		catch ( System.Exception e )
		{
			print( "Unable to connect to serial port: COM" + portNumber + " @ " + baudRate + " (" + e + ")" );
			return false;
		}

		return true;

	}

	private int[] Serial_splitInputs(string line)
	{
		int inputCount = Mathf.CeilToInt( line.Length / inputLength );
		int[] inputs = new int[inputCount];

		// extract all the inputs and patse them into into ready to be used :)
		for( int currentInputId = 0; currentInputId < inputCount; currentInputId++ )
		{
			inputs[ currentInputId ] = int.Parse(line.Substring(currentInputId*inputLength, inputLength)); 
		}
		
		return inputs;
	}

	private void SerialThread()
	{

		string serialReadData = string.Empty;
		

		while (SerialThread_isRunning)
		{

			// Read from serial
			// if there no data in the que to be read, attampt to get the next line.
			if(serial_inputQueue.Count < serial_maxQueueSize)
			{
				serialReadData = Serial_readLine();
				if(serialReadData != string.Empty)
				{
					serial_inputQueue.Enqueue((object)serialReadData);
					serialReadData = string.Empty;
				}
			}

			// write all output data to serial
			while(serial_outputQueue.Count > 0)
			{
				string dataToSend = (string)serial_outputQueue.Dequeue();
				Serial_writeLine(string.Empty);
			}

		}

		// Exit the serial now that the thread has stoped :)
		serialConnection.Close();

	}

	private string Serial_readLine()
	{
		string line = string.Empty;

		try
		{
			Serial_writeLine( "i" );
			serialConnection.BaseStream.Flush();
			line = serialConnection.ReadLine();

		}
		catch(System.Exception e)
		{
			Debug.LogError( "Failed to read line to serial (" + e + ")" );

		}

		return string.Empty;
	}

	private void Serial_writeLine(string line)
	{

		try
		{
			serialConnection.WriteLine( line );
			serialConnection.BaseStream.Flush();
		}
		catch(System.Exception e)
		{
			Debug.LogError( "Failed to write line to serial (" + e + ")" );
		}
	}

	public void Serial_queueWriteLine(string line)
	{
		if(usingSerial)
			serial_outputQueue.Enqueue( (object)line );
	}

	/// <summary>
	/// Gets the input value for input name
	/// </summary>
	/// <param name="inputName">Name of input</param>
	/// <param name="value">REF Value of input</param>
	/// <returns>False if the input does not exist</returns>
	public bool GetInputValue(string inputName, ref int value)
	{

		if( !inputValues.ContainsKey(inputName) )
		{
			Debug.LogError("Input value for "+inputName+" does not exist");
			return false;
		}

		value = inputValues[ inputName ];

		return true;

	}

	private void UpdateInputValue(string inputName, int value)
	{

		if ( inputValues.ContainsKey( inputName ) )
			inputValues[ inputName ] = value;
		else
			inputValues.Add( inputName, value );

	}

	private void OnDestroy()
	{

		// make shore the the thread is stoped :)
		// if the thread is alive stop it and wait for it to die.

		if(usingSerial)
			while ( serial_thread.IsAlive )			// could this break every thing?? ... Lets find out :D 
				if ( SerialThread_isRunning )
					SerialThread_isRunning = false;
		
	}

}
