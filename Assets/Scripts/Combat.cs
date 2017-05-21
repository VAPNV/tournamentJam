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
	public Team team;

    public override void OnStartLocalPlayer()
    {
		if (PlayerPrefs.GetInt ("team") == 0) {
			team = Team.Orange;
			GameObject.Find ("TeamColor").transform.GetChild(0).GetComponent<Image>().color = new Color(1.000f, 0.549f, 0.000f);
		}
		if (PlayerPrefs.GetInt ("team") == 1) {
			team = Team.Blue;
			GameObject.Find ("TeamColor").transform.GetChild(0).GetComponent<Image>().color = new Color(0.000f, 0.000f, 1.000f);
		}
    }



    public void Update()
	{
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

				GetComponent<PlayerMove> ().grenadesLeft = 2;

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
