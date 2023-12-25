using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteController : MonoBehaviour, ISavable
{
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Dialog dialog;

    bool battleLost = false;

    FacingDirection CurrDir;

    Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        CurrDir = character.Animator.DefauleDirection;
        SetFovRotation(CurrDir);
    }

    private void Update()
    {
        character.HandleUpdate();
        //DoFovRotation();
    }

    public IEnumerator TriggerEliteBattle(PlayerController player)
    {
        // show the exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // walk toward the player
        // this is the vector between the elite's position and the player's position
        var diff = player.transform.position - transform.position;
        // minus the vector by 1 so the elite doesnt walk on the player
        var moveVec = diff - diff.normalized;
        // round the number so the x and y of the vector are integers
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // show dialog 
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, null,() =>
        {
            GameController.Instance.StartEliteBattle(this);
        }));
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.SetActive(false);
    }

    void DoFovRotation()
    {
        if (character.Animator.DefauleDirection != CurrDir)
        {
            CurrDir = character.Animator.DefauleDirection;
            SetFovRotation(CurrDir);
        }
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;
        if (battleLost == true)
            fov.SetActive(false);
    }
}
