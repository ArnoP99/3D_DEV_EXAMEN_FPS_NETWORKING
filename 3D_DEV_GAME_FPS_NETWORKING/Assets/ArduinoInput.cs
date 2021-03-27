using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System;
using Photon.Pun;

public class ArduinoInput : MonoBehaviour
{
    public string portName = "COM6";
    public SerialPort sp; // test
    private bool blnPortcanopen = false; //if portcanopen is true the selected comport is open

    private GameObject navAgent;

    //statics to communicate with the serial com thread
    static private int databyte_in; //read databyte from serial port
    static private bool databyteRead = false; //becomes true if there is indeed a character received
    static private int databyte_out; //index in txChars array of possible characters to send
    static private bool databyteWrite = false; //to let the serial com thread know there is a byte to send
    //txChars contains the characters to send: we have to use the index
    private char[] txChars = { 'W', 'X', 'Y', 'Z' };
    private char inputChar = 'a';
    private int inputNum = 0;
    //threadrelated
    private bool stopSerialThread = false; //to stop the thread
    private Thread readWriteSerialThread; //threadvariabele
    private Light flashLight;
    private GameObject[] players;
    private GameObject mainPlayer;

    void Start()
    {
        sp = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
        OpenConnection(); //init COMPort
                          //define thread and start it
        readWriteSerialThread = new Thread(SerialThread);
        readWriteSerialThread.Start(); //start thread
        navAgent = GameObject.FindGameObjectWithTag("NavAgent");
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players)
        {
            if (item.GetPhotonView().IsMine)
            {
                mainPlayer = item;
            }
        }

    }

    void Update()
    {
        if (databyteRead) //if a databyte is received
        {
            databyteRead = false; //to see if a next databyte is received
            inputChar = (char)databyte_in;

            //GameObject.Find("ArduinoIO").GetComponent<PhotonView>().RequestOwnership();
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
                    players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (var item in players)
                    {
                        if (item.GetPhotonView().IsMine)
                        {
                            flashLight = item.GetComponentInChildren<Light>();
                        }
                    }
                    Debug.Log("Button Pressed");
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
                    Debug.Log("Wrong input, ERROR!");
                    break;
            }
        }
        float distance = Vector3.Distance(mainPlayer.transform.position, navAgent.transform.position);
        if (distance < 10)
        {
            if (databyte_out != 3)
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
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players)
        {
            if (item.GetPhotonView().IsMine)
            {
                item.GetComponent<PlayerMovement>().walkSpeed = speed;
            }
        }
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
                        sp.Write(txChars, 0, 1); //tx 'A'
                    }
                    if (databyte_out == 1)
                    {
                        sp.Write(txChars, 1, 1); //tx 'U'
                    }
                    if (databyte_out == 2)
                    {
                        sp.Write(txChars, 2, 1); //tx 'U'
                    }
                    if (databyte_out == 3)
                    {
                        sp.Write(txChars, 3, 1); //tx 'U'
                    }
                    databyteWrite = false; //to be able to send again
                }
                try //trying something to receive takes 20 ms = sp.ReadTimeout
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


    void OnApplicationQuit() //proper afsluiten van de thread
    {
        if (sp != null) sp.Close();
        stopSerialThread = true;
        readWriteSerialThread.Abort();
    }
}
