using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour {

    public GameObject bulletPrefab;

    // Use this for initialization for the local player object
    public override void OnStartLocalPlayer() {
        GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.5f, 0.2f);
    }

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer) {
            return;
        }
        float x = Input.GetAxis("Horizontal") * 0.1f;
        float z = Input.GetAxis("Vertical") * 0.1f;

        transform.Translate(x, 0, z);

        if (Input.GetKeyDown(KeyCode.Space)) {
            CmdFire();
        }
    }

    // [Command] tells this will be called from client but invoked on server
    // Cmd-prefix in Command-methods is a common practice
    [Command]
    void CmdFire() {
        // Create the bullet object locally
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.position - transform.forward, Quaternion.identity);

        // Make the bullet move away in front of the player
        bullet.GetComponent<Rigidbody>().velocity = -transform.forward * 4;

        // Spawn the bullet on the clients
        NetworkServer.Spawn(bullet);

        // When the bullet is destroyed on the server it will automaticaly be destroyed on clients
        Destroy(bullet, 2.0f);
    }
}
