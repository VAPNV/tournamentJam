using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Chceks attached particlesystem
/// </summary>
public class ShortDurationParticleAnimator : MonoBehaviour {

	public ParticleSystem OurJob;

	// Use this for initialization
	void Start () {
		OurJob = this.transform.GetComponent<ParticleSystem> ();
		OurJob.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if (OurJob.isPlaying == false)
			Destroy (this.gameObject);
	}
}
