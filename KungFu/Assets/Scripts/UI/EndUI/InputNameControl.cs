using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputNameControl : MonoBehaviour
{
    public bool bCanShoose;
    private int iMaxIndex = 3;
    private int iCurrentIndex;
    private int[] initials = { 0, 0, 0 };
    char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    Text[] childs;
    // Start is called before the first frame update
    void Start()
    {
        childs = new Text[] {
            transform.GetChild(1).GetChild(2).GetComponent<Text >(),
            transform.GetChild(2).GetChild(2).GetComponent<Text >(),
            transform.GetChild(3).GetChild(2).GetComponent<Text >()
        };
    }

    // Update is called once per frame
    void Update()
    {
        DebugTest();
    }
    private void DebugTest()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            NextCharacter();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            PreviousCharacter();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            NextIndex();

    }

    public void NextIndex()
    {
        if (bCanShoose)
        {
            iCurrentIndex++;
            if (iCurrentIndex < iMaxIndex)
            {
                childs[iCurrentIndex - 1].transform.parent.GetChild(0).gameObject.SetActive(false);
                childs[iCurrentIndex - 1].transform.parent.GetChild(1).gameObject.SetActive(false);

                childs[iCurrentIndex].transform.parent.GetChild(0).gameObject.SetActive(true);
                childs[iCurrentIndex].transform.parent.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                bCanShoose = false;
                childs[iMaxIndex - 1].transform.parent.GetChild(0).gameObject.SetActive(false);
                childs[iMaxIndex - 1].transform.parent.GetChild(1).gameObject.SetActive(false);
                // Upload Data to the High Score Manager
            }
        }
       
    }

    public void NextCharacter()
    {
        if (bCanShoose && iCurrentIndex < iMaxIndex)
        {
            initials[iCurrentIndex]++;
            if (initials[iCurrentIndex] > 25)
                initials[iCurrentIndex] = 0;

            childs[iCurrentIndex].text = alpha[initials[iCurrentIndex]].ToString() + ".";
        }
    }
    public void PreviousCharacter()
    {
        if (bCanShoose && iCurrentIndex < iMaxIndex)
        {
            initials[iCurrentIndex]--;
            if (initials[iCurrentIndex] < 0)
                initials[iCurrentIndex] = 25;

            childs[iCurrentIndex].text = alpha[initials[iCurrentIndex]].ToString() + ".";
        }
    }
}
