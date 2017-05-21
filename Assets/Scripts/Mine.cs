using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mine : NetworkBehaviour {
	public Combat owner;
	public int damage;
	public float toArm = 0.7f;

	public AudioClip BoomSound;
	public GameObject SoundPlayerPrefab;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (toArm < 0) {
			Collider[] hitColliders = Physics.OverlapSphere(transform.position + new Vector3(0, 0.5f, 0), 0.5f);
			foreach (Collider c in hitColliders) {
				var hit = c.gameObject;
				if (hit != gameObject) {
					var hitCombat = hit.GetComponent<Combat>();
					if (hitCombat != null) {
						this.CmdPlaySoundHere ();
						this.CmdExplode();
					}
				}
			}
		} else {
			toArm -= Time.deltaTime;
		}
	}

	[Command]
	public void CmdExplode() {
		Destroy(gameObject);
		float radius = 2;
		float power = 400;
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			CharacterController controller = hit.GetComponent<CharacterController>();
			if (rb != null) {
				rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
			} else if (controller != null) {
				hit.SendMessage("KnockBack", new PlayerMove.KnockBackData(power, transform.position, radius, 0.5f));
			}
			var hitCombat = hit.GetComponent<Combat>();
			if (hitCombat != null) {
				hitCombat.TakeDamage(damage, owner);
			}
			if (hit.transform.GetComponentInParent<Grid>()) {
				Grid GridThatWasHit = hit.GetComponent<Collider>().GetComponentInParent<Grid>();

				if (GridThatWasHit.Damage (damage * 2)) {
					FindObjectsOfType<PlayerMove>()[0].RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, GridThatWasHit.BecomeThisAfterDeath.name);
				}
			}
		}
	}

	[Command]
	void CmdPlaySoundHere() {
		GameObject Soundie = (GameObject)Instantiate (SoundPlayerPrefab, this.transform.position, this.transform.rotation);
		AudioSource SoundieSound = Soundie.GetComponent<AudioSource> ();
		SoundieSound.clip = BoomSound;
	}
}
