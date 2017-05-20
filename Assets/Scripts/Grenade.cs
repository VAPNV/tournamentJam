using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Grenade : MonoBehaviour {

	public GameObject shrapnelPrefab;
	public Combat shooter;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

	}

	void OnDestroy() {
		for (int n = 0; n < 100; n++) {
			Vector3 dir = Random.insideUnitSphere + Vector3.up;
			GameObject shrapnel = (GameObject) Instantiate(shrapnelPrefab, transform.position + dir, Quaternion.identity);
			shrapnel.GetComponent<Bullet>().shooter = shooter;
			shrapnel.GetComponent<Rigidbody>().velocity = dir * 6;
			NetworkServer.Spawn(shrapnel);
			Destroy(shrapnel, 0.4f);
		}
	}
}
