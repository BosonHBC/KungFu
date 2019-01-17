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


    
    // Start is called before the first frame update
    void Start()
    {
        resultParent = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowResult(int _result)
    {
        // Instantiate object
        GameObject go = Instantiate(resultPrefab, resultParent, false);
       // go.transform.position += (resultParent.childCount-1) * Vector3.up;
        
        go.GetComponent<Image>().sprite = resultsSprite[_result];

    }

}
