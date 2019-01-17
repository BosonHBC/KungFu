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
    Image result;
    
    // Start is called before the first frame update
    void Start()
    {
        result = transform.GetChild(0).GetComponent<Image>();
        result.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowResult(int _result)
    {
        result.gameObject.SetActive(true);
        result.sprite = resultsSprite[_result];

        StartCoroutine(DelayResultDisappear(1f));
    }

    IEnumerator DelayResultDisappear(float _time)
    {
        yield return new WaitForSeconds(_time);
        result.gameObject.SetActive(false);
    }
}
