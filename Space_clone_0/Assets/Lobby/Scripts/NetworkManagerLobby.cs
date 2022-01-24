using DapperDino.Tutorials.Lobby;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

    public class NetworkManagerLobby : NetworkManager
    {
        [SerializeField] private int minPlayers = 2;
         [SerializeField] private string menuScene = string.Empty;

        [Header("Room")]
        [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

        [Header("Game")]
        [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
        [SerializeField] private GameObject playerSpawnSystem = null;
   

        private MapHandler mapHandler;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action<NetworkConnection> OnServerReadied;
        public static event Action OnServerStopped;

        public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
        public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby>();

        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        public override void OnStartClient()
        {
            var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

            foreach (var prefab in spawnablePrefabs)
            {
                NetworkClient.RegisterPrefab(prefab);
            }
        }
        public override void OnClientConnect()
        {
            base.OnClientConnect();

            OnClientConnected?.Invoke();
        }
       
      
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();

            OnClientDisconnected?.Invoke();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }

            if (SceneManager.GetActiveScene().name != menuScene)
            {
                conn.Disconnect();
                return;
            }
        }




        public override void OnServerAddPlayer(NetworkConnection conn)
        {

        //if (SceneManager.GetActiveScene().name == menuScene)
        //{

        //    NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab.gameObject).GetComponent<NetworkRoomPlayerLobby>();
        //    bool isLeader = RoomPlayers.Count == 0;
        //    roomPlayerInstance.IsLeader = isLeader;
        //    CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.LobbyId, numPlayers - 1);
        //    roomPlayerInstance.SetSteamId(steamId.m_SteamID);
        //    NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        //    Debug.Log(steamId);
        //}
        if (SceneManager.GetActiveScene().name == menuScene)
        {

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab.gameObject).GetComponent<NetworkRoomPlayerLobby>();
            bool isLeader = RoomPlayers.Count == 0;
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.LobbyId, numPlayers - 1);
            var playerInfoDisplay = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            playerInfoDisplay.SetSteamId(steamId.m_SteamID);
         
        }
    }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

                RoomPlayers.Remove(player);

                NotifyPlayersOfReadyState();
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            OnServerStopped?.Invoke();

            RoomPlayers.Clear();
            GamePlayers.Clear();
        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (var player in RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        private bool IsReadyToStart()
        {
            if (numPlayers < minPlayers) { return false; }

            foreach (var player in RoomPlayers)
            {
                if (!player.IsReady) { return false; }
            }

            return true;
        }

        public void StartGame()
        {
            if (SceneManager.GetActiveScene().name == menuScene)
            {
                if (!IsReadyToStart()) { return; }

          
                ServerChangeScene("Scene_Map_01");

            }
        }

        public override void ServerChangeScene(string newSceneName)
        {
            // From menu to game
            if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("Scene_Map"))
            {
                for (int i = RoomPlayers.Count - 1; i >= 0; i--)
                {
                    var conn = RoomPlayers[i].connectionToClient;
                    var gameplayerInstance = Instantiate(gamePlayerPrefab);
                   

                    NetworkServer.Destroy(conn.identity.gameObject);

                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
                }
            }

            base.ServerChangeScene(newSceneName);
        }
        public override void OnServerSceneChanged(string sceneName)
        {
            if(sceneName.StartsWith("Scene_Map"))
            {
                GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
                NetworkServer.Spawn(playerSpawnSystemInstance);
                //GameObject roundSystemInstance = Instantiate(roundSystem);
                //NetworkServer.Spawn(roundSystemInstance);
            }
        }
      

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);

            OnServerReadied?.Invoke(conn);
        }
    }

