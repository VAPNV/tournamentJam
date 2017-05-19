using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    void OnCollisionEnter(Collision collision) {
        var hit = collision.gameObject;
        var hitCombat = hit.GetComponent<Combat>();
        if (hitCombat != null) {
            //hitCombat.TakeDamage(10);

            // When the bullet on the server is destroyed, since it is a spawned object managed by the network, it will be destroyed on clients too
            Destroy(gameObject);
        }
    }
}
