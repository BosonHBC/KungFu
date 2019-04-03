using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGenerator : MonoBehaviour
{
    #region Instance
    public static ParticleGenerator instance;
    private void Awake()
    {
        if(instance!=this || instance == null)
        {
            instance = this;
        }
    }
    #endregion

    [SerializeField] private GameObject[] particles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateOneTimeParticleAtPosition(int _index, Vector3 _worldPos)
    {
       // UnityEditor.EditorApplication.isPaused = true;
        GameObject go = Instantiate(particles[_index]);
        go.transform.position = _worldPos;
        ParticleSystem ps = go.GetComponent<ParticleSystem>();
        ps.Play();
        Destroy(go, ps.main.startLifetime.constant + ps.main.duration);
    }
}
