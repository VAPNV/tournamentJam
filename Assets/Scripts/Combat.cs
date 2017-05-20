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
        if (PlayerPrefs.GetInt("team") == 0)
            team = Team.Orange;
        if (PlayerPrefs.GetInt("team") == 1)
            team = Team.Blue;
    }

    public void Update()
	{
		if (Ammo<100)
			Ammo++;


		
		if (isLocalPlayer)
        {
            GameObject.Find("hp").GetComponent<Slider>().value = (float)health / (float)maxHealth;
			GameObject.Find ("AmmoText").GetComponent<Text> ().text = "AMMO: " + Ammo/15;
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
        NetworkStartPosition[] spawns = FindObjectsOfType<NetworkStartPosition>();
        NetworkStartPosition spawn = spawns[Random.Range(0, spawns.Length - 1)];
        transform.position = spawn.transform.position;
        transform.rotation = spawn.transform.rotation;
    }
}
