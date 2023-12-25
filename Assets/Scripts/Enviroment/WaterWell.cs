using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWell : MonoBehaviour
{
    [SerializeField] List<Sprite> sprites;

    SpriteRenderer spriteRenderer;
    SpriteAnimator spriteAnimator;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteAnimator = new SpriteAnimator(sprites, spriteRenderer);       
    }
    private void Update()
    {
        spriteAnimator.HandleUpdate();
    }

}
