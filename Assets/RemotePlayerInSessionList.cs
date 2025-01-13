using UnityEngine;

public class RemotePlayerInSessionList : MonoBehaviour
{
    public PlayerImageChooser playerImageChooser;
    public BaseTextHandler playerNameText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetImageFromIndex(int playerCustomProperty)
    {
        playerImageChooser.SetImageFromIndex(playerCustomProperty);
    }

    public void SetPlayerInfo(string playerNickName, int playerCustomProperty)
    {
        playerNameText.SetText(playerNickName);
        SetImageFromIndex(playerCustomProperty);
    }
}
