using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ClientScript : MonoBehaviour {

	#region public variables
	[Header("Tiempo de espera para reintentar la conexion al oponente")]
	public float waitTimeRetry;

	public int randomSeed;

	#endregion

	#region private variables
	NetworkClient myClient;
	short counterMessagges;
	int reliableChannel;
	int unreliableChannel;
	int hostId;
	HostTopology topology;
	ConnectionConfig config;
	string oponentIpAddress;
	int oponentPort;
	int myPort;

	bool connectionEstablished;
	float timePassed;
	byte error;
	int connectionId;

	GameObject ball;
	GameObject playerone;
	GameObject playertwo;


	bool isFirstPlayer;
	bool isSecondPlayer;

	byte[] buffer = new byte[100];
	int bufferLength;
	int channelId;

	NetworkEventType recData;

	#endregion

	// Custom initialization (has to be called in GameManager!!)
	public void StartComponent (string oponentIpAddress, int oponentPort, int myPort) {
		/*Iniciamos la capa de transporte*/
		NetworkTransport.Init();

		/*Configuracion de la capa*/
		config = new ConnectionConfig();
		reliableChannel = config.AddChannel (QosType.Reliable); //Canal especifico para datos importantes del principio del juego
		unreliableChannel = config.AddChannel (QosType.Unreliable); //Canal usual para el bulk de los datos
		topology = new HostTopology(config, 1); //Cuantas conexiones deseamos que soporte el host (solo una, player opuesto)

		/*Creacion del Host. Es una comunicacion P2P pura, cada peer hace de host y envia datos*/
		hostId = NetworkTransport.AddHost (topology, myPort);

		/*Intentaremos contactar con el oponente. Si ya existe, el jugador local sera el segundo, si no el primero.Ser el primer player 
		 * supone que determinara que semilla para el random se usa y sus coordenadas de ball son absolutas (el segundo player se ajustara a ellas).

		NOTA: Super inseguro esto, si los dos peers realizan el start a la vez y les llega la conexion a la vez, pensaran que son el second player y no funcionara bien.*/
		connectionId = NetworkTransport.Connect(hostId, oponentIpAddress, oponentPort, 0, error);

		if (error == 0) {
			connectionEstablished = true;
			isSecondPlayer = true;
			isFirstPlayer = false;
		} else {
			connectionEstablished = false;
			isSecondPlayer = false;
			isFirstPlayer = true;
		}

		/*Inicializaciones varias*/
		timePassed = waitTimeRetry;

		this.oponentIpAddress = oponentIpAddress;
		this.oponentPort = oponentPort;
		this.myPort = myPort;

		ball = GameObject.Find ("Ball");
		playerone = GameObject.Find ("Player1");
		playertwo = GameObject.Find ("Player2");
	}
	
	// Update is called once per frame
	void Update () {
		timePassed += Time.deltaTime;

		if (!connectionEstablished) {//Conexion con el oponente
			if (timePassed > waitTimeRetry) {
				connectionId = NetworkTransport.Connect (hostId, this.oponentIpAddress, this.oponentPort, 0, error);
			
				if (error == 0) {
					connectionEstablished = true;
					/*Envio de la semilla para el random con ID 0 de mensaje al oponente*/
					buffer[0] = randomSeed;
					NetworkTransport.Send (hostId, connectionId, reliableChannel, buffer, sizeof(int), error);

					//START MATCH [CODE MISSING]
				}
				timePassed -= waitTimeRetry;
			}
		} 
		else { //Envio, recibo mensajes y actualizaciones

			/*Recibo de datos*/
			recData = NetworkTransport.ReceiveFromHost(hostId, connectionId, channelId, buffer, 100, out, error);
			switch(recData){
				case NetworkEventType.Nothing:
					break;
				case NetworkEventType.ConnectEvent:
					break;
				case NetworkEventType.DataEvent:
				if(reliableChannel == channelId){ //Mensaje del randomseed
					randomSeed = buffer[0];
					//SET RANDOMSEED [CODE MISSING]
					//START MATCH [CODE MISSING]
				}
				else{
					if(isSecondPlayer){
					//UPDATE BALL COORDINATES
					//UPDATE PLAYER1 COORDINATES
					//TELL BALL SCRIPT NOT TO PREDICT
					}
					else{
					//UPDATE PLAYER2 COORDINATES
					//TELL BALL SCRIPT NOT TO PREDICT
					}
				}
					break;
				case NetworkEventType.DisconnectEvent:
					break;
			}
		}
	}
}
