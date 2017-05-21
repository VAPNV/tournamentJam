﻿using System.Collections;
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

    public void Start()
    {
        if (isServer)
            RpcRespawn();
    }

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
			    GameObject.Find ("AmmoText").GetComponent<Text> ().text = "AMMO: " + Ammo/15;
            else if (plr.GetToolAction() == "Grenade")
                GameObject.Find("AmmoText").GetComponent<Text>().text = "AMMO: " + plr.grenadesLeft;
            else if (plr.GetToolAction() == "Concrete")
                GameObject.Find("AmmoText").GetComponent<Text>().text = "CONCRETE: " + plr.concreteLeft;
            else if (plr.GetToolAction() == "Hammer")
                GameObject.Find("AmmoText").GetComponent<Text>().text = "WOOD: " + plr.hammerLeft;
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
			shooter.score += 1;
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
        // Move back to zero location
        TeamSpawn[] spawns = FindObjectsOfType<TeamSpawn>();
        TeamSpawn spawn = spawns[Random.Range(0, spawns.Length - 1)];
        while (spawn.Team != team)
            spawn = spawns[Random.Range(0, spawns.Length - 1)];
        transform.position = spawn.transform.position;
        transform.rotation = spawn.transform.rotation;
    }
}
