using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeHint : MonoBehaviour
{
    private UIMover mover;
    private RectTransform rectTr;
    private Text hintText;

    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<UIMover>();
        rectTr = GetComponent<RectTransform>();
        hintText = transform.GetChild(1).GetComponent<Text>();
    }

    public void SwitchMode(bool bAttack)
    {
        if (bAttack)
        {
            hintText.text = "ATTACK PHASE";
        }
        else
        {
            hintText.text = "DEFENSE PHASE";
        }
        StartCoroutine(ieUIMover());
    }

    IEnumerator ieUIMover()
    {
        mover.SimpleLocalPositionMover(transform.localPosition, Vector3.left * 300 + transform.localPosition, 0.3f);
        yield return new WaitForSeconds(2.3f);
        mover.SimpleLocalPositionMover(transform.localPosition, Vector3.right * 300 + transform.localPosition, 0.3f);

    }
}
