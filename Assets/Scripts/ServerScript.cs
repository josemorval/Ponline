using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ServerScript : MonoBehaviour {

	#region public variables
	public int listenPort;
	public short idHandshake;

	#endregion

	#region private variables
	NetworkServerSimple server;

	string clientAddress;
	int clientPort;

	Vector3 playerClient;
	bool flagActive = true;

	#endregion

	void OnHandshakeMessage(NetworkMessage netMsg){
		clientAddress = netMsg.conn.address;
		clientPort = netMsg.reader.ReadInt32; //El puerto debe de estar incluido en el mensaje
	}

	void OnCoordinatesMessage(NetworkMessage netMsg){
		playerClient = netMsg.reader.ReadVector3;
	}

	// Use this for initialization
	void Start () {
		server = new NetworkServerSimple ();
		server.Listen (listenPort);
		idHandshake = 0;

		/*Handler de los mensajes de 'handshake'/inicio de partida*/
		server.RegisterHandler (0, OnHandshakeMessage);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	/*Servicio principal donde se produce el envio de mensajes al cliente y la recepcion de los suyos con el 
	consecuente actualizacion de variables (Ball y players).

	NOTA: Estoy dudando de si una corutina es la mejor opcion... En este caso, es un juego simple pero en uno con mas carga
	se enlenteceria la actualizacion de las posiciones...*/
	IEnumerator serverProcess(){
		while (flagActive) {
			
		}
	}

	public void deactivate(){
		flagActive = false;
	}

}
