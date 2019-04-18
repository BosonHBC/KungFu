using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RT_CreateParticle : MonoBehaviour
{

    ParticleSystem ps;
    ParticleSystem.MainModule main;
    [SerializeField] float[] speedArray;
    [SerializeField] float[] startSizeArray;

    // Start is called before the first frame update
    void Start()
    {
        ps = transform.Find("ParticleSystem").GetComponent<ParticleSystem>();
        main = ps.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        //SetPosition();
    }
    public void SetSize(int _currentCombo)
    {
        main.startSpeed = Mathf.Lerp(speedArray[0], speedArray[1], _currentCombo / 65f);
        main.startSize = Mathf.Lerp(startSizeArray[0], startSizeArray[1], _currentCombo / 65f);
        if (_currentCombo < 5)
            ps.Stop();
        else
        {
            ps.Play();
        }
    }
}
