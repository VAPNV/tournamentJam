using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerMove : NetworkBehaviour {
	public class KnockBackData {
		public float power;
		public float radius;
		public Vector3 source;
		public float time;
		public KnockBackData(float power, Vector3 source, float radius, float time) {
			this.power = power;
		 	this.source = source;
			this.radius = radius;
			this.time = time;
		}
	}

	public MeshRenderer RobotModel;
	public GameObject AllTheRobotParts;
	public MeshRenderer RifleModel;

    public float jumpVelocity;
    public float gravity;
    private MouseLook mouse;
    private Camera cam;
    private float gravVelocity;
    private CharacterController controller;
    [SyncVar]
    private float hold = 0;
		[SyncVar]
    private bool leftButtonHeld = false;
    [SyncVar]
    public bool debug = false;

  	public GameObject grenadePrefab;
		private ArrayList knockbacks = new ArrayList();

	// INVENTORY
    [SyncVar]
	public int WhatToBuild = 0;
    [SyncVar]
    public bool fighting = false;
    public GameObject[] buildTools = new GameObject[]{};
    public GameObject[] fightTools = new GameObject[] { };
    public string[] toolActions = new string[]{};

	public int RifleDamage = 30;

    [SyncVar]
    public int grenadesLeft;
    [SyncVar]
    public int hammerLeft;
    [SyncVar]
    public int concreteLeft;

    // AUDIO
    public GameObject SoundplayerPrefab;
	public AudioClip ItemSwichSound;
	public AudioClip ShovelDigSound;
	public AudioClip PickAxeDigSound;
	public AudioClip HammerActionSound;
	public AudioClip ConcreteActionSound;

	public GameObject HitDustCloud;
	public GameObject ShootingEffect;
	public GameObject RifleBarrelEnd;

	public AudioClip RifleShootSound;
	public AudioClip RifleEmptySound;


	void Start(){
        foreach (GameObject tool in buildTools) {
		        tool.SetActive(false);
        }
        foreach (GameObject tool in fightTools)
        {
            tool.SetActive(false);
        }
        buildTools[WhatToBuild].SetActive(true);
	}

    // Use this for initialization for the local player object
    public override void OnStartLocalPlayer() {

		//this.RobotModel = GetComponent<MeshRenderer> ();

		cam = Camera.main;
        cam.transform.SetParent(transform);
        cam.transform.localPosition = Vector3.zero;
		cam.transform.localRotation = Quaternion.identity;
        mouse = new MouseLook();
		//mouse.MaximumX = 43;	// so that vision does not clip with model + GAMEPLAY EFFECT
        mouse.Init(transform, cam.transform);
        controller = GetComponent<CharacterController>();

        foreach (GameObject tool in buildTools) {
    		    tool.transform.SetParent (cam.transform);
        }
        foreach (GameObject tool in fightTools)
        {
            tool.transform.SetParent(cam.transform);
        }

		foreach (MeshRenderer RobotModelPart in AllTheRobotParts.transform.GetComponentsInChildren<MeshRenderer>())
		{
			RobotModelPart.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		}

        cam.transform.localPosition = new Vector3(0,1,0);	//head goes UP!

    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!isLocalPlayer) {
            return;
        }
		GameObject.Find ("GrenadeSlider").GetComponent<Slider> ().value = hold / 0.75f;
        Vector3 move = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
				Vector3 knockback = new Vector3(0, 0, 0);

				ArrayList toBeRemoved = new ArrayList();
				foreach (KnockBackData data in knockbacks) {
					data.time -= Time.fixedDeltaTime;
					if (data.time < 0) {
						toBeRemoved.Add(data);
					}
					Vector3 deflect = Vector3.Normalize(transform.position - data.source);
					float pow = (data.power / 500) * (1 - (Vector3.Distance(transform.position, data.source) / data.radius));
					knockback += deflect * pow;
				}
				foreach (KnockBackData data in toBeRemoved) {
					knockbacks.Remove(data);
				}
        controller.Move(move * 0.1f + knockback + Gravity());
    }

    void Update()
    {
		if (GetComponent<Combat> ().team == Combat.Team.Orange) {
			RobotModel.material.color = Color.yellow;
			RifleModel.material.color = Color.yellow;
		} else if (GetComponent<Combat> ().team == Combat.Team.Blue) {
			RobotModel.material.color = Color.blue;
			RifleModel.material.color = Color.blue;
		}
        foreach (GameObject tool in buildTools)
        {
            tool.SetActive(false);
        }
        foreach (GameObject tool in fightTools)
        {
            tool.SetActive(false);
        }
        if (fighting)
            fightTools[WhatToBuild].SetActive(true);
        else
            buildTools[WhatToBuild].SetActive(true);
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
					CmdCycleDebug();
        }
        if (leftButtonHeld) {
          hold += Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(0))
        {
		    if (GetToolAction() == "Grenade" && grenadesLeft > 0) {
            leftButtonHeld = true;
          } else {
				CmdFire(cam.transform.position, cam.transform.forward, cam.transform.forward + new Vector3(( Random.Range(-0.02f,0.02f)) , (Random.Range(-0.02f,0.02f)),(Random.Range(-0.02f,0.02f)) ) );
					}
				} else if (Input.GetMouseButtonUp(0)) {
          if (hold > 0) {
            if (GetToolAction() == "Grenade" && grenadesLeft > 0) {
              CmdThrowGrenade(cam.transform.position, cam.transform.forward, hold);
                    grenadesLeft--;
                    if (grenadesLeft <= 0)
                        CmdCycleWhatToBuild(1);
            }
            leftButtonHeld = false;
            hold = 0;
          }
				}
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
      if (Input.mouseScrollDelta.y < 0) {
    		this.CmdCycleWhatToBuild(1);
            this.CmdPlaySoundHere(SoundType.ItemSwitch);
        }
        else if (Input.mouseScrollDelta.y > 0) {
            this.CmdCycleWhatToBuild(-1);
            this.CmdPlaySoundHere(SoundType.ItemSwitch);
        }

        mouse.LookRotation(transform, cam.transform);
        mouse.UpdateCursorLock();
    }

		void KnockBack(KnockBackData arg) {
			knockbacks.Add(arg);
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

	[Command]
	void CmdCycleDebug() {
		fighting = !fighting;
		WhatToBuild = 0;
		debug = true;
	}

	/// <summary>
	/// Plays single sound and kills itself. Note: allows multiple at the same time!!
	/// </summary>
	/// <param name="WhatToPlay">What to play.</param>


    // [Command] tells this will be called from client but invoked on server
    // Cmd-prefix in Command-methods is a common practice
    [Command]
    void CmdFire(Vector3 pos, Vector3 forward,Vector3 dir) {
        CmdShoot(pos, forward, dir);
    }

    [Command]
    void CmdThrowGrenade(Vector3 pos, Vector3 dir, float hold) {
			//fightTools[WhatToBuild].transform.position
      GameObject grenade = (GameObject) Instantiate(grenadePrefab, pos + dir, Quaternion.identity);
      grenade.GetComponent<Grenade>().shooter = GetComponent<Combat>();
      grenade.GetComponent<Rigidbody>().velocity = dir * Mathf.Clamp(hold, 0, 0.75f) / 0.75f * 16;
      NetworkServer.Spawn(grenade);
    }

    ///Cycles what can be build now. Short list for now! (once needs certain equipments??)
    [Command]
    void CmdCycleWhatToBuild(int dir)
	{
        WhatToBuild += dir;
        WhatToBuild = mod(WhatToBuild, fighting ? fightTools.Length : buildTools.Length);
    }

    string GetToolAction()
    {
        if (fighting)
            return toolActions[WhatToBuild + buildTools.Length];
        return toolActions[WhatToBuild];
    }

    [Command]
    void CmdShoot(Vector3 pos, Vector3 forward, Vector3 dir)
	{
		RaycastHit hit;

		Debug.Log(GetToolAction());
		if (GetToolAction() == "Rifle" && (this.GetComponent<Combat>().Ammo > 15)) {

			this.GetComponent<Combat> ().Ammo = this.GetComponent<Combat> ().Ammo - 15;

            RpcShootEffect();
			if (Physics.Raycast (pos + forward, dir, out hit))
            {
                RpcDustEffect(hit.point);

                if (hit.transform.tag == "Player")
                {
					hit.transform.GetComponent<Combat>().TakeDamage(RifleDamage, GetComponent<Combat>());
                }

				else if (hit.transform.GetComponentInParent<Grid>())
				{
					Grid GridThatWasHit = hit.collider.GetComponentInParent<Grid>();

					if (GridThatWasHit.Damage (RifleDamage)) {
						RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, GridThatWasHit.BecomeThisAfterDeath.name);
					}
				}
            }

			this.CmdPlaySoundHere (SoundType.RifleShoot);

		}
		//no amo click click

		else if (GetToolAction() == "Rifle" && (this.GetComponent<Combat>().Ammo < 15)) {

			this.CmdPlaySoundHere (SoundType.RifleEmptySound);

		}
			else if (Physics.Raycast (pos + forward, dir, out hit, 4)) {
				Debug.DrawRay (hit.point, Vector3.up, Color.red, 3);




			if (hit.collider.GetComponentInParent<Grid> ())
			{
				//Playsound! *CHUNK*

				Grid GridThatWasHit = hit.collider.GetComponentInParent<Grid>();

				//Pickaxe GOES DOWN
				if (GetToolAction() == "PickAxe") {

					this.CmdPlaySoundHere (SoundType.PickAxeDig);

                    if (GridThatWasHit.WhatIam == "Trench_Low")
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Trench_Deep");
					else if (GridThatWasHit.WhatIam == "Ground_Grass")
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Trench_Low");
                    else if (GridThatWasHit.WhatIam == "Ground_Muddy")
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Trench_Low");

                }

                //Shovel goes  UP
                else if (GetToolAction() == "Shovel" && GridThatWasHit.isOccupied(RobotModel) == false) {

					this.CmdPlaySoundHere (SoundType.ShovelDig);

					if (GridThatWasHit.WhatIam == "Trench_Low")
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Ground_Muddy");
                    else if (GridThatWasHit.WhatIam == "Trench_Deep")
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Trench_Low");
                }

				//Hammer Adds WOOD
				else if (GetToolAction() == "Hammer") {

					this.CmdPlaySoundHere (SoundType.HammerAction);

                    if (GridThatWasHit.WhatIam == "Ground_Grass" && hammerLeft > 0)
                    {
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Ground_Wall");
                        hammerLeft--;
                    }
                    else if (GridThatWasHit.WhatIam == "Ground_Muddy" && hammerLeft > 0)
                    {
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Ground_Wall");
                        hammerLeft--;
                    }

                    else if (GridThatWasHit.WhatIam == "Ground_Wall")
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Ground_Wall_ROT");

                    else if (GridThatWasHit.WhatIam == "Ground_Wall_ROT")
                    {
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Ground_Muddy");
                        hammerLeft++;
                    }

                }

				//ConcreteCammer Adds Concreteblocks and Walls
				else if (GetToolAction() == "Concrete") {

					this.CmdPlaySoundHere (SoundType.ConcreteAction);


					if (GridThatWasHit.WhatIam == "Ground_Grass" && concreteLeft > 0)
                    {
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Ground_ConcreteCube");
                        concreteLeft--;
                    }
                    else if (GridThatWasHit.WhatIam == "Ground_Muddy" && concreteLeft > 0)
                    {
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Ground_ConcreteCube");
                        concreteLeft--;
                    }


                    else if (GridThatWasHit.WhatIam == "Ground_ConcreteCube")
						RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Ground_ConcreteWall");

					else if (GridThatWasHit.WhatIam == "Ground_ConcreteWall")
                    {
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Ground_Muddy");
                        concreteLeft++;
                    }



                    else if (GridThatWasHit.WhatIam == "Trench_Deep" && concreteLeft > 0)
                    {
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Trench_Deep_ConcreteBlock");
                        concreteLeft--;
                    }
                    else if (GridThatWasHit.WhatIam == "Trench_Deep_ConcreteBlock")
                    {
                        RpcGridChanged(GridThatWasHit.x, GridThatWasHit.y, "Trench_Deep");
                        concreteLeft++;
                    }
                }




            }
		}
	}

    [ClientRpc]
    void RpcShootEffect()
    {
        GameObject ShootEffect = (GameObject)Instantiate(ShootingEffect, RifleBarrelEnd.transform.position, this.transform.rotation);
    }

    [ClientRpc]
    void RpcDustEffect(Vector3 point)
    {
        GameObject DustCloud = (GameObject)Instantiate(HitDustCloud, point, new Quaternion(0, 0, 0, 0));
    }

    [Command]
	void CmdPlaySoundHere(SoundType WhatToPlay)
	{
        RpcPlaySoundHere(WhatToPlay);
	}

    [ClientRpc]
    public void RpcGridChanged(int x, int y, string gridType)
    {
        Grid place = null;
        foreach (Grid grid in FindObjectsOfType<Grid>())
            if (grid.x == x && grid.y == y)
                place = grid;
        if (place == null)
            return;

		Debug.Log ("Changing GRID " + place.x  + "-" + place.y  + " of type " + gridType);

        place.ChangeTo(place.Mother.GetElementByName(gridType));
    }


	public enum SoundType { ItemSwitch, ShovelDig, PickAxeDig, HammerAction, ConcreteAction, RifleShoot , RifleEmptySound};
    [ClientRpc]

	/// <summary>
	/// Plays single sound and kills itself. Note: allows multiple at the same time!!
	/// </summary>
	/// <param name="WhatToPlay">What to play.</param>
	void RpcPlaySoundHere(SoundType WhatToPlay)
	{
		AudioClip[] clips = { ItemSwichSound, ShovelDigSound, PickAxeDigSound, HammerActionSound, ConcreteActionSound, RifleShootSound, RifleEmptySound};
        GameObject Soundie = (GameObject)Instantiate (SoundplayerPrefab, this.transform.position, this.transform.rotation);

		AudioSource SoundieSound = Soundie.GetComponent<AudioSource> ();
		SoundieSound.clip = clips[(int)WhatToPlay];

	}
}
