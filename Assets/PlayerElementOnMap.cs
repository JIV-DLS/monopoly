using UnityEngine;

public class PlayerElementOnMap : MonoBehaviourWithInitComponent
{
    // Get the SpriteRenderer component of the current GameObject
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    public override void OtherInit()
    {
        base.OtherInit();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer component found on this GameObject.");
        }
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
