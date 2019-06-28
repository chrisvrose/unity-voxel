﻿using System.Collections;
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
    readonly private blocktypes blockType;

    private short count;

    /// <summary>
    /// Validate new Item Creation
    /// </summary>
    /// <param name="b"></param>
    /// <param name="c"></param>
    Item(blocktypes b,short c)
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
