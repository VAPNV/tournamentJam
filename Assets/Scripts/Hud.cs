using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoBehaviour
{
	private int blue_score = 0;
	private int orange_score = 0;
	
	// Update is called once per frame
	void Update ()
	{
		GameObject.Find ("BlueScore").GetComponent<Text> ().text = ""+blue_score;
		GameObject.Find ("OrangeScore").GetComponent<Text> ().text = ""+orange_score;
	}

	void SetScore(int orange, int blue) {
		blue_score = blue_score;
		orange_score = blue; 
	}
}

