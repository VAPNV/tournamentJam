using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Combat : NetworkBehaviour {

    public const int maxHealth = 100;

    public bool destroyOnDeath;
	public enum Team
	{
		Orange, Blue, None,
	}

	[SyncVar]
	public int Ammo = 100;

	[SyncVar]
	public int score = 0;
    // SyncVars are variables that are updated on the server and replicated on all clients
    [SyncVar]
    public int health = maxHealth;

	[SyncVar]
	public Team team = Team.None;

    public void Start()
    {
        Combat[] players = FindObjectsOfType<Combat>();
        int oranges = 0;
        int blues = 0;
        foreach (Combat plr in players)
        {
            if (plr.team == Team.Blue)
                blues++;
            if (plr.team == Team.Orange)
                oranges++;
        }
        if (oranges < blues && team == Team.None)
            this.team = Team.Orange;
        if (blues < oranges && team == Team.None)
            this.team = Team.Blue;
        if (blues == oranges && team == Team.None)
            this.team = Team.Orange;

        if (isLocalPlayer)
        {
            this.transform.SetParent(GameObject.Find("GameManager").transform);
            if (this.team == Team.Blue)
                this.name = "Blue-" + Random.Range(100, 999);
            else if (GetComponent<Combat>().team == Combat.Team.Orange)
                this.name = "Orange-" + Random.Range(100, 999);

            GameObject.Find("NameText").GetComponent<Text>().text = this.name;
        }
    }

    public override void OnStartLocalPlayer()
    {
    }



    public void Update()
	{
        if (isLocalPlayer)
        {
            if (team == Team.Orange)
            {
                GameObject.Find("TeamColor").transform.GetChild(0).GetComponent<Image>().color = new Color(1.000f, 0.549f, 0.000f);
            }
            if (team == Team.Blue)
            {
                GameObject.Find("TeamColor").transform.GetChild(0).GetComponent<Image>().color = new Color(0.000f, 0.000f, 1.000f);
            }
        }

        if (Ammo<100)
			Ammo++;

		if (isLocalPlayer)
        {
            GameObject.Find("hp").GetComponent<Slider>().value = (float)health / (float)maxHealth;
            PlayerMove plr = GetComponent<PlayerMove>();

            //Debug.Log(plr.grenadesLeft);
            GameObject.Find("AmmoText").GetComponent<Text>().text = "";
            if (plr.GetToolAction() == "Rifle")
			    GameObject.Find ("AmmoText").GetComponent<Text> ().text = "RIFLE AMMO: " + Ammo/15;
            else if (plr.GetToolAction() == "Grenade")
                GameObject.Find("AmmoText").GetComponent<Text>().text = "GRENADES: " + plr.grenadesLeft;
            else if (plr.GetToolAction() == "Concrete")
                GameObject.Find("AmmoText").GetComponent<Text>().text = "CONCRETE: " + plr.concreteLeft;
            else if (plr.GetToolAction() == "Hammer")
                GameObject.Find("AmmoText").GetComponent<Text>().text = "WOOD: " + plr.hammerLeft;
			else if (plr.GetToolAction() == "Mine")
				GameObject.Find("AmmoText").GetComponent<Text>().text = "MINES: " + plr.minesLeft;
        }
    }

	public void TakeDamage(int amount, Combat shooter) {
        if (!isServer)
            return;
//		if (team != shooter.team)
//		{
			health -= amount;
//		}

        if (health <= 0) {
            if (GetComponent<PlayerMove>().playerIsHoldingFlag)
            {
                GetComponent<PlayerMove>().RpcplayerDropFlag(transform.position);
            }
			GetComponentInParent<GameManager> ().AnnounceMessage (shooter.name + " killed " + this.name + "!");

			if (shooter == this || shooter.team == this.team)
				this.GetScore (-1);
			else
				shooter.GetScore (1);

            if (destroyOnDeath) {
                Destroy(gameObject);
            } else {
                health = maxHealth;

                // Called on the server, will be invoked on the clients
                RpcRespawn();
            }
        }
    }

	public void GetScore(int amount) {
		this.score += amount;

		if (this.team == Team.Blue)
			GetComponentInParent<GameManager> ().Score_Blue += amount;
		else if (this.team == Team.Orange)
			GetComponentInParent<GameManager> ().Score_Orange += amount;




	}

    // ClientRpc calls are sent from objects on the server to objects on clients
    [ClientRpc]
    public void RpcRespawn() {
      PlayerMove p = GetComponent<PlayerMove>();
      if (p != null) {
        p.knockbacks.Clear();
      }

        // Move back to zero location
        NetworkStartPosition[] spawns = FindObjectsOfType<NetworkStartPosition>();
        NetworkStartPosition spawn = spawns[Random.Range(0, spawns.Length - 1)];
        transform.position = spawn.transform.position;
        transform.rotation = spawn.transform.rotation;
    }
}
