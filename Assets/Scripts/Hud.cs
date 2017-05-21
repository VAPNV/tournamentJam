using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoBehaviour
{
	public int blue_score = 0;
	public int orange_score = 0;
	
	// Update is called once per frame
	void Update ()
	{
		GameObject.Find ("BlueScore").GetComponent<Text> ().text = ""+blue_score;
		GameObject.Find ("OrangeScore").GetComponent<Text> ().text = ""+orange_score;
	}

	public void SetScore(int orange, int blue) {
		blue_score = blue;
		orange_score = blue; 
	}
}

