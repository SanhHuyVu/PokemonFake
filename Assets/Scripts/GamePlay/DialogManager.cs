using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    [SerializeField] GameObject optionBox;
    [SerializeField] Text opt1;
    [SerializeField] Text opt2;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    DialogState state;

    Unit unitToGive = null;

    public static DialogManager Instance { get; private set; }

    // set refrence to assec from other class
    private void Awake()
    {
        Instance = this;
    }

    Dialog dialog;
    Action OnDialogFinished;

    int currentLine = 0;
    bool isTyping;

    public bool isShowing { get; private set; }

    public IEnumerator ShowDialog(Dialog dialog, Unit unitToGive = null, Action Onfinished = null)
    {
        yield return new WaitForEndOfFrame();

        state = DialogState.Dialog;

        this.unitToGive = unitToGive;

        state = DialogState.Dialog;
        OnShowDialog.Invoke();

        isShowing = true;
        this.dialog = dialog;
        OnDialogFinished = Onfinished;

        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping == false)
            {
                ++currentLine;
                if (currentLine < dialog.Lines.Count && state == DialogState.Dialog)
                {
                    StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                }
                else if (unitToGive != null && state == DialogState.Dialog)
                {
                    state = DialogState.ChooseAction;
                    StartCoroutine(TypeDialog("do you want one?"));
                    optionBox.SetActive(true);
                }
                else if(state == DialogState.Dialog)
                {
                    isShowing = false;
                    currentLine = 0;
                    dialogBox.SetActive(false);
                    optionBox.SetActive(false);

                    OnDialogFinished?.Invoke();
                    OnCloseDialog?.Invoke();
                }
            }
        }
        else if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Backspace)) && !isTyping)
        {
            if (state == DialogState.Dialog)
            {
                currentLine = 0;
                isShowing = false;
                dialogBox.SetActive(false);
                OnDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }



    // temorary unit give action
    public void OptHover(int opt)
    {
        switch (opt)
        {
            case 0:
                opt1.color = Color.blue;
                break;
            case 1:
                opt2.color = Color.blue;
                break;
        }
    }
    public void OptHoverExit(int opt)
    {
        switch (opt)
        {
            case 0:
                opt1.color = Color.black;
                break;
            case 1:
                opt2.color = Color.black;
                break;
        }
    }
    public void Optclicked(int opt)
    {
        switch (opt)
        {
            case 0:
                state = DialogState.Dialog;
                optionBox.SetActive(false);
                if (GameController.Instance.PlayerController.GetComponent<UnitParty>().AddUnitToParty(unitToGive))
                {
                    StartCoroutine(TypeDialog($"{unitToGive.Base.name} added to your party"));
                    unitToGive = null;
                }
                else
                {
                    StartCoroutine(TypeDialog("party is full"));
                    unitToGive = null;
                }
                break;
            case 1:
                state = DialogState.Dialog;
                optionBox.SetActive(false);
                StartCoroutine(TypeDialog("Come back if you change your mind"));
                break;
        }
    }
    // end of temp
}

public enum DialogState { Dialog, ChooseAction }