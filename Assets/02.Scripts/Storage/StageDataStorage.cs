using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageDataStorage : Storage<StageData>
{
    private static Dictionary<int, StageData> Stages
    {
        get
        {
            if (_stages == null)
            {
                _stages = new Dictionary<int, StageData>();

                var stages = Resources.LoadAll<StageData>("StageData");

                for (int i = 0; i < stages.Length; i++)
                {
                    _stages[stages[i].Level] = stages[i];
                }
            }

            return _stages;
        }
    }

    private static Dictionary<int, StageData> _stages;

    public static StageData[] Get()
    {
        return Stages.Values.ToArray();
    }

    public static StageData Get(int level)
    {
        return Stages.TryGetValue(level, out var stage) ? stage : null;
    }
}
