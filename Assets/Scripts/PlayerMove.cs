using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour {

    public GameObject bulletPrefab;
    public float jumpVelocity;
    public float gravity;
    private MouseLook mouse;
    private Camera cam;
    private float gravVelocity;

    // Use this for initialization for the local player object
    public override void OnStartLocalPlayer() {
        GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.5f, 0.2f);
        cam = Camera.main;
        cam.transform.SetParent(transform);
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;
        mouse = new MouseLook();
        mouse.Init(transform, cam.transform);
    }

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer) {
            return;
        }
        Vector3 move = Input.GetAxis("Vertical") * Vector3.forward + Input.GetAxis("Horizontal") * Vector3.right;

        transform.Translate(move * 0.1f);

        if (Input.GetMouseButtonDown(0)) {
            CmdFire();
        }

        Gravity();
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        mouse.LookRotation(transform, cam.transform);
        mouse.UpdateCursorLock();
    }

    void Gravity()
    {
        transform.Translate(Vector3.up * gravVelocity);
        gravVelocity -= gravity;
        if (transform.position.y <= 0)
        {
            gravVelocity = 0;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    void Jump()
    {
        if (transform.position.y > 0)
            return;
        gravVelocity = jumpVelocity;
        Debug.Log("jump");
    }

    // [Command] tells this will be called from client but invoked on server
    // Cmd-prefix in Command-methods is a common practice
    [Command]
    void CmdFire() {
        // Create the bullet object locally
        GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.position + cam.transform.forward, Quaternion.identity);

        // Make the bullet move away in front of the player
        bullet.GetComponent<Rigidbody>().velocity = cam.transform.forward * 4;

        // Spawn the bullet on the clients
        NetworkServer.Spawn(bullet);

        // When the bullet is destroyed on the server it will automaticaly be destroyed on clients
        Destroy(bullet, 2.0f);
    }
}
