using UnityEngine;
using System.Collections;

public class ScoreScript : MonoBehaviour {

	public float initialScale;
	public float maxScale;
	public AnimationCurve anim;
	public float maxTime;

	public void AddScore(int score){

		AnimAdd();
		GetComponent<TextMesh>().text = score.ToString();
		GetComponent<TextMesh>().text = score.ToString();
		transform.GetChild(0).GetComponent<TextMesh>().text = score.ToString();

	}

	public void AnimAdd(){
		StartCoroutine(AnimAddCoroutine());
	}

	IEnumerator AnimAddCoroutine(){

		float time = 0f;

		while(time<maxTime){

			transform.localScale = (initialScale + maxScale*anim.Evaluate(time/maxTime))*Vector3.one;

			time+=Time.deltaTime;
			yield return null;
		}

		transform.localScale  = initialScale*Vector3.one;

	}



}
