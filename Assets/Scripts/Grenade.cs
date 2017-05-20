using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Grenade : NetworkBehaviour {

	public GameObject shrapnelPrefab;
	public Combat shooter;
	public int timer = 180;

	public AudioClip BoomSound;
	public GameObject SoundPlayerPrefab;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void FixedUpdate () {
			timer--;
			if (timer <= 0) {

				this.CmdPlaySoundHere ();
				Debug.Log ("SHOULD BOOM!");

				Destroy(gameObject);
				float radius = 15;
				float power = 200;
				Vector3 explosionPos = transform.position;
				Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
				foreach (Collider hit in colliders)
				{
					Rigidbody rb = hit.GetComponent<Rigidbody>();

					CharacterController controller = hit.GetComponent<CharacterController>();
					if (rb != null) {
						rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
					} else if (controller != null) {
						hit.SendMessage("KnockBack", new PlayerMove.KnockBackData(power, transform.position, radius, 1));
					}
				}



				for (int n = 0; n < 100; n++) {
					Vector3 dir = Random.insideUnitSphere + Vector3.up;
					GameObject shrapnel = (GameObject) Instantiate(shrapnelPrefab, transform.position + dir, Quaternion.identity);
					shrapnel.GetComponent<Bullet>().shooter = shooter;
					shrapnel.GetComponent<Rigidbody>().velocity = dir * 6;
					NetworkServer.Spawn(shrapnel);
					Destroy(shrapnel, 3);
				}
			}

	}
	[Command]

	void CmdPlaySoundHere()
	{
		//RpcPlayBoomHere();

		Debug.Log ("SOUNDING?!");


		GameObject Soundie = (GameObject)Instantiate (SoundPlayerPrefab, this.transform.position, this.transform.rotation);

		AudioSource SoundieSound = Soundie.GetComponent<AudioSource> ();
		SoundieSound.clip = BoomSound;

	}
//
//
//	[ClientRpc]
//
//	void RpcPlayBoomHere()
//	{
//
//		Debug.Log ("SOUNDING?!");
//
//
//		GameObject Soundie = (GameObject)Instantiate (SoundPlayerPrefab, this.transform.position, this.transform.rotation);
//
//		AudioSource SoundieSound = Soundie.GetComponent<AudioSource> ();
//		SoundieSound.clip = BoomSound;
//
//
//
//	}
}
