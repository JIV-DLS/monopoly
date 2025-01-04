using System;
using UnityEngine;


public abstract class PurchasableCard : MonoBehaviour, IClickableButtonHandler
{
    private ButtonHandler _flipButton;
    private PurchasableCard _targetPurchasableCard;
    private PurchasableTile _lastPurchasableTile;
    protected void Awake()
    {
        Init();
    }

    public void SetTargetPurchasableCard( PurchasableCard targetPurchasableCard)
    {
        SetTargetPurchasableCard(_lastPurchasableTile, targetPurchasableCard);
    }
    public void SetTargetPurchasableCard(PurchasableTile purchasableTile, PurchasableCard targetPurchasableCard)
    {
        // Prevent infinite recursion
        if (_targetPurchasableCard != null && _targetPurchasableCard == targetPurchasableCard)
            return;

        _targetPurchasableCard = targetPurchasableCard;
        _lastPurchasableTile = purchasableTile;
        // Set the current card as the target of the target card
        if (_targetPurchasableCard != null && _targetPurchasableCard._targetPurchasableCard != this)
        {
            _targetPurchasableCard.SetTargetPurchasableCard(purchasableTile, this);
        }
    }


    public void FlipCard()
    {
        gameObject.SetActive(false);
        _targetPurchasableCard.gameObject.SetActive(true);
    }

    public void OnClick()
    {
        FlipCard();
    }

    protected T  Clone<T> () where T : PurchasableCard
    {
        var clone = Instantiate(this, transform.parent).GetComponent<T>();
        clone.Init();
        return clone;
    }

    public abstract PurchasableCard Clone (PurchasableTile purchasableTile);

    public PurchasableCard Clone()
    {
        return Clone(_lastPurchasableTile);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    protected virtual void Init()
    {
        _flipButton = GetComponentInChildren<ButtonHandler>();
        if (_flipButton == null)
        {
            Debug.LogError("flipButton component not found");
        }
        _flipButton.Handler = this;
    }
}
public abstract class PurchasableFaceCard : PurchasableCard
{

    public void UpdateTile(PurchasableTile purchasableTile)
    {
        SetTargetPurchasableCard(purchasableTile, purchasableTile.GetBehindCard());
    }

}
public abstract class PurchasableBehindCard : PurchasableCard
{

    public void UpdateTile(PurchasableTile purchasableTile)
    {
        SetTargetPurchasableCard(purchasableTile, purchasableTile.GetFaceCard());
    }
}

    