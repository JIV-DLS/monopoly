using System;
using Photon.Pun;
using UnityEngine;

public class PlayerPieceOnBoardBuilder : MonoBehaviour
{
    public PlayerElementOnMap Create(string playerPieceName, Transform parentTransform)
    {
        Enum.TryParse(playerPieceName, out PlayerPieceEnum playerPiece);
        return Create(playerPiece, parentTransform);
    }
    public PlayerElementOnMap Create(PlayerPieceEnum playerTypeToCreate, Transform parentTransform)
    {
        //PlayerElementOnMap prefab = _getPrefab(playerTypeToCreate);
        //prefab.SetNetworkInstance();
        PlayerElementOnMap createdObject = PhotonNetwork.Instantiate(playerTypeToCreate.ToString(), transform.position, transform.rotation).GetComponent<PlayerElementOnMap>(); 
        createdObject.transform.SetParent(parentTransform);
        createdObject.gameObject.SetActive(true);
        createdObject.transform.rotation = Quaternion.Euler(85.75034f, 0.05139616f, 2.250318e-08f);
        return createdObject;
    }
    private PlayerElementOnMap _getPrefab(PlayerPieceEnum playerTypeToCreate)
    {
        switch (playerTypeToCreate)
        {
            case PlayerPieceEnum.BattleShip:
                return ChildUtility.GetChildComponentByName<PlayerElementOnMap>(transform, "BattleShip");
            case PlayerPieceEnum.TopHat:
            default:
                return ChildUtility.GetChildComponentByName<PlayerElementOnMap>(transform, "TopHat");
        }
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
