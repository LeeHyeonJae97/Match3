using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Match", menuName = "ScriptableObject/Board/Match")]
public class MatchStrategy : ScriptableObject
{
    [SerializeField] private LegBoardData _boardData;

    public void Match(int row, int column)
    {
        MatchRow();
        MatchColumn();

        // LOCAL FUNCTION
        void MatchRow()
        {
            int minColumn = Mathf.Max(0, column - 2);
            int maxColumn = Mathf.Min(_boardData.Column - 1, column + 2);

            for (int c = minColumn; c <= maxColumn; c++)
            {
                // check
            }
        }

        void MatchColumn()
        {
            int minRow = Mathf.Max(0, row - 2);
            int maxRow = Mathf.Min(_boardData.Row - 1, row + 2);

            for (int r = minRow; r <= maxRow; r++)
            {
                // check
            }
        }
    }
}
