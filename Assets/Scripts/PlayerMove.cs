﻿using System.Collections;
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
    private CharacterController controller;


	// INVENTORY
	private string WhatToBuild = "Shovel";
	public GameObject Shovel;
	public GameObject PickAxe;
	public GameObject Rifle;

	// AUDIO
	public GameObject SoundplayerPrefab;
	public AudioClip ItemSwichSound;
	public AudioClip ShovelDigSound;
	public AudioClip PickAxeDigSound;
	public AudioClip RifleShootSound;

	void Start(){
		PickAxe.SetActive (false);
		Rifle.SetActive (false);
	}

    // Use this for initialization for the local player object
    public override void OnStartLocalPlayer() {
        GetComponent<MeshRenderer>().material.color = new Color(0.2f, 0.5f, 0.2f);
        cam = Camera.main;
        cam.transform.SetParent(transform);
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;
        mouse = new MouseLook();
        mouse.Init(transform, cam.transform);
        controller = GetComponent<CharacterController>();

		Shovel.transform.SetParent (cam.transform);
		PickAxe.transform.SetParent (cam.transform);
		Rifle.transform.SetParent (cam.transform);

    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!isLocalPlayer) {
            return;
        }
        Vector3 move = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;

        controller.Move(move * 0.1f + Gravity());
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            CmdFire(cam.transform.forward);
        }
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
		if (Input.GetMouseButtonDown(2))
		{
			this.CycleWhatToBuild ();
		}

        mouse.LookRotation(transform, cam.transform);
        mouse.UpdateCursorLock();
    }

    bool OnGround()
    {
        RaycastHit hit;
        return Physics.SphereCast(transform.position, 0.5f, Vector3.down, out hit, 1.25f + 0.1f);
    }

    Vector3 Gravity()
    {
        if (OnGround() && gravVelocity <= 0)
        {
            gravVelocity = 0;
        }
        else
        {
            gravVelocity -= gravity;
        }
        Debug.Log(gravVelocity);
        return Vector3.up * gravVelocity;
    }

    void Jump()
    {
        if (!OnGround())
            return;
        gravVelocity = jumpVelocity;
    }

	///Cycles what can be build now. Short list for now! (once needs certain equipments??)
	/// SHOULD THIS BE COMMAND TO SERveR?? (soo it looks to all the same??)
	void CycleWhatToBuild()
	{
		this.PlaySoundHere (ItemSwichSound);

		if (WhatToBuild == "Shovel") {
			WhatToBuild = "PickAxe";
			PickAxe.SetActive (true);
			Shovel.SetActive (false);
		}
		else if (WhatToBuild == "PickAxe"){
			WhatToBuild = "Rifle";
			Rifle.SetActive (true);
			PickAxe.SetActive (false);
		}
		else if (WhatToBuild == "Rifle"){
			WhatToBuild = "Shovel";
			Shovel.SetActive (true);
			Rifle.SetActive (false);
		}
	}

	/// <summary>
	/// Plays single sound and kills itself. Note: allows multiple at the same time!!
	/// </summary>
	/// <param name="WhatToPlay">What to play.</param>
	void PlaySoundHere(AudioClip WhatToPlay)
	{
		GameObject Soundie = (GameObject)Instantiate (SoundplayerPrefab, this.transform.position, this.transform.rotation);

		AudioSource SoundieSound = Soundie.GetComponent<AudioSource> ();
		SoundieSound.clip = WhatToPlay;
	
	}

    // [Command] tells this will be called from client but invoked on server
    // Cmd-prefix in Command-methods is a common practice
    [Command]
    void CmdFire(Vector3 dir) {
        // Create the bullet object locally
        //GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.position + cam.transform.forward, Quaternion.identity);

        // Make the bullet move away in front of the player
        //bullet.GetComponent<Rigidbody>().velocity = cam.transform.forward * 4;

        // Spawn the bullet on the clients
        //NetworkServer.Spawn(bullet);

        // When the bullet is destroyed on the server it will automaticaly be destroyed on clients
        //Destroy(bullet, 2.0f);
        RpcShoot(dir);
    }

    [ClientRpc]
    void RpcShoot(Vector3 dir)
    {
        RaycastHit hit;

		if (WhatToBuild == "Rifle") {

			// TODO: ACTUAL SHOOTINGS!

			this.PlaySoundHere (RifleShootSound);

		}
		else if (Physics.Raycast (transform.position, dir, out hit, 4)) {
			Debug.DrawRay (hit.point, Vector3.up, Color.red, 3);




			if (hit.collider.GetComponentInParent<Grid> ()) 
			{
				//Playsound! *CHUNK*

				Grid GridThatWasHit = hit.collider.GetComponentInParent<Grid>();

				//Pickaxe GOES DOWN
				if (WhatToBuild == "PickAxe") {

					this.PlaySoundHere (PickAxeDigSound);

					if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Trench_Low.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Trench_Deep);
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Trench_Low);
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground_Grass.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Trench_Low);

				}

				//Shovel goes  UP
				else if (WhatToBuild == "Shovel") {
					
					this.PlaySoundHere (ShovelDigSound);

					if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Trench_Low.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Ground);
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Trench_Deep.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Trench_Low);
				}


				
			}
		}
    }
}
