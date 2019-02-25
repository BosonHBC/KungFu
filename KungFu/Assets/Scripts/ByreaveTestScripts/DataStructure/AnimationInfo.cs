using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public enum BeatMode
{
    Attack,
    Defend
}
public class AnimationInfo
{
    public int AnimationID;
    public string AnimationName;
    public BeatMode Mode;
    public int[] BeatIDs;
    public AnimationInfo(JSONNode animInfo)
    {
        AnimationID = animInfo["AnimationID"].AsInt;
        AnimationName = animInfo["AnimationName"];
        if (animInfo["Mode"] == "Attack")
            Mode = BeatMode.Attack;
        else
            Mode = BeatMode.Defend;
        BeatIDs = DataUtility.GetIntArrayFromJSONNode(animInfo["Beats"]);
    }
}
