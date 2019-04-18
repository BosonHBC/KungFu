using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ComboController : MonoBehaviour
{
    RT_CreateParticle particleCtrl;
    Text comboText;
    Animator comboAnim;
    // Start is called before the first frame update
    void Start()
    {
        particleCtrl = GetComponent<RT_CreateParticle>();
        comboText = transform.Find("ComboText").GetComponent<Text>();
        comboAnim = GetComponent<Animator>();
        comboText.text = "0";
        ChangeCombo(0);
    }

    public void ChangeCombo(int _combo)
    {
        if (_combo != 0)
            comboAnim.Play("Pop");
        comboText.text = _combo.ToString();

        particleCtrl.SetSize(_combo);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
