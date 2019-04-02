using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class DataUtility
{
    public static int[] GetIntArrayFromJSONNode(JSONNode node)
    {
        if (node.Count == 0)
            return null;
        int[] retArr = new int[node.Count];
        for (int i = 0; i < node.Count; ++i)
        {
            retArr[i] = node[i].AsInt;
        }
        return retArr;
    }
    public static bool HasElement(int[] array, int element)
    {
        foreach (int i in array)
        {
            if (i == element)
                return true;
        }
        return false;
    }
    public static bool AllTrue(bool[] boolArray)
    {
        foreach(bool b in boolArray)
        {
            if (!b)
                return false;
        }
        return true;
    }
    public static bool DictionaryAllTrue(Dictionary<int, bool> dic)
    {
        foreach(bool b in dic.Values)
        {
            if (!b)
                return false;
        }
        return true;
    }
    public static int IntArrayIndex(int[] intArray, int value)
    {
        for(int i = 0; i < intArray.Length; ++ i)
        {
            if (value == intArray[i])
                return i;
        }
        return -1;
    }
}
