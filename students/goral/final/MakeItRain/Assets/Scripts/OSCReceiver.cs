using UnityEngine;
using System.Collections;



public class OSCReceiver : MonoBehaviour {
    


public string RemoteIP = "127.0.0.1"; //127.0.0.1 signifies a local host (if testing locally
public int SendToPort= 9000; //the port you will be sending from
public int ListenerPort = 8000; //the port you will be listening on
public Transform controller;
public string gameReceiver = "Ben"; //the tag of the object on stage that you want to manipulate
private Osc handler;
private int zoom=0; //the zoom
private int iValue = 0;
private float myValue = 0;
	// Use this for initialization
	void Start () {
        
  
    UDPPacketIO udp = GetComponent<UDPPacketIO>();
   Debug.Log(udp);
	udp.init(RemoteIP, SendToPort, ListenerPort);
	handler = GetComponent<Osc>();
	handler.init(udp);
	handler.SetAllMessageHandler(AllMessageHandler);
	
	}
	
	// Update is called once per frame
	void Update () {

        iValue = Mathf.RoundToInt(myValue);
        zoom -= iValue * 2;

        GameObject go = GameObject.Find(gameReceiver) as GameObject;

        if (iValue == 1) {
            go.transform.Translate(zoom, 0, 0);
        } else if(iValue == 0) {
            go.transform.position = new Vector3(0, -7, 5);
            zoom = 0;
        }

        Debug.Log(iValue);

    }
    
    public void AllMessageHandler(OscMessage message){
    
        string msgString =  Osc.OscMessageToString(message); //the message and value combined
        myValue = (float)message.Values[0];

    }
}

