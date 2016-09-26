using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : MonoBehaviour {

	#region public variables
		[Header("Transform de los jugadores")]
		public Transform player;
		
		[Header("Transform de la pelota")]
		public Transform ball;

		[Header("Controles para el jugador uno")]
		public KeyCode playerUp;
		public KeyCode playerDown;
		[Space(2)]

		[Header("Direccion en la que se mueven los jugadores")]
		public float dirVel;

		[Space(2)]

		[Header("Velocidad maxima de las palas")]
		public float maxVel;

		[Space(2)]

		[Header("Anchura (la mitad) lógica del campo")]
		public float widthField;

		[Header("Fuente de comandos del jugador")]
		public playerMode modo;

	#endregion

	void Start(){

		player = this.transform;
	}

	void Update () {

		if (modo == playerMode.Local) {
			MovePlayer ();
		} else if (modo == playerMode.IA) {
			MovePlayerIA ();
		} else if (modo == playerMode.Online) {
			MovePlayerIA();
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

	void MovePlayerIA(){
		if (ball.position.y > player.position.y) {
			dirVel = 1f;
		} else if (ball.position.y < player.position.y){
			dirVel = -1f;
		}
		else{
			dirVel = 0f;
		}

		Vector3 v = player.position;
		v = v + maxVel*dirVel*transform.up*(2*Mathf.Abs(player.position.y-ball.position.y)/(2*widthField));
	
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

	public enum playerMode{Local=1, IA=2, Online=3};
	
}
