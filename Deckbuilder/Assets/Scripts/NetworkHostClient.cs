using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkHostClient : MonoBehaviour {

    public string natServerHost;
    public ushort natServerPort;
    public InputField ipAddress = null;
    public GameObject networkManager = null;
    private NetworkManager mgr;

    private void Start() {
        Rpc.MainThreadRunner = MainThreadManager.Instance;
    }

    public void Host() {
        UDPServer server = new UDPServer(2);
        server.Connect("127.0.0.1", (ushort)15937, natServerHost, natServerPort);

        server.playerTimeout += (player, sender) => {
            Debug.Log("Player " + player.NetworkId + " timed out");
        };

        server.playerAccepted += (player, sender) => {
            Debug.Log("Player " + player.NetworkId + " joined");
        };

        server.playerRejected += (player, sender) => {
            Debug.Log("Player " + player.NetworkId + " tried to join, rejected");
        };

        server.playerDisconnected += (player, sender) => {
            Debug.Log("Player " + player.NetworkId + " disconnected");
        };
        Connected(server);
    }

    public void Connect() {
        UDPClient client = new UDPClient();
        client.Connect(ipAddress.text, (ushort)15937, natServerHost, natServerPort);
        client.disconnected += (sender) => {
            Debug.Log("Disconnected");
        };
        Connected(client);
    }

    private void Connected(NetWorker networker) {
        if (mgr == null) {
            mgr = Instantiate(networkManager).GetComponent<NetworkManager>();
        }

        mgr.Initialize(networker);
        if (networker is IServer)
            SceneManager.LoadScene("NetworkGameBoard");
    }
}
