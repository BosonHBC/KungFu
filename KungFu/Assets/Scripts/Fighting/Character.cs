using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    protected string sCharName;
    public int iCharID;
    protected Transform trOppoent;


    protected bool bFaceToOpponent = true; 

    // Start is called before the first frame update
   protected virtual void Start()
    {
        SetData();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        FaceToOpponent();
    }

    void SetData()
    {
        bFaceToOpponent = true;
        name = "P" + iCharID + "_" + sCharName;
        Character[] _characters = FindObjectsOfType<Character>();
        for (int i = 0; i < _characters.Length; i++)
        {
            if (_characters[i].iCharID != iCharID)
            {
                trOppoent = _characters[i].transform;
            }
        }
    }

    void FaceToOpponent()
    {
        if (bFaceToOpponent)
        {
            transform.LookAt(trOppoent.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
