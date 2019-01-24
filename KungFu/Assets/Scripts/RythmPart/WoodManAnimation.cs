using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManAnimation : MonoBehaviour
{
    Animator anim;
    List<Animator> handAnims = new List<Animator>();
    bool[] _bool;
    bool[] isPlaying;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        for (int i = 1; i < transform.childCount; i++)
        {
            handAnims.Add(transform.GetChild(i).GetComponent<Animator>());
        }
        isPlaying = new bool[3];

    }

    // Update is called once per frame
    void Update()
    {
        _bool = GameManager.instance.GetUnoInputs();

        if((_bool[1] || _bool[2] || _bool[3]) && !isPlaying[0])
        {
            Debug.Log("Arm 0");
            isPlaying[0] = true;
            handAnims[0].Play("ArmShake0");
            GameManager.instance.PlaySoundEffectRandomly();
            StartCoroutine(CancelPlay(0));
        }
        if ((_bool[5] || _bool[6] || _bool[7]) && !isPlaying[1])
        {
            Debug.Log("Arm 1");
            isPlaying[1] = true;
            handAnims[1].Play("ArmShake1");
            GameManager.instance.PlaySoundEffectRandomly();
            StartCoroutine(CancelPlay(1));
        }
        if ((_bool[8] || _bool[9] || _bool[10]) && !isPlaying[2])
        {
            Debug.Log("Arm 2");
            isPlaying[2] = true;
            handAnims[2].Play("ArmShake2");
            GameManager.instance.PlaySoundEffectRandomly();
            StartCoroutine(CancelPlay(2));
        }

    }

    IEnumerator CancelPlay(int _i)
    {
        yield return new WaitForSeconds(1f);
        isPlaying[_i] = false;
       
    }
}
