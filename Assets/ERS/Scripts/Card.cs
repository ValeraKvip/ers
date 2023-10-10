using UnityEngine;
using UnityEngine.U2D;

public class Card : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas atlas;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetupSprite();
    }

    public void SetupSprite()
    {
        spriteRenderer.sprite = atlas.GetSprite(gameObject.name);
    }

    public void SetDisplayingOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }


}
