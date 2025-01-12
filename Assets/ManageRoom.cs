using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ManageRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomToCreateName;
    public TMP_InputField roomToJoinName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void CreateRoom()
    {
        
        // Set the player's name
        PhotonNetwork.LocalPlayer.NickName = PlayerPieceEnum.TopHat.ToString();
        PhotonNetwork.JoinOrCreateRoom(roomToCreateName.text, new RoomOptions(){MaxPlayers = 8}, TypedLobby.Default, null);
    }

    public void JoinRoom()
    {
        
        // Set the player's name
        PhotonNetwork.LocalPlayer.NickName = PlayerPieceEnum.BattleShip.ToString();
        PhotonNetwork.JoinRoom(roomToJoinName.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MonopolyMainScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
