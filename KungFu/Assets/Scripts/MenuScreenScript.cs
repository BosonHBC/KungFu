using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreenScript : MonoBehaviour
{

    public float delay;

    // Start is called before the first frame update
    public void StartGame()
    {
        Invoke("StartAfterDelay",delay);
    }

    private void StartAfterDelay()
    {
        SceneManager.LoadScene("BosonScene");
    }

}
