using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarControl : MonoBehaviour
{

    private Image hpFillBar;
    private Animator barShake;
    private int maxHp = 200;

    [SerializeField]Color[] hpBarColor;
    RectTransform psRectTr;
    ParticleSystem ps;
    [SerializeField] float fEndXPosition = 150f;

    // Start is called before the first frame update
    void Start()
    {
        hpFillBar = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        ps = GetComponentInChildren<ParticleSystem>();
        psRectTr = ps.GetComponent<RectTransform>();
        barShake = GetComponent<Animator>();
    }

    public void GetDamage(float fCurrentHp, float _dmg)
    {
        float _afterDmg = fCurrentHp - _dmg;
        StartCoroutine(Ie_GetDamage(fCurrentHp, _afterDmg < 0 ? 0 : _afterDmg));
    }

    IEnumerator Ie_GetDamage(float _CurrentHp, float _AfterDmgHp, float _fadeTime = 0.2f)
    {
        barShake.Play("IconGetRed");
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        
        
        barShake.Play("ShakeHp");
        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;
            ps.Emit(1);
            float currentValue = Mathf.Lerp(_CurrentHp, _AfterDmgHp, _lerpPercentage);

            float hpPercent = currentValue / maxHp;
            psRectTr.anchoredPosition = new Vector3(Mathf.Lerp(-fEndXPosition, fEndXPosition, hpPercent), psRectTr.anchoredPosition.y);
            hpFillBar.fillAmount = hpPercent;
            hpFillBar.color = Color.Lerp(hpBarColor[0], hpBarColor[1], hpPercent);
            if (_lerpPercentage >= 1) break;
            yield return new WaitForEndOfFrame();
        }

    }
}
