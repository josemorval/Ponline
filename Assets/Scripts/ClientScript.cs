using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ClientScript : MonoBehaviour {

	#region public variables
	[Header("Datos del host")]
	public string hostIpAddress;
	public int hostPort;
	#endregion

	#region private variables
	NetworkClient myClient;
	short counterMessagges;

	#endregion
	/*
	public void OnConnected(NetworkConnection conn, NetworkReader reader){
		Debug.Log ("Connected to server");
	}

	public void OnDisconnected(NetworkConnection conn, NetworkReader reader){
		Debug.Log ("Disonnected to server");
	}

	public void OnError(NetworkConnection conn, NetworkReader reader){
		Debug.Log ("Error connecting");
	}*/

	// Custom initialization (has to be called in GameManager!!)
	public void StartComponent (string hostIpAddress, int hostPort) {
		myClient = new NetworkClient ();
		this.hostIpAddress = hostIpAddress;
		this.hostPort = hostPort;
		counterMessagges = 0;
		/*
		myClient.RegisterHandler (MsgType.Connect, OnConnected);
		myClient.RegisterHandler (MsgType.Disconnect, OnDisconnected);
		myClient.RegisterHandler (MsgType.Error, OnError);
		*/

		myClient.Connect (hostIpAddress, hostPort);
		/*Hacemos una especia de handshake para confirmar la conexion entre los dos*/
		myClient.Send (counterMessagges, );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
