using UnityEngine;

public class PlayerContent : MonoBehaviour
{
    public void UpdateTile(BoardTile tile)
    {
        placeValue.setTile(tile);
        
    }

    public void updateMoney(int money)
    {
        MoneyValue.SetText(money.ToString());
    }
    public void EnableBuyAction(int amountOfBuy)
    {
        placeButtonAction.SetButtonText($"Acheter {amountOfBuy}");
        placeButtonAction.SetButtonInteractable(true);
    }
    public void EnableRollDiceAction()
    {
        throwDices.SetButtonInteractable(true);
    }
    public void DisableBuyAction()
    {
        placeButtonAction.SetButtonText("Acheter");
        placeButtonAction.SetButtonInteractable(false);
    }
    public void DisableAllActionButtons()
    {
        placeButtonAction.SetButtonInteractable(false);
        throwDices.SetButtonInteractable(false);
    }
    public void DisableRollDiceAction()
    {
        throwDices.SetButtonInteractable(false);
    }
    public void SetSelfMadePlayer(SelfmadePlayer selfmadePlayer)
    {
        this.selfmadePlayer = selfmadePlayer;
        thrownEqualDicesValue.SetSelfMadePlayer(selfmadePlayer);
    }
    private SelfmadePlayer selfmadePlayer;
    public void SetDicesRolled(int rollResult, bool allEqual)
    {
        throwResultValue.SetText(rollResult.ToString());
        if (allEqual)
        {
            thrownEqualDicesValue.Happened();
        }
    }
    private PlayerContentCardsScroll CardScroll;
    private SoldValue MoneyValue;
    private PrisonValue prisonValue;
    private ThrowResultValue throwResultValue;
    private ThrownEqualDicesValue thrownEqualDicesValue;
    private PlaceValue placeValue;
    private PlaceButtonAction placeButtonAction;
    private ThrowDices throwDices;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CardScroll = GetComponentInChildren<PlayerContentCardsScroll>();
        MoneyValue = GetComponentInChildren<SoldValue>();
        prisonValue = GetComponentInChildren<PrisonValue>();
        throwResultValue = GetComponentInChildren<ThrowResultValue>();
        thrownEqualDicesValue = GetComponentInChildren<ThrownEqualDicesValue>();
        placeValue = GetComponentInChildren<PlaceValue>();
        placeButtonAction = gameObject.GetComponentInChildren<PlaceButtonAction>();
        throwDices = gameObject.GetComponentInChildren<ThrowDices>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
