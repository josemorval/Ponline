using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {


	#region public variables
		[Header("Controlador de los jugadores")]
		public PlayerController playerController;

		[Space(2)]

		[Header("Parametros de la bola")]
		public float initialVel;
		public float maxVel;
		public float maxDistance;
		public float initialHeight;
		public float factorVel;

		[Space(2)]

		[Header("Controlador de la puntuacion")]
		public ScoreScript scoreP1;
		public ScoreScript scoreP2;
	#endregion


	#region private variables
		Vector3 dir;
		Vector3 vel;
		float currentVel;
		int[] randomDistributionHeight = new int[]{
			1,1,1,1,1,1,1,1,2,2,2,2,4,4,4,6,6,8,8,10
		};
		int scoreP1val;
		int scoreP2val;
		float currentHeight;

	#endregion

	void Start(){
		RestartMatch();
	}

	void Update () {

		vel = dir*1f/(1f+factorVel*currentHeight*currentHeight);

		Vector3 v = transform.position;
		v = v + vel*Time.deltaTime;

		Vector3 vLocal = transform.GetChild(0).localPosition;
		vLocal.y = 0.55f-currentHeight*(v.x-maxDistance)*(v.x+maxDistance);
		transform.GetChild(0).localPosition = vLocal;

		transform.position = v;

		if(v.y>5.5f){
			v.y = 5.5f;
			transform.position = v;
			dir.y = -dir.y;

		}else if(v.y<-5.5f){
			v.y = -5.5f;
			transform.position = v;
			dir.y = -dir.y;

		}

		if(v.x>maxDistance){

			if(Mathf.Abs(transform.position.y-playerController.playertwo.position.y)<1.3f){
				v.x = maxDistance;
				transform.position = v;
				dir.x = -dir.x;

				if(playerController.dirVelTwo>0f && dir.normalized.y<0.5f){
					dir.y += 8f*playerController.dirVelTwo;
				}else if(playerController.dirVelTwo<0f && dir.normalized.y>-0.5f){
					dir.y += 8f*playerController.dirVelTwo;
				}else{
					dir.y += Random.Range(-1f,1f);
				}

				currentHeight = initialHeight*randomDistributionHeight[Random.Range(0,randomDistributionHeight.Length)];
				currentVel*=1.01f;
				currentVel = Mathf.Clamp(currentVel,0f,maxVel);
				dir = currentVel*dir.normalized;
			}else{
				scoreP1.AddScore(++scoreP1val);
				RestartMatch();
			}

		}else if(v.x<-maxDistance){
			
			if(Mathf.Abs(transform.position.y-playerController.playerone.position.y)<1.3f){
				v.x = -maxDistance;
				transform.position = v;
				dir.x = -dir.x;

				if(playerController.dirVelOne>0f && dir.normalized.y<0.5f){
					dir.y += 8f*playerController.dirVelOne;
				}else if(playerController.dirVelOne<0f && dir.normalized.y>-0.5f){
					dir.y += 8f*playerController.dirVelOne;
				}else{
					dir.y += Random.Range(-1f,1f);
				}

				currentHeight = initialHeight*randomDistributionHeight[Random.Range(0,randomDistributionHeight.Length)];
				currentVel*=1.01f;
				currentVel = Mathf.Clamp(currentVel,0f,maxVel);
				dir = currentVel*dir.normalized;

			}else{
				scoreP2.AddScore(++scoreP2val);
				RestartMatch();
			}

		}
			
			
	}

	public void RestartMatch(){
		currentVel = initialVel;
		dir = currentVel*new Vector3(1f,0f,0f);
		vel = dir*1f/(1f+factorVel*currentHeight*currentHeight);
		Vector3 v = transform.position;
		v.x = 0f;
		v.y = 0f;
		transform.position = v;
	}
}
