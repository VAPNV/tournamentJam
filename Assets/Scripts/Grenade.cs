using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Grenade : MonoBehaviour {

	public GameObject shrapnelPrefab;
	public Combat shooter;
	public int timer = 180;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void FixedUpdate () {
			timer--;
			if (timer <= 0) {
				Destroy(gameObject);
				float radius = 100;
				float power = 200;
				Vector3 explosionPos = transform.position;
				Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
				foreach (Collider hit in colliders)
				{
					Rigidbody rb = hit.GetComponent<Rigidbody>();

					if (rb != null) {
						rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
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
}
