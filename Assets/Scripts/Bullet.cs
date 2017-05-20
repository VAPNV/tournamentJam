using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public int damage;
    public float radius;
    public Combat shooter;

    void Update () {
  		Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
      foreach (Collider c in hitColliders) {
        var hit = c.gameObject;
        var hitCombat = hit.GetComponent<Combat>();
        if (hitCombat != null) {
          hitCombat.TakeDamage(damage, shooter);
          Destroy(gameObject);
        }
      }
    }

    // void OnCollisionEnter(Collision collision) {
    //     var hit = collision.gameObject;
    //     var hitCombat = hit.GetComponent<Combat>();
    //     if (hitCombat != null) {
    //       hitCombat.TakeDamage(damage, shooter);
    //
    //       // When the bullet on the server is destroyed, since it is a spawned object managed by the network, it will be destroyed on clients too
    //       Destroy(gameObject);
    //     }
    // }
}
