using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BallSpawnerScript : NetworkBehaviour {

	[Header("Ball Gameobject")]
	public GameObject ballPrefab;

	public override void OnStartServer(){
		GameObject ball;
		Vector3 original = new Vector3 (0, 0, 0);
		ball = (GameObject)Instantiate (ballPrefab, original, Quaternion.identity);
		NetworkServer.Spawn (ball);
	}

	void Start(){
	}

	void Update(){
	}
}
