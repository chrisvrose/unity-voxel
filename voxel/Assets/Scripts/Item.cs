using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item object is passed around by game to simplify stuff
/// Represents a bunch of objects, or a singular. 
/// Also used to implement item stacks.
/// Stacks are counted as 
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
            //Some small validation
            if (value == blocktypes.Invalid)
            {
                count = 0;
            }
            blockType = value;
        }
    }

    protected short count;

    /// <summary>
    /// Item piece creation
    /// </summary>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public Item(blocktypes b,short c)
    {
        blockType = c>0?b:blocktypes.Invalid;
        count = c > 0 ? c : (short)1;
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

    
    public short GetCount()
    {
        return count;
    }

    /// <summary>
    /// Change count by Delta. Accepts int, but changes to short
    /// </summary>
    /// <returns>New count</returns>
    public short DeltaCount(int change)
    {
        if (count + change > 0)
        {
            return count = (short)(count + change);
        }
        else
        {
            return count = 0;
        }
    }
}
