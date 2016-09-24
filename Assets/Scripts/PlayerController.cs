using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour {

	#region public variables
		[Header("Transform de los jugadores")]
		public Transform player;
		//public Transform playertwo;
		
		[Header("Controles para el jugador uno")]
		public KeyCode playerUp;
		public KeyCode playerDown;
		[Space(2)]

		[Header("Controles para el jugador dos")]
		//public KeyCode playertwoUp;
		//public KeyCode playertwoDown;
		[Space(2)]

		[Header("Direccion en la que se mueven los jugadores")]
		public float dirVel;
		//public float dirVelTwo;

		[Space(2)]

		[Header("Velocidad maxima de las palas")]
		public float maxVel;

		[Space(2)]

		[Header("Anchura (la mitad) lógica del campo")]
		public float widthField;
	#endregion

	void Start(){
		if (transform.position.x < 0) { /*Super dirty*/
			this.name = "Player1";
			playerUp = KeyCode.A;
			playerDown = KeyCode.Z;
		} else {
			this.name = "Player2";
			playerUp = KeyCode.UpArrow;
			playerDown = KeyCode.DownArrow;
		}

		player = this.transform;
	}

	void Update () {

		if (isLocalPlayer) {
			MovePlayer ();
		}

	}

	void MovePlayer(){
		if(Input.GetKey(playerUp)){
			dirVel = 1f;
		}else if(Input.GetKey(playerDown)){
			dirVel = -1f;
		}else{
			dirVel = 0f;
		}

		Vector3 v = player.position;
		v = v + maxVel*dirVel*transform.up;
		player.position = v;

		if(v.y<-widthField){
			v.y = -widthField;
			player.position = v; 
			dirVel = 1f;
		}if(v.y>widthField){
			v.y = widthField;
			player.position = v; 
			dirVel = -1f;
		}
	}

	/*void MovePlayerTwo(){

		if(Input.GetKey(playertwoUp)){
			dirVelTwo = 1f;
		}else if(Input.GetKey(playertwoDown)){
			dirVelTwo = -1f;
		}else{
			dirVelTwo = 0f;
		}

		Vector3 v = playertwo.position;
		v = v + maxVel*dirVelTwo*transform.up;
		playertwo.position = v;

		if(v.y<-widthField){
			v.y = -widthField;
			playertwo.position = v; 
			dirVelTwo = 1f;
		}if(v.y>widthField){
			v.y = widthField;
			playertwo.position = v; 
			dirVelTwo = -1f;
		}
		
	}*/
}
