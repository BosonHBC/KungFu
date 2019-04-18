using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class RotateCharacter : MonoBehaviour
{
    [SerializeField] Transform[] charas;
    Animator[] anims;
    Vector3 lookDir = Vector3.back;

    int currentCharacter = 0;
    bool bRotating;
    Quaternion destQuat;

    private PostProcessVolume volume;
    private DepthOfField DoF;
    // Start is called before the first frame update
    void Start()
    {
        anims = new Animator[charas.Length];
        for (int i = 0; i < charas.Length; i++)
        {
            anims[i] = charas[i].GetComponent<Animator>();
        }

        volume = Camera.main.GetComponent<PostProcessVolume>();
        bool isValid1 = volume.profile.TryGetSettings<DepthOfField>(out DoF);
        MenuCanvasControl.OnCanvasChange += ChangeDoF;
    }

    public void ChangeDoF(MenuCanvasControl.MenuCanvas _canvas)
    {
        if (_canvas == MenuCanvasControl.MenuCanvas.CharacterSelect)
        {
            // turn on DoF
            DoF.active = true;
        }
        else
        {
            // turn off Dof
            DoF.active = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < charas.Length; i++)
        {
            charas[i].rotation = Quaternion.LookRotation(lookDir, charas[i].up);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SwitchChar(-1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SwitchChar(1);
        }
    }

    public void SwitchChar(int _dleta)
    {
        if (!bRotating)
        {
            if (_dleta > 0)
            {
                // next character
                currentCharacter++;
                if (currentCharacter > 2)
                    currentCharacter = 0;
            }
           else if (_dleta < 0)
            {
                // next character
                currentCharacter--;
                if (currentCharacter < 0)
                    currentCharacter = 2;
            }
            destQuat = Quaternion.Euler(new Vector3(0, currentCharacter * 120f, 0));
            StartCoroutine(Rotator(destQuat, delegate { anims[currentCharacter].SetBool("PlayGesture", true); },0.8f));
        }

    }

    IEnumerator Rotator(Quaternion _endLPos, UnityAction _onFinishRotation,float _fadeTime = 0.5f)
    {
        bRotating = true;
        float _timeStartFade = Time.time;
        float _timeSinceStart = Time.time - _timeStartFade;
        float _lerpPercentage = _timeSinceStart / _fadeTime;

        Quaternion start = transform.localRotation;
        while (true)
        {
            _timeSinceStart = Time.time - _timeStartFade;
            _lerpPercentage = _timeSinceStart / _fadeTime;

            Quaternion currentValue = Quaternion.Lerp(start, _endLPos, _lerpPercentage);
           transform.localRotation = currentValue;

            if (_lerpPercentage >= 1) break;

            yield return new WaitForEndOfFrame();
        }
        bRotating = false;
        if (_onFinishRotation != null)
            _onFinishRotation.Invoke();
    }
}
