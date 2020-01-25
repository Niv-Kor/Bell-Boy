using UnityEngine;

public class LifeBit : MonoBehaviour
{
    [Tooltip("A full heart sprite that represents an unused life bit.")]
    [SerializeField] private Sprite fullSprite;

    [Tooltip("An empty heart sprite that represents a lack of life bit.")]
    [SerializeField] private Sprite emptySprite;

    private SpriteRenderer spriteRender;
    private ParticleSystem partSystem;
    
    public bool Exists { get; private set; }

    private void Start() {
        this.spriteRender = GetComponent<SpriteRenderer>();
        this.partSystem = GetComponent<ParticleSystem>();
        this.Exists = true;
        Switch(true);
    }

    /// <summary>
    /// Switch between a full life bit and an empty life bit.
    /// </summary>
    /// <param name="flag">True to fill the life bit or false to empty it</param>
    public void Switch(bool flag) {
        spriteRender.sprite = flag ? fullSprite : emptySprite;
        Exists = flag;
    }

    /// <summary>
    /// Activate particles effect.
    /// </summary>
    public void PlayParticles() { partSystem.Play(); }
}