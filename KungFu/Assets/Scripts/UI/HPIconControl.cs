using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class HPIconControl : MonoBehaviour
{
    [SerializeField] Image[] iconImages;
    [SerializeField] Sprite[] IconSprites;
    [SerializeField] Image[] nameImages;
    [SerializeField] Sprite[] NameSprites;
    
    public void SetIconAndName(int _player, int _enemy)
    {
        iconImages[0].sprite = IconSprites[_player];
        iconImages[1].sprite = IconSprites[_enemy];

        nameImages[0].sprite = NameSprites[_player];
        nameImages[1].sprite = NameSprites[_enemy];
    }
}
