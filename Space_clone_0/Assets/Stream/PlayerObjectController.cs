using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class PlayerObjectController : NetworkBehaviour
{
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong  PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    private CustomNetWorkManager manager;
    private CustomNetWorkManager Manager
    {
        get
        {
            if (manager != null) { return manager; }
            return manager = CustomNetWorkManager.singleton as CustomNetWorkManager;
        }
    }
    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        Debug.Log(11);
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer(); 
        LobbyController.Instance.UpdateLobbyName();

    }
    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();

    }
    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();

    }
    [Command]
    private void CmdSetPlayerName(string PlayName) {
      PlayerNameUpdate(this.PlayerName, PlayName);
    }
  public  void PlayerNameUpdate(string OldValue,string NewValue)
    {
        if (isServer)
        {
            this.PlayerName = NewValue;
        }
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }

    }
}
