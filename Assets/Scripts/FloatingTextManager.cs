using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject textContainer;
    public GameObject textPrefab;

    public List<FloatingText> floatingTexts = new List<FloatingText>();

    private void Update()
    {
        foreach (FloatingText txt in floatingTexts)
            txt.UpdateFloatingText();
    }

    public void Show(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        FloatingText floatingText = GetFloatingText();

        floatingText.txt.text = msg;
        floatingText.txt.fontSize = fontSize;
        floatingText.txt.color = color;

        //                                   transfer wolrd space to screen space so we can use it in UI
        //floatingText.go.transform.position = Camera.main.WorldToScreenPoint(position);

        floatingText.go.transform.position = new Vector3(position.x + 0.5f, position.y - 0.2f, position.z);
        floatingText.go.transform.localScale = new Vector3(1f, 1f, 1f);

        floatingText.motion = motion;
        floatingText.duration = duration;

        floatingText.Show();
    }

    private FloatingText GetFloatingText()
    {
        // take the floating text array and find one that is not active
        FloatingText txt = floatingTexts.Find(t => !t.active);

        if (txt == null)
        {
            txt = new FloatingText();
            txt.go = Instantiate(textPrefab);
            txt.go.transform.SetParent(textContainer.transform);
            txt.txt = txt.go.GetComponent<UnityEngine.UI.Text>();

            floatingTexts.Add(txt);
        }

        return txt; 
    }
}
