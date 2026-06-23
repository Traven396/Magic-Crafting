using Array2DEditor;
using UnityEditor;
using UnityEngine;
using MyNamespace;

namespace MyNamespace
{
    [CustomPropertyDrawer(typeof(StarPattern2D))]
    public class StarPropertyDrawer : Array2DDrawer
    {
        protected override object GetDefaultCellValue() => 0;

        protected override object GetCellValue(SerializedProperty cell) => cell.intValue;

        protected override void SetValue(SerializedProperty cell, object obj)
        {
            cell.intValue = (int)obj;
        }
    }


}