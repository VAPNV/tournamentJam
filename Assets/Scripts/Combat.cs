using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Combat : NetworkBehaviour {

    public const int maxHealth = 100;
    public bool destroyOnDeath;

    // SyncVars are variables that are updated on the server and replicated on all clients
    [SyncVar]
    public int health = maxHealth;

    public void TakeDamage(int amount) {
        if (!isServer)
            return;

        health -= amount;
        if (health <= 0) {
            if (destroyOnDeath) {
                Destroy(gameObject);
            } else {
                health = maxHealth;

                // Called on the server, will be invoked on the clients
                RpcRespawn();
            }
        }
    }

    // ClientRpc calls are sent from objects on the server to objects on clients
    [ClientRpc]
    void RpcRespawn() {
        if (isLocalPlayer) {
            // Move back to zero location
            transform.position = Vector3.zero;
        }
    }
}
