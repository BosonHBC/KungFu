using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialControl : MonoBehaviour
{
    AudioSource audioControl;
    bool beatSpawnShowed = false;
    bool beatHitShowed = false;
    [SerializeField]
    GameObject BeatSpawn;
    [SerializeField]
    GameObject BeatHit;
    // Start is called before the first frame update
    void Start()
    {
        audioControl = FindObjectOfType<AudioSource>();
        ShowBeatSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        audioControl.Pause();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        audioControl.UnPause();
    }

    public void ShowBeatSpawn()
    {
        if(!beatSpawnShowed)
        {
            StartCoroutine(BeatSpawnHint());
            beatSpawnShowed = true;
        }
    }

    public void ShowBeatHit()
    {
        if(!beatHitShowed)
        {
            StartCoroutine(BeatHitHint());
            beatHitShowed = true;
        }
    }

    IEnumerator BeatSpawnHint()
    {
        var wait = new WaitForEndOfFrame();
        BeatSpawn.SetActive(true);
        Text beatSpawnText = BeatSpawn.GetComponentInChildren<Text>();
        while(beatSpawnText.color.a < 1.0f)
        {
            beatSpawnText.color = beatSpawnText.color + new Color(0.0f, 0.0f, 0.0f, 0.005f);
            yield return wait;
        }
        yield return new WaitForSeconds(5.0f);
        BeatSpawn.SetActive(false);
    }

    IEnumerator BeatHitHint()
    {
        var wait = new WaitForEndOfFrame();
        BeatHit.SetActive(true);
        Text beatHitText = BeatHit.GetComponentInChildren<Text>();
        while (beatHitText.color.a < 1.0f)
        {
            beatHitText.color = beatHitText.color + new Color(0.0f, 0.0f, 0.0f, 0.005f);
            yield return wait;
        }
        yield return new WaitForSeconds(5.0f);
        BeatHit.SetActive(false);
    }
}
