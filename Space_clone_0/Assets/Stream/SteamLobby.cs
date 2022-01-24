using Mirror;
using Steamworks;
using UnityEngine;
    public class SteamLobby : Singleton<SteamLobby>
    {
       

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;
    public ulong CurrentLobbyId;
        private const string HostAddressKey = "HostAddress";

        private NetworkManager networkManager;

        public static CSteamID LobbyId { get; private set; }

        private void Start()
        {
            networkManager = GetComponent<NetworkManager>();

            if (!SteamManager.Initialized) { return; }

            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        public void HostLobby()
        {
         

            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
               
                return;
            }

            LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

            networkManager.StartHost();

            SteamMatchmaking.SetLobbyData(
                LobbyId,
                HostAddressKey,
                SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(
                LobbyId,
                "name",
                SteamFriends.GetPersonaName().ToString()+"S LOBBY");
        }

        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            if (NetworkServer.active) { return; }
        CurrentLobbyId = callback.m_ulSteamIDLobby;
            string hostAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HostAddressKey);

            networkManager.networkAddress = hostAddress;
            networkManager.StartClient();

            
        }
    }

