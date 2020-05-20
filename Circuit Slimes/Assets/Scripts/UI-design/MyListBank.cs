using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The example of the simplest ListBank
public class MyListBank : BaseListBank
{
    private int[] _contents = {
        0, 1, 2
    };

    public override string GetListContent(int index)
    {
        return _contents[index].ToString();
    }

    public override int GetListLength()
    {
        return _contents.Length;
    }

    public int GetSelectedContentID(int contentID)
    {
        Debug.Log("Selected content ID: " + contentID.ToString() +
            ", Content: " + GetListContent(contentID));

        return contentID;
    }
}


