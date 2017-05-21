using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mine : NetworkBehaviour {
	public GameObject shrapnelPrefab;
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
		float power = 300;
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			CharacterController controller = hit.GetComponent<CharacterController>();
			if (rb != null) {
				rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
			} else if (controller != null) {
				hit.SendMessage("KnockBack", new PlayerMove.KnockBackData(power, transform.position - new Vector3(0, -0.5f, 0), radius, 1.5f));
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
		for (int n = 0; n < 10; n++) {
			Vector3 dir = Random.insideUnitSphere + Vector3.up;
			GameObject shrapnel = (GameObject) Instantiate(shrapnelPrefab, transform.position + dir, Quaternion.identity);
			shrapnel.GetComponent<Bullet>().shooter = owner;
			shrapnel.GetComponent<Rigidbody>().velocity = dir * 6;
			NetworkServer.Spawn(shrapnel);
			Destroy(shrapnel, 4);
		}
	}

	[Command]
	void CmdPlaySoundHere() {
		GameObject Soundie = (GameObject)Instantiate (SoundPlayerPrefab, this.transform.position, this.transform.rotation);
		AudioSource SoundieSound = Soundie.GetComponent<AudioSource> ();
		SoundieSound.clip = BoomSound;
	}
}
