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
    protected blocktypes blockType;
    public blocktypes Type {
        get
        {
            return blockType;
        }
        set
        {
            blockType = value;
        }
    }
    

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
