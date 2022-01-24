using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using System;
using System.Linq;

public class LobbyController : Singleton<LobbyController>
{
    public Text LobbyNameText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItempRefab;
    public GameObject LocalPlayerObject;


    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalPlayerController;
    private CustomNetWorkManager manager;
    private CustomNetWorkManager Manager
    {
        get
        {
            if (manager != null) { return manager; }
            return manager = CustomNetWorkManager.singleton as CustomNetWorkManager;
        }
    }
    public void UpdateLobbyName() {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyId;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");

    }

    public void UpdatePlayerList() {
        if (!PlayerItemCreated)
        {
            CreateHostPlayerItem();
        }
        if (playerListItems.Count < Manager.GamePlayers.Count)
        {
            CreateClientPlayerItem();

        }
        if (playerListItems.Count > Manager.GamePlayers.Count)
        {
            RemovePlayerItem();

        }
        if (playerListItems.Count == Manager.GamePlayers.Count)
        {
            UpdatePlayerItem();

        }

    }
    public void FindLocalPlayer() {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");

        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }
    private void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController  player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItempRefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();
            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.SetPlayerValues();
            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;
            playerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }
        private void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!playerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItempRefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();
                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.SetPlayerValues();
                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;
                playerListItems.Add(NewPlayerItemScript);

            }

        }

    }


    public void UpdatePlayerItem() {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListItemScript in playerListItems)
            {
                if (PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.SetPlayerValues();
                }
            }
        }

}
public void RemovePlayerItem() {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();
        foreach (PlayerListItem playerlistItem in playerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionID == playerlistItem.ConnectionID))
            { playerListItemToRemove.Add(playerlistItem); }
        }
        if (playerListItemToRemove.Count > 0)
        {
            foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                GameObject objectToRemove = playerlistItemToRemove.gameObject;
                playerListItems.Remove(playerlistItemToRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }
}
