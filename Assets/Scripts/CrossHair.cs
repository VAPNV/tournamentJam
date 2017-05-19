using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour {
	public Texture2D crosshairTexture;
	public bool isOn = true;
	private Rect position;

	void Start () {
		float x = (Screen.width - crosshairTexture.width) / 2;
		float y = (Screen.height - crosshairTexture.height) / 2;
		position = new Rect(x, y, crosshairTexture.width, crosshairTexture.height);
	}

  public void OnGUI()
  {
		if (isOn) {
    	GUI.DrawTexture(position, crosshairTexture);
		}
  }

	void Update () {

	}
}
