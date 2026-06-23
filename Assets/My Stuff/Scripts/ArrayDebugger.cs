using UnityEngine;

public class ArrayDebugger : MonoBehaviour
{
    public StarPattern Pattern;


    private void Start()
    {
        for (int i = 0; i < Pattern.StarPatternGrid.GridSize.x; i++)
        {
            for (int j = 0; j < Pattern.StarPatternGrid.GridSize.y; j++)
            {
                if(Pattern.StarPatternGrid.GetCell(i, j) != 0)
                    Debug.Log($"Value at {i}, {j} is {Pattern.StarPatternGrid.GetCell(i, j)}");
            }
        }


    }
}
