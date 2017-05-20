using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
    public float buildTime;
    private float buildTimeLeft;
	// Use this for initialization
	void Start () {
        if (!isServer)
            enabled = false;
        buildTimeLeft = buildTime;
	}
	
	// Update is called once per frame
	void Update () {
        if (buildTimeLeft < 0)
        {
            buildTimeLeft = 0;
            PlayerMove[] players = FindObjectsOfType<PlayerMove>();
            foreach (PlayerMove plr in players)
            {
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
