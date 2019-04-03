using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class BeatInfo
{
    public int BeatID;
    public int[] ButtonIDs;
    public int[] JointIDs;
    public BeatMode Mode;
    public float OKStart;
    public float OKDuration;
    public bool IsCombo;
    public float PerfectStart;
    public float PerfectDuration;
    public float KnockBack_H;
    public float KnockBack_V;

    public BeatInfo(JSONNode beatInfo)
    {
        float _scalar = 1.0f;
        BeatID = beatInfo["BeatID"].AsInt;
        ButtonIDs = DataUtility.GetIntArrayFromJSONNode(beatInfo["ButtonID"]);
        JointIDs = DataUtility.GetIntArrayFromJSONNode(beatInfo["JointID"]);
        IsCombo = beatInfo["IsCombo"].AsBool;
        OKStart = beatInfo["OKStart"].AsFloat * _scalar;
        OKDuration = beatInfo["OKDuration"].AsFloat * _scalar;
        PerfectStart = beatInfo["PerfectStart"].AsFloat * _scalar;
        PerfectDuration = beatInfo["PerfectDuration"].AsFloat * _scalar;
        KnockBack_H = beatInfo["KnockBack_H"];
        KnockBack_V = beatInfo["KnockBack_V"];
        if (beatInfo["Mode"].Value == "Defend")
            Mode = BeatMode.Defend;
        else
            Mode = BeatMode.Attack;
    }

    //public BeatInfo(int id, int[] buttonIDs, int[] jointIDs, bool isCombo, float okstart, float okduration, float perstart, float perduration)
    //{
    //    BeatID = id;
    //    ButtonIDs = buttonIDs;
    //    JointIDs = jointIDs;
    //    IsCombo = isCombo;
    //    OKStart = okstart;
    //    OKDuration = okduration;
    //    PerfectStart = perstart;
    //    PerfectDuration = perduration;
    //}
}
