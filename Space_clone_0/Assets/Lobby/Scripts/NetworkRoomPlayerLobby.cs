using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

    public class NetworkRoomPlayerLobby : NetworkBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject lobbyUI = null;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
        [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
        [SerializeField] private RawImage[] playerImages = new RawImage[4];
        [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleSteamIdUpdated))]
    private ulong steamId;

    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool IsReady = false;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

        private bool isLeader;
        public bool IsLeader
        {
            set
            {
                isLeader = value;
                startGameButton.gameObject.SetActive(value);
            }
        }

        private NetworkManagerLobby room;
        private NetworkManagerLobby Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
        }
        public void SetSteamId(ulong steamI) {

        this.steamId = steamI;
   
    }
        public override void OnStartAuthority()
        {
           

            lobbyUI.SetActive(true);
        }

        public override void OnStartClient()
        {
            Room.RoomPlayers.Add(this);
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(onAvatarImageLoaded);
            UpdateDisplay();
        }

        public override void OnStopClient()
        {
            Room.RoomPlayers.Remove(this);

            UpdateDisplay();
        }
    private void HandleSteamIdUpdated(ulong oldSteamId, ulong newSteamId) => UpdateDisplay();


    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
      

        private void UpdateDisplay()
        {

        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
            var cSteamId = new CSteamID(steamId);

            playerNameTexts[i].text = SteamFriends.GetFriendPersonaName(cSteamId);
            int imageId = SteamFriends.GetLargeFriendAvatar(cSteamId);
            if (imageId == -1) { return; }
            playerImages[i].texture = Room.RoomPlayers[i].GetSteamImageAsTexture(imageId);

        }
    }
    public Texture2D GetSteamImageAsTexture(int iImage) {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(iImage,out uint width,out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];
            isValid = SteamUtils.GetImageRGBA(iImage,image,(int)(width * height * 4));
            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        return texture; 

    }
    private void onAvatarImageLoaded(AvatarImageLoaded_t callback) {
        if (callback.m_steamID.m_SteamID != steamId) { return; }
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerImages[i].texture = GetSteamImageAsTexture(callback.m_iImage);
        }
    }
        public void HandleReadyToStart(bool readyToStart)
        {
            if (!isLeader) { return; }

            startGameButton.interactable = readyToStart;
        }

       

        [Command]
        public void CmdReadyUp()
        {
            IsReady = !IsReady;

            Room.NotifyPlayersOfReadyState();
        }

        [Command]
        public void CmdStartGame()
        {
            if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

            Room.StartGame();
        }
    }

