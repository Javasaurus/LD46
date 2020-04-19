using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimationSync : MonoBehaviour
{

    private Sprite[] sprites;

    private SpriteRenderer targetRenderer;
    public SpriteRenderer animationRenderer;
    public string spriteSheetPath;

    public void Awake()
    {
        if (sprites == null)
        {
            sprites = Resources.LoadAll<Sprite>(spriteSheetPath); // load all sprites in "assets/Resources/sprite" folder
        }
        targetRenderer = GetComponent<SpriteRenderer>();
    }

    public void setSprites(Sprite[] sprites )
    {
        this.sprites = sprites;
    }

    public void LateUpdate()
    {
        targetRenderer.flipX = animationRenderer.flipX;
        targetRenderer.sprite = GetSprite();
    }

    private Sprite GetSprite()
    {
        if (sprites == null)
        {
            return null;
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].name == animationRenderer.sprite.name)
            {
                return sprites[i];
            }
        }

        return sprites[0];
    }
}
