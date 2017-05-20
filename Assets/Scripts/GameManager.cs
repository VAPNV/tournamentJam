using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {
    public float buildTime;
    [SyncVar]
    private float buildTimeLeft;
	// Use this for initialization
	void Start () {
        buildTimeLeft = buildTime;
	}
	
	// Update is called once per frame
	void Update () {
        GameObject.Find("Announcement").GetComponent<Text>().text = "";
        if (buildTimeLeft > 0)
        {
            GameObject.Find("Announcement").GetComponent<Text>().text = "Prepare your trenches";
            if (buildTimeLeft <= 4)
                GameObject.Find("Announcement").GetComponent<Text>().text = "Fighting in " + (int)buildTimeLeft;
        }
        if (!isServer)
            return;
        if (buildTimeLeft < 0)
        {
            buildTimeLeft = 0;
            PlayerMove[] players = FindObjectsOfType<PlayerMove>();
            foreach (PlayerMove plr in players)
            {
                if (plr.debug)
                    continue;
                plr.fighting = true;
                plr.WhatToBuild = mod(plr.WhatToBuild, plr.fighting ? plr.fightTools.Length : plr.buildTools.Length); ;
            }
        }
        else
            buildTimeLeft -= Time.deltaTime;
	}

    int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

}
