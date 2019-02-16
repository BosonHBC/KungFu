using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatAnimation
{
    public int ID;
    public string Name;
    public float OKStart;
    public float OKDuration;
    public float PerfectStart;
    public float PerfectDuration;
    public BeatAnimation(int id, string name, float okstart, float okduration, float perstart, float perduration)
    {
        ID = id;
        Name = name;
        OKStart = okstart;
        OKDuration = okduration;
        PerfectStart = perstart;
        PerfectDuration = perduration;
    }
}
