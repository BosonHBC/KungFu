using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultImageControl : MonoBehaviour
{

    //0 perfect 1 good 2 miss
    public Image[] Images;
    // Start is called before the first frame update

    ParticleSystem ps;
    public void ShowResult(HitResult hitResult)
    {
        //gameObject.GetComponentInChildren<ParticleSystem>().Play();
        switch (hitResult)
        {
            case HitResult.Perfect:
                StartCoroutine(ShowImage(0));                 
                break;
            case HitResult.Good:
                StartCoroutine(ShowImage(1));
                break;
            case HitResult.Miss:
                StartCoroutine(ShowImage(2));
                break;
            case HitResult.Mismatch:
                //StartCoroutine(ShowImage(Images[3]));
                break;
            default:
                break;
        }
    }


    IEnumerator ShowImage(int id, float timeToShow = 1f)
    {
        Image imageToShow = Images[id];
        ps = transform.GetChild(id).GetComponent<ParticleSystem>();
        if (ps)
        {
            ps.gameObject.SetActive(true);
            ps.Play(true);
        }
        imageToShow.color = new Color(imageToShow.color.r, imageToShow.color.g, imageToShow.color.b, 1.0f);
        while (imageToShow.color.a >= 0.0f)
        {
            imageToShow.color = new Color(imageToShow.color.r, imageToShow.color.g, imageToShow.color.b, imageToShow.color.a - Time.deltaTime);
            imageToShow.gameObject.transform.Translate(Vector3.up * Time.deltaTime / 3.0f);
            yield return new WaitForSeconds(Time.deltaTime * timeToShow);
        }
        imageToShow.color = new Color(imageToShow.color.r, imageToShow.color.g, imageToShow.color.b, 0.0f);
        imageToShow.gameObject.transform.position = gameObject.transform.position;
        Destroy(gameObject, 0.85f);
    }
}
