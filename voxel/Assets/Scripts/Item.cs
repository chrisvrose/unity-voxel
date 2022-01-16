using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item object is passed around by game to encap the beautiful enums
/// </summary>
public class Item {
    /// <summary>
    /// Set at Item creation, readonly
    /// </summary>
    public blocktypes blockType;

    /// <summary>
    /// Item piece creation
    /// </summary>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public Item(blocktypes b)
    {
        blockType = b;
        //count = c > 0 ? c : (short)1;
    }

    public Item()
    {
        blockType = blocktypes.Invalid;
    }

    /// <summary>
    /// Get type of item
    /// </summary>
    /// <param name="someType"></param>
    /// <returns></returns>
    public bool IsType(blocktypes someType)
    {
        return someType == blockType;
    }
}
