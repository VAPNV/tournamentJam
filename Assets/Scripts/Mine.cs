using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour {
	public Combat owner;
	public int damage;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1);
		foreach (Collider c in hitColliders) {
			var hit = c.gameObject;
			if (hit != gameObject) {
				var hitCombat = hit.GetComponent<Combat>();
				if (hitCombat != null) {
					hitCombat.TakeDamage(damage, owner);
					Destroy(gameObject);
				}
			}
		}
	}
}
