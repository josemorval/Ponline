using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class ClientScript : MonoBehaviour {

	#region public variables
	[Header("Tiempo de espera para reintentar la conexion al oponente")]
	public float waitTimeRetry;

	[Header("Cantidad de paquetes por segundo (ratio de actualizacion del estado del juego")]
	public int tickRate = 60;

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
	BallController ballScript;
	GameObject playerone;
	GameObject playertwo;
	PlayerController playeroneScript;
	PlayerController playertwoScript;


	bool isFirstPlayer;
	bool isSecondPlayer;

	byte[] buffer = new byte[100];
	byte[] aux = new byte[100];
	int bufferLength;
	int channelId;
	int counter;

	NetworkEventType recData;
	Vector3 posicion;

	bool ballActiveFlag;
	bool playeroneActiveFlag;
	bool playertwoActiveFlag;

	int receivedSize;
	int connectionIdOut;
	float waitTimePackage;
	bool matchPaused;

	bool isHost;

	#endregion

	// Custom initialization (has to be called in GameManager!!)
	public void StartComponent (string oponentIpAddress, int oponentPort, int myPort, bool isHost) {
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
		connectionId = NetworkTransport.Connect(hostId, oponentIpAddress, oponentPort, 0, out error);

		/*Inicializaciones varias*/
		timePassed = waitTimeRetry;

		this.oponentIpAddress = oponentIpAddress;
		this.oponentPort = oponentPort;
		this.myPort = myPort;

		ball = GameObject.Find ("Ball");
		playerone = GameObject.Find ("Player1");
		playertwo = GameObject.Find ("Player2");
		ballScript = ball.GetComponent<BallController> ();
		playeroneScript = playerone.GetComponent<PlayerController> ();
		playertwoScript = playertwo.GetComponent<PlayerController> ();
		posicion = new Vector3 ();

		ballActiveFlag = true;
		playeroneActiveFlag = true;
		playertwoActiveFlag = true;
		matchPaused = true;

		waitTimePackage = 1 / tickRate;

		if (isHost) {
			isFirstPlayer = true;
			isSecondPlayer = false;
			playeroneScript.modo = PlayerController.playerMode.Local;
			playertwoScript.modo = PlayerController.playerMode.Online;
		}
		else{
			isFirstPlayer = false;
			isSecondPlayer = true;
			playeroneScript.modo = PlayerController.playerMode.Online;
			playertwoScript.modo = PlayerController.playerMode.Local;
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (!ballScript.enabled && !matchPaused) {
			ballScript.enabled = true;
		}
		if (!playeroneScript.enabled && !matchPaused) {
			playeroneScript.enabled = true;
		}
		if (!playertwoScript.enabled && !matchPaused) {
			playertwoScript.enabled = true;
		}

		timePassed += Time.deltaTime;

		if (!connectionEstablished) {//Conexion con el oponente
			if (isConnected ()) {
				connectionEstablished = true;
				if (isFirstPlayer) {
					/*Envio de la semilla para el random con ID 0 de mensaje al oponente*/
					Array.Copy (BitConverter.GetBytes (randomSeed), buffer, sizeof(int));
					NetworkTransport.Send (hostId, connectionId, reliableChannel, buffer, sizeof(int), out error);

					//START MATCH
					ballScript.enabled = true;
					ballScript.RestartMatch ();
					matchPaused = false;
				}
			} else if (timePassed > waitTimeRetry) {
					connectionId = NetworkTransport.Connect (hostId, this.oponentIpAddress, this.oponentPort, 0, out error);
					timePassed -= waitTimeRetry;
			}


		} 
		else { //Envio, recibo mensajes y actualizaciones
			buffer = new byte[100];
			/*Recibo de datos*/
			recData = NetworkTransport.ReceiveFromHost(hostId, out connectionIdOut, out channelId, buffer, 100, out receivedSize, out error);

			switch(recData){
				case NetworkEventType.Nothing:
					break;
				case NetworkEventType.ConnectEvent:
					break;
				case NetworkEventType.DataEvent:
				if(reliableChannel == channelId){ //Mensaje del randomseed
					aux = new byte[100];
					Array.Copy (buffer, aux, sizeof(int));
					randomSeed = BitConverter.ToInt32(aux, 0);

					ballScript.seedRandom = randomSeed;
					ballScript.updateSeed = true;
					//START MATCH 
					ballScript.enabled = true;
					ballScript.RestartMatch();
					matchPaused = false;
				}
				else{
					counter = sizeof(float);

					if(isSecondPlayer){
						posicion.Set(BitConverter.ToSingle(buffer, 0), BitConverter.ToSingle(buffer, counter), BitConverter.ToSingle(buffer, 2*counter));
						ball.transform.position = posicion;

						posicion.Set(BitConverter.ToSingle(buffer, 3*counter), BitConverter.ToSingle(buffer, 4*counter), BitConverter.ToSingle(buffer, 5*counter));
						playerone.transform.position = posicion;
					
						//TELL BALL AND PLAYER ONE SCRIPT NOT TO DO ANYTHING
						ballScript.enabled = false;
						playeroneScript.enabled = false;
					}
					else{
						posicion.Set(BitConverter.ToSingle(buffer, 0), BitConverter.ToSingle(buffer, counter), BitConverter.ToSingle(buffer, 2*counter));
						playertwo.transform.position = posicion;

						//TELL PLAYER TWO SCRIPT NOT TO DO ANYTHING
						playertwoScript.enabled = false;

					}
				}
					break;
				case NetworkEventType.DisconnectEvent:
					break;
			}


			if (timePassed > waitTimePackage) {
				timePassed -= waitTimePackage;
				/*Envio de datos*/
				if (isFirstPlayer) {
					counter = sizeof(float);
					buffer = new byte[100];
					Array.Copy (BitConverter.GetBytes (ball.transform.position.x), 0, buffer, 0 * counter, sizeof(float));
					Array.Copy (BitConverter.GetBytes (ball.transform.position.y), 0,  buffer, 1 * counter, sizeof(float));
					Array.Copy (BitConverter.GetBytes (ball.transform.position.z), 0, buffer, 2 * counter, sizeof(float));
					Array.Copy (BitConverter.GetBytes (playerone.transform.position.x), 0, buffer, 3 * counter, sizeof(float));
					Array.Copy (BitConverter.GetBytes (playerone.transform.position.y), 0, buffer, 4 * counter, sizeof(float));
					Array.Copy (BitConverter.GetBytes (playerone.transform.position.z), 0, buffer, 5 * counter, sizeof(float));

					NetworkTransport.Send (hostId, connectionId, unreliableChannel, buffer, counter + 1, out error);
				} else {
					counter = sizeof(float);
					buffer = new byte[100];
					Array.Copy (BitConverter.GetBytes (playertwo.transform.position.x), 0, buffer, 0 * counter, sizeof(float));
					Array.Copy (BitConverter.GetBytes (playertwo.transform.position.y), 0, buffer, 1 * counter, sizeof(float));
					Array.Copy (BitConverter.GetBytes (playertwo.transform.position.z), 0, buffer, 2 * counter, sizeof(float));
					
					NetworkTransport.Send (hostId, connectionId, unreliableChannel, buffer, counter + 1, out error);
			
				}
			}
		}
	}


	bool isConnected(){
		NetworkEventType rec;
		bool res = false;

		rec = NetworkTransport.ReceiveFromHost(hostId, out connectionIdOut, out channelId, buffer, 100, out receivedSize, out error);

		if (recData == NetworkEventType.ConnectEvent) {
			res = true;	
		}

		return res;
	}
}




