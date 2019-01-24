using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (instance != this || instance == null)
            instance = this;
    }

    // public
    public bool bIsGameOver;
    public float fReactTime;


    // private
    private DataController _dataController;
    private MusicData currentMusicData;
    [SerializeField]
    private GameObject hitObjePrefab;

    private bool bIsMusicActive;
    private float fCurrentTime;
    private int iCurrentBeat = -1;
    private bool[] UnoInput;
    private bool[] beatPlayed;
    private bool bInBeat;


    private BeatData[] beatData;
    private AudioClip musicClip;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioSource SFXSource;
    [SerializeField]
    private Text titleText;
    [SerializeField]
    private EndGame endGame;
    private List<AudioClip> SFXclips = new List<AudioClip>();

    public int i_ExistingHitObject = 0;

    private float currentScore;
    private float totalScore = 100000;
    private int highComb;
    private int currentComb;
    private float combScore;

    private int maxComb;
    private int basicPerfectScore = 3000;
    private int basicGoodScore = 1500;

    private int perfectCount;
    private int okCount;
    private int missCount;

    // Start is called before the first frame update
    void Start()
    {
        _dataController = FindObjectOfType<DataController>();
        currentMusicData = _dataController.GetCurrentMusicData();
        musicClip = Resources.Load<AudioClip>("BGM/" + currentMusicData.name);
        titleText.text = currentMusicData.name;
        for (int i = 1; i < 10; i++)
        {
            string path = "SoundEffects/SoundEffect" + i.ToString();
            AudioClip temp = Resources.Load<AudioClip>(path);

            if (temp)
                SFXclips.Add(temp);
            else
                break;
        }

        Debug.Log("SFX count:" + SFXclips.Count);

        maxComb = currentMusicData.hitArray.Length;
        Debug.Log("Total Hit: " +maxComb);
        combScore = (totalScore - maxComb * basicPerfectScore) / ((1 + maxComb) * maxComb * 0.5f);
        Debug.Log("Basic Comb Score: " + combScore);
        beatPlayed = new bool[currentMusicData.numOfBeat - 1];
        if (!musicClip)
        {
            Debug.LogError("No certain music is found!");
            return;
        }
        audioSource.clip = musicClip;

        beatData = currentMusicData.beatArray;
        fCurrentTime = 0;

        Invoke("StartGame", 1f);
    }

    public void PlaySoundEffectRandomly()
    {
        SFXSource.Stop();
        SFXSource.clip = SFXclips[Random.Range(0, SFXclips.Count)];
        SFXSource.Play();
    }

    public int GetCurrentCombo()
    {
        return currentComb;
    }

    // Update is called once per frame
    void Update()
    {
        if (bIsMusicActive)
        {
            fCurrentTime += Time.deltaTime;

            CheckBeat();

            if (fCurrentTime > currentMusicData.lengthInSeconds)
            {
                // Game Over
                bIsMusicActive = false;
                bIsGameOver = true;
                endGame.SetEndData((int)currentScore, highComb, perfectCount, okCount, missCount, currentMusicData.name);
            }
        }
    }

    public void HitResult(int _result)
    {
        switch (_result)
        {
            // perfect
            case 0:
                {
                    perfectCount++;
                    currentComb++;
                    if (currentComb >= highComb)
                        highComb = currentComb;

                    currentScore += (currentComb * combScore + basicPerfectScore);
                    break;
                }
           // good
            case 1:
                {
                    okCount++;
                    currentComb++;
                    if (currentComb >= highComb)
                        highComb = currentComb;

                    currentScore += (currentComb * combScore + basicGoodScore);
                    break;
                }
            // miss
            case 2:
                {
                    missCount++;
                    currentComb = 0;
                    break;
                }

            default:
                break;
        }
    }

    public void StartGame()
    {
        bIsMusicActive = true;
        audioSource.Play();
        Debug.Log("Game Start!");
        fCurrentTime = 0;
        UnoInput = new bool[currentMusicData.numOfBeat - 1];
    }

    public float GetPercentage()
    {
        float percent = 0;
        if (currentMusicData != null)
            percent = fCurrentTime / currentMusicData.lengthInSeconds;
        else
            percent = 0;
        return percent;
    }

    public int GetCurrentScore()
    {
        return (int)currentScore;
    }

    // get the current time which beat it is in.
    void CheckBeat()
    {
        for (int i = 0; i < beatData.Length - 1; i++)
        {
            if (beatPlayed[i])
                continue;
            if (Mathf.Abs((beatData[i].timeToHit - fReactTime) - fCurrentTime) <= 0.01f)
            {
                beatPlayed[i] = true;
                iCurrentBeat = i;
                int repeatHit = 0;

                Debug.Log("Start beat: " + iCurrentBeat);
                // go through the hit array
                for (int j = 0; j < currentMusicData.hitArray.Length; j++)
                {
                    // compare the beatID with the current beat id
                    if (currentMusicData.hitArray[j].beatID == iCurrentBeat)
                    {
                        // Instantiate hit object
                        GameObject go = Instantiate(hitObjePrefab, UIController.instance.refParent);
                        //UIController.instance.SetReference(iCurrentBeat);
                        go.transform.localPosition += repeatHit * Vector3.up;
                        repeatHit++;
                        go.GetComponent<HitObjet>().SetButtonID(currentMusicData.hitArray[j].buttonID, iCurrentBeat);

                    }

                }
                break;
            }
            else
            {
                iCurrentBeat = -1;
            }
        }
    }

    public void DebugInput(int _buttonID)
    {
        UnoInput[_buttonID] = true;

        StartCoroutine(falseButton(_buttonID));
    }
    IEnumerator falseButton(int _buttonID)
    {
        yield return new WaitForSeconds(0.1f);
        UnoInput[_buttonID] = false;
    }


    public void SetUnoInput(bool[] _input)
    {
        UnoInput = _input;
    }
    public bool GetUnoInput(int _buttonID)
    {
        return UnoInput[_buttonID];
    }
    public bool[] GetUnoInputs()
    {
        return UnoInput;
    }
}
