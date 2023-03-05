using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stage : MonoBehaviour
{    
    public static Stage Instance { get; private set; }

    public StageData Data => _stageDataAnchor.Value;
    public Board Board => _board;
    public int RemainTryCount
    {
        get { return _remainTryCount; }

        set
        {
            if (value < 0) return;

            _remainTryCount = value;
            if (_remainTryCount == 0)
            {
                _gameEventChannel.Fail();
            }
            _stageEventChannel.UpdateCurrentTryCount(_remainTryCount);
        }
    }
    public int RemainClearCount
    {
        get { return _remainClearCount; }

        set
        {
            if (value < 0) return;

            _remainClearCount = value;
            if (_remainClearCount == 0)
            {
                _gameEventChannel.Clear();
            }
            _stageEventChannel.UpdateCurrentClearCount(_remainClearCount);
        }
    }

    [SerializeField] private Board _board;
    [SerializeField] private StageDataAnchor _stageDataAnchor;
    private GameEventChannel _gameEventChannel;
    private StageEventChannel _stageEventChannel;
    private int _remainTryCount;
    private int _remainClearCount;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _gameEventChannel = EventChannelStorage.Get<GameEventChannel>();
        _stageEventChannel = EventChannelStorage.Get<StageEventChannel>();
    }

    private void Start()
    {
        _board.Initialize(Data);

        RemainTryCount = Data.TryCount;
        RemainClearCount = Data.ClearCondition.Count;
    }

    private void OnEnable()
    {
        _stageEventChannel.OnBoardItemRemoved += OnBoardItemRemoved;
    }

    private void OnDisable()
    {
        _stageEventChannel.OnBoardItemRemoved -= OnBoardItemRemoved;
    }

    /// <summary>
    /// called when item removed to check clear condition
    /// </summary>
    /// <param name="data"></param>
    private void OnBoardItemRemoved(BoardItemData data)
    {
        if (data.Type == Data.ClearCondition.Type && data.Color == Data.ClearCondition.Color)
        {
            RemainClearCount -= 1;
        }
    }
}
