using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int letterPerSecond;

    [SerializeField] Text dialogText;
    [SerializeField] Image portrait;
    [SerializeField] Image hostilePortrait;
    [SerializeField] Image dialogBoxContainer;
    [SerializeField] Image battleUI_player;

    public void SetDialog(string dialog, Unit unit = null, bool isHostile=false)
    {
        if (unit == null)
        {
            hostilePortrait.gameObject.SetActive(false);
            portrait.gameObject.SetActive(false);
        }
        else
        {
            if (isHostile == true)
            {
                hostilePortrait.gameObject.SetActive(true);
                portrait.gameObject.SetActive(false);
                hostilePortrait.sprite = unit.Base.FrontSprite;
            }
            else
            {
                hostilePortrait.gameObject.SetActive(false);
                portrait.gameObject.SetActive(true);
                portrait.sprite = unit.Base.BackSprite;
            }
        }
        unit = null;

        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog, Unit unit = null, bool isHostile = false)
    {
        if (unit == null)
        {
            hostilePortrait.gameObject.SetActive(false);
            portrait.gameObject.SetActive(false);
        }
        else
        {
            if (isHostile == true)
            {
                hostilePortrait.gameObject.SetActive(true);
                portrait.gameObject.SetActive(false);
                hostilePortrait.sprite = unit.Base.FrontSprite;
            }
            else
            {
                hostilePortrait.gameObject.SetActive(false);
                portrait.gameObject.SetActive(true);
                portrait.sprite = unit.Base.BackSprite;
            }
        }
        unit = null;

        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
    }

    public void enableDialogBoxContainer(bool enabled)
    {
        dialogBoxContainer.gameObject.SetActive(enabled);
        if (enabled == true)
            battleUI_player.gameObject.SetActive(false);
        else
            battleUI_player.gameObject.SetActive(true);
    }
}
