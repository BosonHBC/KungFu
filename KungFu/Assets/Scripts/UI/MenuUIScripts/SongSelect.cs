using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongSelect : MonoBehaviour
{
    [SerializeField]
    GameObject [] SelectArrow;
    [SerializeField]
    int currentSongIndex = 0;
    private void OnEnable()
    {
        MenuCanvasControl.OnSelectLeft += SongSelectLeft;
        MenuCanvasControl.OnSelectRight += SongSelectRight;
    }
    void SongSelectLeft(MenuCanvasControl.MenuCanvas currentCanvas)
    {
        if(currentCanvas == MenuCanvasControl.MenuCanvas.SongSelect)
        {
            SelectArrow[currentSongIndex].SetActive(false);
            currentSongIndex -= 1;
            if (currentSongIndex < 0)
                currentSongIndex = SelectArrow.Length - 1;
            SelectArrow[currentSongIndex].SetActive(true);
            MyGameInstance.instance.SongIndex = currentSongIndex;
        }
    }
    void SongSelectRight(MenuCanvasControl.MenuCanvas currentCanvas)
    {
        if (currentCanvas == MenuCanvasControl.MenuCanvas.SongSelect)
        {
            SelectArrow[currentSongIndex].SetActive(false);
            currentSongIndex = (currentSongIndex + 1) % SelectArrow.Length;
            SelectArrow[currentSongIndex].SetActive(true);
            MyGameInstance.instance.SongIndex = currentSongIndex;
        }
    }
    private void OnDisable()
    {
        MenuCanvasControl.OnSelectLeft -= SongSelectLeft;
        MenuCanvasControl.OnSelectRight -= SongSelectRight;
    }
}
