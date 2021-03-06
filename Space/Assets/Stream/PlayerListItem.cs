using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    private bool AvatarReceived;

    public Text PlayerNameText;
    public RawImage PlayerIcon;
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;
    private void Start()
    {
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(onAvatarImageLoaded);
    }
    public void SetPlayerValues() {
        PlayerNameText.text = PlayerName;
        if (!AvatarReceived)
        {
            GetPlayerIcon();

        }
    }
    void GetPlayerIcon() {
        int imageId = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if (imageId == -1) { return; }
        PlayerIcon.texture = GetSteamImageAsTexture(imageId);

    }
    private void onAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID != PlayerSteamID) { return; }

        PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
    
    }

    public Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];
            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));
            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived = true;
        return texture;

    }
}
