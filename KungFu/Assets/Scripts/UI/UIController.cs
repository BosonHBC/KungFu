using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    private void Awake()
    {
        if (instance == null || instance != this)
            instance = this;
    }

    [SerializeField] Sprite[] resultsSprite;
    [SerializeField] GameObject resultPrefab;
    private Image result;
    private Transform resultParent;
    private Image fillBar;
    private RectTransform fillIndicator;
    private Text scoreText;
    private Text comboText;
    private Animator pandaAnim;

    private List<Sprite> imgRef = new List<Sprite>();
    [HideInInspector]
    public Transform refParent;
    private int numOfBeat;
    
    // Start is called before the first frame update
    void Start()
    {
        Transform background = transform.Find("Background");
        resultParent = transform.Find("ResultParent");
        refParent = transform.Find("CurrentPoseParent");
        fillBar = background.Find("FillBar").GetComponent<Image>();
        fillIndicator = fillBar.transform.GetChild(0).GetComponent<RectTransform>();
        scoreText = background.Find("Score").GetComponent<Text>();
        comboText = background.Find("Combo").GetChild(0).GetComponent<Text>();
        pandaAnim = background.Find("Panda").GetChild(0).GetComponent<Animator>();
        DataController _data = FindObjectOfType<DataController>();
        if (!_data)
        {
            Debug.LogError("No data founded");
            return;
        }
        numOfBeat = _data.GetCurrentMusicData().numOfBeat - 1;
        for (int i = 0; i < numOfBeat; i++)
        {
            imgRef.Add(Resources.Load<Sprite>("Images/HitRef/" + ((i+1 < 10) ? ("0" + (i+1).ToString()) : (i+1).ToString())));
        }


    }

    // Update is called once per frame
    void Update()
    {
        float percentage = GameManager.instance.GetPercentage();
        fillBar.fillAmount = percentage;
        fillIndicator.localPosition = new Vector3(-400 + percentage * 800, 0,0);

        scoreText.text = GameManager.instance.GetCurrentScore().ToString();

        comboText.text = GameManager.instance.GetCurrentCombo().ToString();
    }


    public void SetReference(int beatID)
    {
        if(beatID < numOfBeat)
        {
            // set next
            refParent.GetComponent<Image>().sprite = imgRef[beatID];
            refParent.GetComponent<Image>().color = Color.white;
        }

    }

    public void PlayPandaNodHead()
    {
        pandaAnim.Play("PandaNode");
    }
    public Sprite GetReference(int beatID)
    {
        return imgRef[beatID];
    }


    public void ShowResult(int _result)
    {
        // Instantiate object
        GameObject go = Instantiate(resultPrefab, resultParent, false);
        go.transform.position += (resultParent.childCount-1) * 2.5f* Vector3.right;
        
        go.GetComponent<Image>().sprite = resultsSprite[_result];

    }

}
