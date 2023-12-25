using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] BattleHud allyHud;
    [SerializeField] EnemyBattleHUD enemyHud;
    [SerializeField] CanvasGroup allyCanvasGroup;
    [SerializeField] CanvasGroup enemyCanvasGroup;
    [SerializeField] ApplyBuffIndicator buffIndicator;

    public Unit Unit { get; set; }
    public bool isPlayerUnit { get; set; }
    public BattleHud AllyHud { get { return allyHud; } }
    public EnemyBattleHUD EnemyHud { get { return enemyHud; } }
    public ApplyBuffIndicator BuffIndicator { get { return buffIndicator; } } 
    Image image;
    Vector3 orginalPos;
    Color orginalColor;
    public void Awake()
    {
        image = GetComponent<Image>();
        orginalPos = image.rectTransform.localPosition;
        orginalColor = image.color;
    }
    public void SetUp(Unit unit, bool IsPlayerUnit)
    {
        Unit = unit;
        isPlayerUnit = IsPlayerUnit;
        if (isPlayerUnit)
        {
            image.sprite = Unit.Base.BackSprite;
            allyHud.SetData(unit);
            allyHud.gameObject.SetActive(true);
        }
        else
        {
            image.sprite = Unit.Base.FrontSprite;
            enemyHud.SetDataEnemies(unit);
            enemyHud.gameObject.SetActive(true);
        }

        image.color = orginalColor;
        PlayeEnterAnimation();
    }

    public void PlayeEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(orginalPos.x, orginalPos.y - 30f);
        else
            image.transform.localPosition = new Vector3(orginalPos.x, orginalPos.y - 15f);
        image.transform.DOLocalMoveY(orginalPos.y, 1f);
    }
    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalRotate(new Vector3(0, 0, orginalPos.z + 5f), 0.2f));
            sequence.Append(image.transform.DOLocalRotate(new Vector3(0, 0, orginalPos.z - 10f), 0.2f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalRotate(new Vector3(0f, 0f, orginalPos.z - 5f), 0.2f));
            sequence.Append(image.transform.DOLocalRotate(new Vector3(0f, 0f, orginalPos.z + 15f), 0.2f));
        }
        sequence.Append(image.transform.DOLocalRotate(new Vector3(0f, 0f, orginalPos.z), 0.25f));
        //sequence.Append(image.transform.DOLocalMoveX(orginalPos.x, 0.25f));
    }
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x - 10f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x + 10f, 0.25f));

        sequence.Join(image.DOColor(Color.grey, 0.25f));
        sequence.Append(image.transform.DOLocalMoveX(orginalPos.x, 0.25f));
        sequence.Join(image.DOColor(orginalColor, 0.25f));
    }
    public void PlayDeadAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalRotate(new Vector3(0f, 0f, orginalPos.z - 10f), 1.0f));
        else
            sequence.Append(image.transform.DOLocalRotate(new Vector3(0f, 0f, orginalPos.z + 10f), 1.0f));

        sequence.Join(image.transform.DOLocalMoveY(orginalPos.y - 12f, 1.0f));
        sequence.Join(image.DOColor(Color.grey, 1.0f));
        if (isPlayerUnit)
        {
            //sequence.Append(allyCanvasGroup.DOFade(0f, 0.5f));
        }
        else
            sequence.Append(enemyCanvasGroup.DOFade(0f, 0.5f));
    }
    public void DeFadeCG()
    {
        if (isPlayerUnit)
            allyCanvasGroup.DOFade(1f, 0f);
        else
            enemyCanvasGroup.DOFade(1f, 0f);
    }
}
