using UnityEngine;
using System.Collections;

public class GameManagerController : MonoBehaviour {

	#region public variables
		[Header("GameObjects de la network")]
		public GameObject server;
		public GameObject client;

		[Header("Botones para iniciar la network")]
		public KeyCode serverKey;
		public KeyCode clientKey;

		[Header("Boton para salir de la network")]
		public KeyCode exitKey;

		[Header("Parámetros conexión")]
		public string ipAddress;
		public int serverPort;
		public int myPort;

		[Header("Controllers de los objetos")]
		public BallController ballScript;
		public PlayerController playerone;
		public PlayerController playertwo;
	#endregion


	#region private variables

		bool flag; //Para  detectar si ya se esta corriendo como host o cliente
		GameObject networkManager;
		GameObject ball;


	#endregion

	// Use this for initialization
	void Start () {
		flag = false;
	}

	// Update is called once per frame
	void Update () {
		//Vemos si debemos instanciar algun network gameobject
		if (!flag) {
			if (Input.GetKey (clientKey)) {
				flag = true;
				networkManager = Instantiate (client);
				networkManager.name = networkManager.name.Replace ("(Clone)", "");

				networkManager.GetComponent<ClientScript> ().StartComponent (ipAddress, serverPort, myPort);

				/*Paramos el juego*/
				ballScript.RestartMatch();
				ballScript.enabled = false;


			}
		}

		//Salir del multijugador
		if (Input.GetKey (exitKey) && flag) {
			Destroy (networkManager);
			flag = false;
		}



	}

	public void pararJuego(){
		ballScript.RestartMatch();
		ballScript.enabled = false;
	}

	public void comenzarJuego(){
		ballScript.enabled = true;
	}

}
