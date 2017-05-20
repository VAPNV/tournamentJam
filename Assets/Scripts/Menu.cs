using UnityEngine;
using UnityEngine.Networking;

public class Menu : MonoBehaviour {
	private NetworkManager manager;
	// Use this for initialization
	void Start () {
		manager = GetComponent<NetworkManager>();
        PlayerPrefs.SetInt("team", 0);
    }

	public void StartServer()
	{
		manager.StartHost();
	}

	public void JoinServer()
	{
		if (manager.networkAddress.Trim() == "")
			manager.networkAddress = "localhost";
		manager.StartClient();
	}

	public void SetServerIp(string serverIp)
	{
		manager.networkAddress = serverIp;
	}

    public void SetTeam(int team)
    {
        PlayerPrefs.SetInt("team", team);
    }


}