using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GetGUID : MonoBehaviour
{
  static public GUID _staticGetGUID()
    {
        return GUID.Generate();
    }
}
