using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialControl : MonoBehaviour
{
    bool beatSpawnShowed = false;
    bool beatHitShowed = false;
    bool hitHintShowed = false;
    bool scoreHitShowed = false;
    [SerializeField]
    GameObject BeatSpawn;
    [SerializeField]
    GameObject BeatHit;
    [SerializeField]
    GameObject HitHint;
    [SerializeField]
    GameObject ScoreHit;
    // Start is called before the first frame update
    void Start()
    {
        ShowBeatSpawn();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ShowBeatSpawn()
    {
        if(!beatSpawnShowed)
        {
            StartCoroutine(TutorialShow(BeatSpawn));
            beatSpawnShowed = true;
        }
    }

    public void ShowBeatHit()
    {
        if(!beatHitShowed)
        {
            StartCoroutine(TutorialShow(BeatHit));
            beatHitShowed = true;
        }
    }

    public void ShowHitHint()
    {
        if(!hitHintShowed)
        {
            StartCoroutine(TutorialShow(HitHint));
            hitHintShowed = true;
        }
    }

    public void ShowScoreHint()
    {
        if(beatHitShowed)
        {
            if(!scoreHitShowed)
            {
                StartCoroutine(TutorialShow(ScoreHit));
                scoreHitShowed = true;
            }
        }
    }


    IEnumerator TutorialShow(GameObject tutorial, float DiedAwayTime = 5.0f)
    {
        var wait = new WaitForEndOfFrame();
        tutorial.GetComponent<CanvasGroup>().alpha = 1.0f;
        Text text = tutorial.GetComponentInChildren<Text>();
        while (text.color.a < 1.0f)
        {
            text.color = text.color + new Color(0.0f, 0.0f, 0.0f, 0.005f);
            yield return wait;
        }
        yield return new WaitForSeconds(DiedAwayTime);
        tutorial.GetComponent<CanvasGroup>().alpha = 0.0f;
        tutorial.SetActive(false);
    }
}
