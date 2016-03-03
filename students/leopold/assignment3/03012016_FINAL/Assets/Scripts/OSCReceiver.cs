using UnityEngine;
using System.Collections;



public class OSCReceiver : MonoBehaviour {
    


public string RemoteIP = "127.0.0.1"; //127.0.0.1 signifies a local host (if testing locally
public int SendToPort= 9000; //the port you will be sending from
public int ListenerPort = 8050; //the port you will be listening on
public Transform controller;
public string gameReceiver = "Cube"; //the tag of the object on stage that you want to manipulate
private Osc handler;
private int yRot=0; //the rotation around the y axis
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
        GameObject go = GameObject.Find(gameReceiver) as GameObject;
		GameObject go2 = GameObject.Find("Ball") as GameObject;
      
        go.transform.Rotate(0, yRot, 0);
		//go2.transform.Rotate(0, yRot, 0);
		go2.transform.localScale = new Vector3 (1, yRot, 0);
        
	}
    
    public void AllMessageHandler(OscMessage message){
        
        Debug.Log(message);
        Debug.Log("stuff sent");
    
        string msgString =  Osc.OscMessageToString(message); //the message and value combined
        float myValue = (float)message.Values[0];
        
        int iValue = Mathf.RoundToInt(myValue);
        yRot = iValue;

    }
}

