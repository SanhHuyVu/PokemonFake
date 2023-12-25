using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ApplyBuffIndicator : MonoBehaviour
{
    [SerializeField] List<Sprite> buffSprites;
    [SerializeField] List<Sprite> debuffSprites;
    [SerializeField] Image indicator;

    Vector3 orginalPos;
    private void Awake()
    {
        orginalPos = indicator.transform.localPosition;
    }
    public IEnumerator PlayBuffAplly(bool positive = true)
    {
        bool fade = false;
        List<Sprite> sprites;

        if (positive)
            sprites = buffSprites;
        else
            sprites = debuffSprites;

        indicator.DOFade(1f, 0.25f);
        indicator.transform.DOLocalMoveY(orginalPos.y + 3f, 0.2f);
        for (int i = 0; i < sprites.Count; i++)
        {
            indicator.sprite = sprites[i];
            yield return new WaitForSeconds(0.15f);
            if (i >= sprites.Count - 1)
            {
                indicator.transform.DOLocalMoveY(orginalPos.y + 3f, 0.2f);
                indicator.DOFade(0f, 0.25f);
            }
        }

    }
}
