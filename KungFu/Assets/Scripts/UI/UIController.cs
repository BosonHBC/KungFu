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

    [SerializeField] List<Sprite> imgRef = new List<Sprite>();
    Transform refParent;
    private int numOfBeat;
    
    // Start is called before the first frame update
    void Start()
    {
        resultParent = transform.GetChild(0);
        refParent = transform.GetChild(2);
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
        
    }

    public void SetReference(int beatID)
    {
        if(beatID < numOfBeat)
        {
            // set next
            refParent.GetComponent<Image>().sprite = imgRef[beatID];
            refParent.GetComponent<Image>().color = Color.white;
        }
        else
        {
            // end
            CleanReference();
        }
    }

    public Sprite GetReference(int beatID)
    {
        return imgRef[beatID];
    }

    public void CleanReference()
    {
        Color _color = Color.white;
        _color.a = 0;
        refParent.GetComponent<Image>().color = _color;
    }

    public void ShowResult(int _result)
    {
        // Instantiate object
        GameObject go = Instantiate(resultPrefab, resultParent, false);
       // go.transform.position += (resultParent.childCount-1) * Vector3.up;
        
        go.GetComponent<Image>().sprite = resultsSprite[_result];

    }

}
