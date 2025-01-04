using System;
using UnityEngine;


public abstract class PurchasableCard : MonoBehaviour, IClickableButtonHandler
{
    private ButtonHandler _flipButton;
    private PurchasableCard _targetPurchasableCard;
    protected void Awake()
    {
        Init();
    }

    protected void SetTargetPurchasableCard(PurchasableCard targetPurchasableCard)
    {
        // Prevent infinite recursion
        if (_targetPurchasableCard == targetPurchasableCard)
            return;

        _targetPurchasableCard = targetPurchasableCard;

        // Set the current card as the target of the target card
        if (_targetPurchasableCard != null && _targetPurchasableCard._targetPurchasableCard != this)
        {
            _targetPurchasableCard.SetTargetPurchasableCard(this);
        }
    }


    public void FlipCard()
    {
        transform.parent.gameObject.SetActive(false);
        _targetPurchasableCard.transform.parent.gameObject.SetActive(true);
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
public class PurchasableFaceCard : PurchasableCard
{

    public void UpdateTile(PurchasableTile purchasableTile)
    {
        SetTargetPurchasableCard(purchasableTile.behind);
    }
}
public class PurchasableBehindCard : PurchasableCard
{

    public void UpdateTile(PurchasableTile purchasableTile)
    {
        SetTargetPurchasableCard(purchasableTile.face);
    }
}

    