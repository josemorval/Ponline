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
	#endregion


	#region private variables

		bool flag; //Para  detectar si ya se esta corriendo como host o cliente
		GameObject networkManager;

	#endregion

	// Use this for initialization
	void Start () {
		flag = false;
	}
	
	// Update is called once per frame
	void Update () {
		//Vemos si debemos instanciar algun network gameobject
		if (!flag) {
			if (Input.GetKey (serverKey)) {
				networkManager = Instantiate (server);
				networkManager.name = networkManager.name.Replace ("(Clone)", "");
				flag = true;
			} else if (Input.GetKey (clientKey)) {
				networkManager = Instantiate (client);
				networkManager.name = networkManager.name.Replace ("(Clone)", "");

				networkManager.GetComponent<ClientScript> ().StartComponent (ipAddress, serverPort);

				flag = true;
			}
		}

		//Salir del multijugador
		if (Input.GetKey (exitKey) && flag) {
			Destroy (networkManager);
			flag = false;
		}



	}
}
