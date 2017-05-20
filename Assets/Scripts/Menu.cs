using UnityEngine;
using UnityEngine.Networking;

public class Menu : MonoBehaviour {
	private NetworkManager manager;
	// Use this for initialization
	void Start () {
		manager = GetComponent<NetworkManager>();
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


}