using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    int[,] nums = { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 } };

    private void Awake()
    {
        bool row = false;

        int l1 = row ? nums.GetLength(0) : nums.GetLength(1);
        int l2 = row ? nums.GetLength(1) : nums.GetLength(0);

        for (int i = 0; i < l1; i++)
        {
            for (int j = 0; j < l2; j++)
            {
                int r = row ? i : j;
                int c = row ? j : i;

                Debug.Log($"{nums[r,c]}");
            }
        }
    }
}
