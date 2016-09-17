using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	#region public variables
		[Header("Transform de los jugadores")]
		public Transform playerone;
		public Transform playertwo;
		
		[Header("Controles para el jugador uno")]
		public KeyCode playeroneUp;
		public KeyCode playeroneDown;
		[Space(2)]

		[Header("Controles para el jugador dos")]
		public KeyCode playertwoUp;
		public KeyCode playertwoDown;
		[Space(2)]

		[Header("Direccion en la que se mueven los jugadores")]
		public float dirVelOne;
		public float dirVelTwo;

		[Space(2)]

		[Header("Velocidad maxima de las palas")]
		public float maxVel;

		[Space(2)]

		[Header("Anchura (la mitad) lógica del campo")]
		public float widthField;
	#endregion
	
	void Update () {
	
		MovePlayerOne();
		MovePlayerTwo();

	}

	void MovePlayerOne(){
		if(Input.GetKey(playeroneUp)){
			dirVelOne = 1f;
		}else if(Input.GetKey(playeroneDown)){
			dirVelOne = -1f;
		}else{
			dirVelOne = 0f;
		}

		Vector3 v = playerone.position;
		v = v + maxVel*dirVelOne*transform.up;
		playerone.position = v;

		if(v.y<-widthField){
			v.y = -widthField;
			playerone.position = v; 
			dirVelOne = 1f;
		}if(v.y>widthField){
			v.y = widthField;
			playerone.position = v; 
			dirVelOne = -1f;
		}
	}

	void MovePlayerTwo(){

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
		
	}
}
