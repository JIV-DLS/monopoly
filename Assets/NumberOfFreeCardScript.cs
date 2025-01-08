using UnityEngine;
using UnityEngine.PlayerLoop;

public class NumberOfFreeCardScript : MonoBehaviourWithInitComponent
{
    public BaseTextHandler textHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNumberOfFreeCard(int numberOfFreeCards)
    {
        if (numberOfFreeCards > 0)
        {
            textHandler.SetText(numberOfFreeCards.ToString());
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public override void OtherInit()
    {
        textHandler.OtherInit();
    }
}
