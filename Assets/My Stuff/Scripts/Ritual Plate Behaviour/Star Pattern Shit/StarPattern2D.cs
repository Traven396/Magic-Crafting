using Array2DEditor;
using System;
using UnityEngine;


[Serializable]
public class StarPattern2D : Array2D<int>
{
    [SerializeField]
    CellRowInt[] cells;
    public StarPattern2D(int size)
    {
        cells = new CellRowInt[size];
    }

    protected override CellRow<int> GetCellRow(int idx)
    {
        return cells[idx];
    }
    public CellRow<int> GetCellRowPublic(int idx)
    {
        return cells[idx];
    }
} 
