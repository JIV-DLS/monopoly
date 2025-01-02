using UnityEngine;

public class PlayerPieceOnBoardBuilder : MonoBehaviour
{
    public PlayerElementOnMap Create(PlayerPieceEnum playerTypeToCreate, Transform parentTransform)
    {
        PlayerElementOnMap prefab = _getPrefab(playerTypeToCreate);
        PlayerElementOnMap createdObject = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation); 
        createdObject.transform.SetParent(parentTransform);
        createdObject.gameObject.SetActive(true);
        return createdObject;
    }
    private PlayerElementOnMap _getPrefab(PlayerPieceEnum playerTypeToCreate)
    {
        switch (playerTypeToCreate)
        {
            case PlayerPieceEnum.TopHat:
            default:
                return GetComponentInChildren<TopHatPlayerOnMap>(true);
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
