using UnityEngine;

public class PlayerSummaryButton : ButtonHandler
{
    private PlayerResumeName _playerResumeName;
    private PlayerResumeMoney _playerResumeMoney;
    
    protected override void Awake()
    {
        base.Awake();
        _playerResumeName = GetComponentInChildren<PlayerResumeName>();
        if (_playerResumeName == null)
        {
            Debug.LogError("Player resume name component not found as a child of the Button!");
        }
        _playerResumeMoney = GetComponentInChildren<PlayerResumeMoney>();
        if (_playerResumeMoney == null)
        {
            Debug.LogError("Player resume money component not found as a child of the Button!");
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

    public void UpdatePlayer(MonopolyPlayer selfMadePlayer)
    {
        _playerResumeName.SetText(selfMadePlayer.name);
        _playerResumeMoney.SetText(selfMadePlayer.money.ToString());
    }

    public void setPlayer(MonopolyPlayer monopolyPlayerToSet)
    {
        monopolyPlayer = monopolyPlayerToSet;
        Refresh();
    }

    public void Refresh()
    {
        _playerResumeName.SetText(monopolyPlayer.name);
        _playerResumeMoney.SetText(monopolyPlayer.money.ToString());
    }

    public MonopolyPlayer monopolyPlayer { get; private set; }
}
