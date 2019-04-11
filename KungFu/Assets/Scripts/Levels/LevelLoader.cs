using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    private void Awake()
    {
        if(instance==null || instance != this)
        {
            instance = this;
        }
    }
    //   [SerializeField] private CanvasGroup previousCanvas;
    private CanvasGroup loadingCanvas;
    private Image loadingFillBar;
    private Text loadingFillText;
    [SerializeField] private float minimumWaitTime = 2f;
    private float collpaseTime;
    // Start is called before the first frame update
    void Start()
    {
        loadingCanvas = GetComponent<CanvasGroup>();
        loadingCanvas.alpha = 0;
        loadingFillBar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        loadingFillText = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        
    }
    public void LoadScene(string _name)
    {
        MyGameInstance.instance.RestartGame();
        if (SceneManager.GetSceneByName(_name) != null)
        {
            StartCoroutine(LoadAsynchronously(_name));
        }
        else
            Debug.LogError("No such scene called " + _name);
    }

    IEnumerator LoadAsynchronously(string _name)
    {

        //previousCanvas.GetComponent<UIFader>().FadeOut(0.25f);
        //yield return new WaitForSeconds(0.25f);
        GetComponent<UIFader>().FadeIn(0.25f);
        yield return new WaitForSeconds(0.25f);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_name);
        //   SceneManager.LoadScene(_name);
        asyncOperation.allowSceneActivation = false;
        collpaseTime = 0;
        Debug.Log("Loading Scene");
        while (!asyncOperation.isDone || collpaseTime < minimumWaitTime)
        {
            collpaseTime += Time.deltaTime;
            
            float _waitTime = collpaseTime / minimumWaitTime;
            float _progress = Mathf.Clamp01(asyncOperation.progress/ 0.9f);
            _progress = Mathf.Min(_waitTime, _progress);
            loadingFillBar.fillAmount = _progress;
            loadingFillText.text = (int)(_progress * 100f) + "%";

            if(_progress >= 1f)
            {
                Debug.Log("Loading Done!");
                asyncOperation.allowSceneActivation = true;
            }
            FightingManager.instance.StartGame();
            yield return null;
        }
        
    }

}
