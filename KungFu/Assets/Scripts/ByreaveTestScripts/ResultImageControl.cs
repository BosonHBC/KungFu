﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultImageControl : MonoBehaviour
{
    
    //0 perfect 1 good 2 miss
    public SpriteRenderer[] Images;
    // Start is called before the first frame update


    public void ShowResult(HitResult hitResult)
    {
        gameObject.GetComponentInChildren<ParticleSystem>().Play();
        switch(hitResult)
        {
            case HitResult.Perfect:
                StartCoroutine(ShowImage(Images[0]));
                break;
            case HitResult.Good:
                StartCoroutine(ShowImage(Images[1]));
                break;
            case HitResult.Miss:
                StartCoroutine(ShowImage(Images[2]));
                break;
            default:
                break;
        }
    }


    IEnumerator ShowImage(SpriteRenderer imageToShow, float timeToShow = 1.0f)
    {
        imageToShow.color = new Color(imageToShow.color.r, imageToShow.color.g, imageToShow.color.b, 1.0f);
        while (imageToShow.color.a >= 0.0f)
        {
            imageToShow.color = new Color(imageToShow.color.r, imageToShow.color.g, imageToShow.color.b, imageToShow.color.a - Time.deltaTime);
            imageToShow.gameObject.transform.Translate(Vector3.up * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime * timeToShow);
        }
        imageToShow.color = new Color(imageToShow.color.r, imageToShow.color.g, imageToShow.color.b, 0.0f);
        imageToShow.gameObject.transform.position = gameObject.transform.position;
        Destroy(gameObject, 1.0f);
    }
}