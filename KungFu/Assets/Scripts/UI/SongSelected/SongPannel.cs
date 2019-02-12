using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongPannel : MonoBehaviour
{
    private Animator pannelAnim;
    // Start is called before the first frame update
    void Start()
    {
        pannelAnim = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExpandPannel() {
        pannelAnim.Play("Expand");
    }

    public void FoldPannel()
    {
        pannelAnim.Play("Fold");

    }
}
