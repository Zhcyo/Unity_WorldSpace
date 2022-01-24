using DapperDino.Mirror.Tutorials.Lobby;
using DapperDino.Tutorials.Lobby;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Steamworks;

public class CustomNetWorkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();
    public override void OnServerAddPlayer(NetworkConnection conn)
    {

        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count+1;
            GamePlayerInstance.PlayerSteamID=(ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyId, GamePlayers.Count);
            NetworkServer.AddPlayerForConnection(conn, GamePlayerPrefab.gameObject);
            Debug.Log(GamePlayerInstance.PlayerSteamID);

        }
        }
}
