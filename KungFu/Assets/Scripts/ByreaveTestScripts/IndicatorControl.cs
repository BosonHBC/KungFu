using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorControl : MonoBehaviour
{
    
    public GameObject ResultImageShow;
    [SerializeField]
    GameObject [] ChildBodyParts;
    public Material activateMat;
    public Material deactivateMat;
    public Material matchMat;
    public Material hitMat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //hightlight the parts to hit
    public void ActivateButton(int[] ButtonIDs)
    {
        foreach (int i in ButtonIDs)
        {
            //Debug.Log(i);
            ChildBodyParts[i].GetComponent<MeshRenderer>().material = activateMat;
        }
    }

    public void MatchButton(int ButtonID)
    {
        ChildBodyParts[ButtonID].GetComponent<MeshRenderer>().material = matchMat;
    }

    //there is a hit
    public void HitButton(int ButtonID)
    {
        //PlayRandomMissSFX();
        ChildBodyParts[ButtonID].GetComponent<MeshRenderer>().material = hitMat;
    }
    //hit released
    public void DeactivateButtons(int[] ButtonIDs)
    {
        foreach (int i in ButtonIDs)
        {
            ChildBodyParts[i].GetComponent<MeshRenderer>().material = deactivateMat;
        }
    }

    public void DeactiveButton(int ButtonID)
    {
        ChildBodyParts[ButtonID].GetComponent<MeshRenderer>().material = deactivateMat;
    }

    public void ShowResultAt(int ButtonID, HitResult hitResult)
    {
        if (ButtonID >= ChildBodyParts.Length)
            return;
        Transform locTrans = ChildBodyParts[ButtonID].transform;
        GameObject ri = Instantiate(ResultImageShow, locTrans.position, transform.rotation);
        ri.GetComponent<ResultImageControl>().ShowResult(hitResult);
    }
}
