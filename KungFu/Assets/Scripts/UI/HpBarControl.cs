using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarControl : MonoBehaviour
{

    private Image hpFillBar;
    private Animator barShake;
    private int maxHp = 200;

    RectTransform psRectTr;
    ParticleSystem ps;
    ParticleSystem.MainModule psMain;
    [SerializeField] float fEndXPosition = 150f;

    [SerializeField] Gradient gradient;

    // Start is called before the first frame update
    void Start()
    {
        hpFillBar = transform.GetChild(1).GetComponent<Image>();
        ps = GetComponentInChildren<ParticleSystem>();
        psRectTr = ps.GetComponent<RectTransform>();
        barShake = GetComponent<Animator>();
        psMain = ps.main;
    }

    public void GetDamage(float fCurrentHp, float _dmg)
    {
        float _afterDmg = fCurrentHp - _dmg;
        StartCoroutine(Ie_GetDamage(fCurrentHp, _afterDmg < 0 ? 0 : _afterDmg));
    }

    IEnumerator Ie_GetDamage(float _CurrentHp, float _AfterDmgHp, float _fadeTime = 0.2f)
    {

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
            Color _color = gradient.Evaluate(hpPercent);
            hpFillBar.color = _color;

            _color.a = 1;
            psMain.startColor = new ParticleSystem.MinMaxGradient(_color, _color/2);
            if (_lerpPercentage >= 1) break;
            yield return new WaitForEndOfFrame();
        }

    }
}
