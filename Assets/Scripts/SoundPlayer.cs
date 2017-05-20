using UnityEngine;
using System.Collections;

/// <summary>
/// Just plays one sound at one lokation and then kills itself
/// </summary>
public class SoundPlayer : MonoBehaviour {

	public AudioSource ReasonWhyWeAreHere;
	
	void Start () {
	
		ReasonWhyWeAreHere.Play ();

	}
	
	// Update is called once per frame
	void Update () {
	

		if (!ReasonWhyWeAreHere.isPlaying)
			Destroy (this.gameObject);

	}
}
