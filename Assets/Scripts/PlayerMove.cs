using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour {

	public MeshRenderer RobotModel;

    public float jumpVelocity;
    public float gravity;
    private MouseLook mouse;
    private Camera cam;
    private float gravVelocity;
    private CharacterController controller;

  public GameObject grenadePrefab;

	// INVENTORY
	private int WhatToBuild = 0;
  public GameObject[] tools = new GameObject[]{};
  public string[] toolActions = new string[]{};

	// AUDIO
	public GameObject SoundplayerPrefab;
	public AudioClip ItemSwichSound;
	public AudioClip ShovelDigSound;
	public AudioClip PickAxeDigSound;
	public AudioClip HammerActionSound;
	public AudioClip ConcreteActionSound;


	public AudioClip RifleShootSound;

	void Start(){
    foreach (GameObject tool in tools) {
		    tool.SetActive(false);
    }
    tools[WhatToBuild].SetActive(true);
	}

    // Use this for initialization for the local player object
    public override void OnStartLocalPlayer() {

		//this.RobotModel = GetComponent<MeshRenderer> ();

		if (GetComponent<Combat>().team == Combat.Team.Orange)
			RobotModel.material.color = Color.yellow;
		else if (GetComponent<Combat>().team == Combat.Team.Blue)
			RobotModel.material.color = Color.blue;

		cam = Camera.main;
        cam.transform.SetParent(transform);
        cam.transform.localPosition = Vector3.zero;
		cam.transform.localRotation = Quaternion.identity;
        mouse = new MouseLook();
		mouse.MaximumX = 43;	// so that vision does not clip with model + GAMEPLAY EFFECT
        mouse.Init(transform, cam.transform);
        controller = GetComponent<CharacterController>();

        foreach (GameObject tool in tools) {
    		    tool.transform.SetParent (cam.transform);
        }

		cam.transform.localPosition = new Vector3(0,1,0);	//head goes UP!

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
      if (Input.mouseScrollDelta.y < 0) {
    		this.CmdCycleWhatToBuild(1);
      } else if (Input.mouseScrollDelta.y > 0) {
        this.CmdCycleWhatToBuild(-1);
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
        // Debug.Log(gravVelocity);
        return Vector3.up * gravVelocity;
    }

    void Jump()
    {
        if (!OnGround())
            return;
        gravVelocity = jumpVelocity;
    }


  int mod(int x, int m) {
      int r = x%m;
      return r<0 ? r+m : r;
  }

	/// <summary>
	/// Plays single sound and kills itself. Note: allows multiple at the same time!!
	/// </summary>
	/// <param name="WhatToPlay">What to play.</param>


    // [Command] tells this will be called from client but invoked on server
    // Cmd-prefix in Command-methods is a common practice
    [Command]
    void CmdFire(Vector3 dir) {
        CmdShoot(dir);
    }

	///Cycles what can be build now. Short list for now! (once needs certain equipments??)
	void CmdCycleWhatToBuild(int dir)
	{
		Debug.Log(WhatToBuild);
		this.CmdPlaySoundHere (ItemSwichSound);
		tools[WhatToBuild].SetActive(false);
		WhatToBuild += dir;
		WhatToBuild = mod(WhatToBuild, tools.Length);
		tools[WhatToBuild].SetActive(true);
	}


	void CmdShoot(Vector3 dir)
	{
		RaycastHit hit;

		Debug.Log(toolActions[WhatToBuild]);
		if (toolActions[WhatToBuild] == "Rifle") {

			// TODO: ACTUAL SHOOTINGS!

			this.CmdPlaySoundHere (RifleShootSound);

		}
    else if (toolActions[WhatToBuild] == "Grenade") {
      GameObject grenade = (GameObject) Instantiate(grenadePrefab, tools[WhatToBuild].transform.position + dir, Quaternion.identity);
      grenade.GetComponent<Grenade>().shooter = GetComponent<Combat>();
      grenade.GetComponent<Rigidbody>().velocity = dir * 6;
      NetworkServer.Spawn(grenade);
      Destroy(grenade, 3.0f);
    }
		else if (Physics.Raycast (transform.position, dir, out hit, 4)) {
			Debug.DrawRay (hit.point, Vector3.up, Color.red, 3);




			if (hit.collider.GetComponentInParent<Grid> ())
			{
				//Playsound! *CHUNK*

				Grid GridThatWasHit = hit.collider.GetComponentInParent<Grid>();

				//Pickaxe GOES DOWN
				if (toolActions[WhatToBuild] == "PickAxe") {

					this.CmdPlaySoundHere (PickAxeDigSound);

					if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Trench_Low.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Trench_Deep);
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Trench_Low);
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground_Grass.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Trench_Low);

				}

				//Shovel goes  UP
				else if (toolActions[WhatToBuild] == "Shovel") {

					this.CmdPlaySoundHere (ShovelDigSound);

					if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Trench_Low.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Ground);
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Trench_Deep.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Trench_Low);
				}

				//Hammer Adds WOOD
				else if (toolActions[WhatToBuild] == "Hammer") {

					this.CmdPlaySoundHere (HammerActionSound);

					if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground_Grass.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Ground_WoodWall_NS);
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Ground_WoodWall_NS);
					
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground_WoodWall_NS.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Ground_WoodWall_WE);
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground_WoodWall_WE.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Ground);
				}

				//ConcreteCammer Adds Concreteblocks and Walls
				else if (toolActions[WhatToBuild] == "Concrete") {

					this.CmdPlaySoundHere (ConcreteActionSound);

					if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground_Grass.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Ground_ConcreteBlock);
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Ground_ConcreteBlock);
					
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground_ConcreteBlock.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Ground_ConcreteWall);
					
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Ground_ConcreteWall.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Ground);

					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Trench_Deep.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Trench_Deep_Concreteblock);
					
					else if (GridThatWasHit.WhatIam == GridThatWasHit.Mother.Trench_Deep_Concreteblock.name)
						GridThatWasHit.ChangeTo (GridThatWasHit.Mother.Trench_Deep);
				}



			}
		}
	}

	void CmdPlaySoundHere(AudioClip WhatToPlay)
	{
		GameObject Soundie = (GameObject)Instantiate (SoundplayerPrefab, this.transform.position, this.transform.rotation);

		AudioSource SoundieSound = Soundie.GetComponent<AudioSource> ();
		SoundieSound.clip = WhatToPlay;

	}
//
//    [ClientRpc]
//
//	/// <summary>
//	/// Plays single sound and kills itself. Note: allows multiple at the same time!!
//	/// </summary>
//	/// <param name="WhatToPlay">What to play.</param>
//	void RpcPlaySoundHere(AudioClip WhatToPlay)
//	{
//		GameObject Soundie = (GameObject)Instantiate (SoundplayerPrefab, this.transform.position, this.transform.rotation);
//
//		AudioSource SoundieSound = Soundie.GetComponent<AudioSource> ();
//		SoundieSound.clip = WhatToPlay;
//
//	}
}
