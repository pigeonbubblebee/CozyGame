using System;
using System.Collections;
using System.Collections.Generic;

public class ItemComparer : IComparer<Item>
{
    // Compares Items
    public int Compare(Item a, Item b) {
        return a.CompareTo(b);
    }
}