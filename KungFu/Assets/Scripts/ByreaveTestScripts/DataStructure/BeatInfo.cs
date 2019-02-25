using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class BeatInfo
{
    public int BeatID;
    public int[] ButtonIDs;
    public float OKStart;
    public float OKDuration;
    public float PerfectStart;
    public float PerfectDuration;

    public BeatInfo(JSONNode beatInfo)
    {
        BeatID = beatInfo["BeatID"].AsInt;
        ButtonIDs = DataUtility.GetIntArrayFromJSONNode(beatInfo["ButtonID"]);
        OKStart = beatInfo["OKStart"].AsFloat;
        OKDuration = beatInfo["OKDuration"].AsFloat;
        PerfectStart = beatInfo["PerfectStart"].AsFloat;
        PerfectDuration = beatInfo["PerfectDuration"].AsFloat;
    }

    public BeatInfo(int id, int[] buttonIDs, float okstart, float okduration, float perstart, float perduration)
    {
        BeatID = id;
        ButtonIDs = buttonIDs;
        OKStart = okstart;
        OKDuration = okduration;
        PerfectStart = perstart;
        PerfectDuration = perduration;
    }
}
