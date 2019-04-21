using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBarControl : MonoBehaviour
{
    public static TimeBarControl instance;
    private void Awake()
    {
        if (instance == null || instance != this)
            instance = this;
    }

    [SerializeField] Image fillImg;
    [SerializeField] RectTransform pointerRectTr;

    Vector3 pointerStartPos;
    // Start is called before the first frame update
    void Start()
    {
        pointerStartPos = pointerRectTr.anchoredPosition3D;
        SetPercentage(0);
    }


    public void SetPercentage(float _percentage)
    {
        float _realPercent = Mathf.Lerp(0.055f, 1, _percentage);
        fillImg.fillAmount = _realPercent;
        Vector3 _realPosition = Vector3.Lerp(pointerStartPos, new Vector3(343.7f, pointerStartPos.y, pointerStartPos.z), _percentage);
        float clampX = _realPosition.x;
        clampX = Mathf.Clamp(clampX, pointerStartPos.x, 301f);

        pointerRectTr.anchoredPosition = new Vector3(clampX, pointerStartPos.y, pointerStartPos.z);
    }
}
