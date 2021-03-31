using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;

public class ArduinoInput : MonoBehaviour
{
    // Port related variables
    public string portName = "COM6";
    public SerialPort sp;
    private bool blnPortcanopen = false;

    // Other variables
    private GameObject navAgent;
    private Light flashLight;
    private GameObject localPlayer;
    private float distance;

    // Statics to communicate with the serial com thread
    static private int databyte_in; //read databyte from serial port
    static private bool databyteRead = false; //becomes true if there is indeed a character received
    static private int databyte_out; //index in txChars array of possible characters to send
    static private bool databyteWrite = false; //to let the serial com thread know there is a byte to send

    // txChars contains the characters to send: we have to use the index
    private char[] txChars = { 'W', 'X', 'Y', 'Z' };
    private char inputChar = 'a';

    // Thread-related
    private bool stopSerialThread = false; //to stop the thread
    private Thread readWriteSerialThread; //threadvariabele

    void Start()
    {
        sp = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
        OpenConnection();

        readWriteSerialThread = new Thread(SerialThread);
        readWriteSerialThread.Start();

        navAgent = GameObject.FindGameObjectWithTag("NavAgent");
        localPlayer = GameObject.FindGameObjectWithTag("Player");
        flashLight = localPlayer.GetComponentInChildren<Light>();
    }

    void Update()
    {
        if (databyteRead)
        {
            databyteRead = false;
            inputChar = (char)databyte_in;

            switch (inputChar)
            {
                case 'A':
                    setSpeed(1f);
                    break;
                case 'B':
                    setSpeed(2f);
                    break;
                case 'C':
                    setSpeed(3f);
                    break;
                case 'D':
                    setSpeed(4f);
                    break;
                case 'E':
                    setSpeed(5f);
                    break;
                case 'F':
                    if (flashLight.enabled)
                    {
                        flashLight.enabled = false;
                    }
                    else
                    {
                        flashLight.enabled = true;
                    }
                    break;

                default:
                    Debug.Log("Wrong input");
                    break;
            }
        }

        if(localPlayer  != null) { 
        distance = Vector3.Distance(localPlayer.transform.position, navAgent.transform.position);
        }

        if (distance < 10)
        {
            if (databyte_out != 3) // check is necessary in order to not continously send data
            {
                databyte_out = 3;
                databyteWrite = true;
            }
        }
        if (distance >= 10 && distance < 20)
        {
            if (databyte_out != 2)
            {
                databyte_out = 2;
                databyteWrite = true;
            }
        }
        if (distance >= 20 && distance < 30)
        {
            if (databyte_out != 1)
            {
                databyte_out = 1;
                databyteWrite = true;
            }
        }
        if (distance >= 30)
        {
            if (databyte_out != 0)
            {
                databyte_out = 0;
                databyteWrite = true;
            }
        }
    }

    void setSpeed(float speed)
    {
        localPlayer.GetComponent<PlayerMovement>().walkSpeed = speed;
    }

    void SerialThread() //separate thread is needed because we need to wait sp.ReadTimeout = 20 ms to see if a byte is received
    {
        while (!stopSerialThread) //close thread on exit program
        {
            if (blnPortcanopen)
            {
                if (databyteWrite)
                {
                    if (databyte_out == 0)
                    {
                        sp.Write(txChars, 0, 1); //tx 'W'
                    }
                    if (databyte_out == 1)
                    {
                        sp.Write(txChars, 1, 1); //tx 'X'
                    }
                    if (databyte_out == 2)
                    {
                        sp.Write(txChars, 2, 1); //tx 'Y'
                    }
                    if (databyte_out == 3)
                    {
                        sp.Write(txChars, 3, 1); //tx 'Z'
                    }
                    databyteWrite = false; //to be able to send again
                }
                try
                {
                    databyte_in = sp.ReadChar();
                    databyteRead = true;
                }
                catch (Exception)
                {
                    //Debug.Log(e.Message);
                }
            }
        }
    }

    //Function connecting to Arduino
    public void OpenConnection()
    {
        if (sp != null)
        {
            if (sp.IsOpen)
            {
                string message = "Port is already open!";
                Debug.Log(message);
            }
            else
            {
                try
                {
                    sp.Open();  // opens the connection
                    blnPortcanopen = true;
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    blnPortcanopen = false;
                }
                if (blnPortcanopen)
                {
                    sp.ReadTimeout = 20;  // sets the timeout value before reporting error
                    Debug.Log("Port Opened!");
                }
            }
        }
        else
        {
            Debug.Log("Port == null");
        }
    }

    void OnApplicationQuit()
    {
        if (sp != null) sp.Close();
        stopSerialThread = true;
        readWriteSerialThread.Abort();
    }
}
