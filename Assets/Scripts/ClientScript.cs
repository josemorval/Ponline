using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

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
	BallController ballScript;
	GameObject playerone;
	GameObject playertwo;
	PlayerController playeroneScript;
	PlayerController playertwoScript;


	bool isFirstPlayer;
	bool isSecondPlayer;

	byte[] buffer = new byte[100];
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
		connectionId = NetworkTransport.Connect(hostId, oponentIpAddress, oponentPort, 0, out error);

		if (error == 0) {
			connectionEstablished = true;
			isSecondPlayer = true;
			isFirstPlayer = false;
			playeroneScript.modo = PlayerController.playerMode.Online;
		}
		else{
			connectionEstablished = false;
			isSecondPlayer = false;
			isFirstPlayer = true;
			playertwoScript.modo = PlayerController.playerMode.Online;
		}

		/*Inicializaciones varias*/
		timePassed = waitTimeRetry;

		this.oponentIpAddress = oponentIpAddress;
		this.oponentPort = oponentPort;
		this.myPort = myPort;

		ball = GameObject.Find ("Ball");
		playerone = GameObject.Find ("Player1");
		playertwo = GameObject.Find ("Player2");
		ballScript = ball.GetComponent<BallController> ();
		posicion = new Vector3 ();

		ballActiveFlag = true;
		playeroneActiveFlag = true;
		playertwoActiveFlag = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!ballScript.enabled) {
			ballScript.enabled = true;
		}
		if (!playeroneScript.enabled) {
			playeroneScript.enabled = true;
		}
		if (!playertwoScript.enabled) {
			playertwoScript.enabled = true;
		}

		timePassed += Time.deltaTime;

		if (!connectionEstablished) {//Conexion con el oponente
			if (timePassed > waitTimeRetry) {
				connectionId = NetworkTransport.Connect (hostId, this.oponentIpAddress, this.oponentPort, 0, out error);
			
				if (error == 0) {
					connectionEstablished = true;
					/*Envio de la semilla para el random con ID 0 de mensaje al oponente*/
					buffer = BitConverter.GetBytes(randomSeed);
					NetworkTransport.Send (hostId, connectionId, reliableChannel, buffer, sizeof(int), out error);

					//START MATCH
					ballScript.enabled = true;
					ballScript.RestartMatch();
				}
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
					randomSeed = BitConverter.ToInt32(buffer, 0);
					ballScript.seedRandom = randomSeed;
					ballScript.updateSeed = true;
					//START MATCH 
					ballScript.enabled = true;
					ballScript.RestartMatch();
				}
				else{
					counter = sizeof(double);

					if(isSecondPlayer){
						posicion.Set(BitConverter.ToDouble(buffer, 0), BitConverter.ToDouble(buffer, counter), BitConverter.ToDouble(buffer, 2*counter));
						ball.transform.position = posicion;

						posicion.Set(BitConverter.ToDouble(buffer, 3*counter), BitConverter.ToDouble(buffer, 4*counter), BitConverter.ToDouble(buffer, 5*counter));
						playerone.transform.position = posicion;
					
						//TELL BALL AND PLAYER ONE SCRIPT NOT TO DO ANYTHING
						ballScript.enabled = false;
						playeroneScript.enabled = false;
					}
					else{
						posicion.Set(BitConverter.ToDouble(buffer, 0), BitConverter.ToDouble(buffer, counter), BitConverter.ToDouble(buffer, 2*counter));
						playertwo.transform.position = posicion;

						//TELL PLAYER TWO SCRIPT NOT TO DO ANYTHING
						playertwoScript.enabled = false;

					}
				}
					break;
				case NetworkEventType.DisconnectEvent:
					break;
			}


			/*Envio de datos*/
			if(isFirstPlayer){
				counter = 0;
				buffer = new byte[100];
				buffer = BitConverter.GetBytes(ball.transform.position.x);
				counter += sizeof(double);
				(buffer+counter) = BitConverter.GetBytes(ball.transform.position.y);
				counter += sizeof(double);
				(buffer+counter) = BitConverter.GetBytes(ball.transform.position.z);
				counter += sizeof(double);

				(buffer+counter) = BitConverter.GetBytes(playerone.transform.position.x);
				counter += sizeof(double);
				(buffer+counter) = BitConverter.GetBytes(playerone.transform.position.y);
				counter += sizeof(double);
				(buffer+counter) = BitConverter.GetBytes(playerone.transform.position.z);
				counter += sizeof(double);

				NetworkTransport.Send (hostId, connectionId, unreliableChannel, buffer, counter+1, error);
			}
			else{
				counter = 0;
				buffer = new byte[100];
				(buffer+counter) = BitConverter.GetBytes(playertwo.transform.position.x);
				counter += sizeof(double);
				(buffer+counter) = BitConverter.GetBytes(playertwo.transform.position.y);
				counter += sizeof(double);
				(buffer+counter) = BitConverter.GetBytes(playertwo.transform.position.z);
				counter += sizeof(double);
				
				NetworkTransport.Send (hostId, connectionId, unreliableChannel, buffer, counter+1, error);
			}

		}
	}
}




