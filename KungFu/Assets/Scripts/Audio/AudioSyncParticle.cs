using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSyncParticle : AudioSyncer
{
    public ParticleSystem ps;

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnBeat()
    {
        base.OnBeat();
        ps.Emit(8);
        Debug.Log("Play Particle");
    }
}
