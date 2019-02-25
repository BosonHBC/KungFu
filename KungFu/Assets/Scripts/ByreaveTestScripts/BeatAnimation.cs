using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatTiming
{
    public float OKStart;
    public float OKDuration;
    public float PerfectStart;
    public float PerfectDuration;
    public BeatTiming(float okstart, float okduration, float perstart, float perduration)
    {
        OKStart = okstart;
        OKDuration = okduration;
        PerfectStart = perstart;
        PerfectDuration = perduration;
    }
}
