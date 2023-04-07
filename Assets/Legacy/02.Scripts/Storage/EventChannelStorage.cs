using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChannelStorage : Storage<EventChannel>
{
    private static Dictionary<string, EventChannel> Channels
    {
        get
        {
            if (_channels == null)
            {
                _channels = new Dictionary<string, EventChannel>();

                var channels = Resources.LoadAll<EventChannel>("EventChannel");

                for (int i = 0; i < channels.Length; i++)
                {
                    _channels[channels[i].GetType().ToString()] = channels[i];
                }
            }

            return _channels;
        }
    }

    private static Dictionary<string, EventChannel> _channels;

    public static T Get<T>() where T : EventChannel
    {
        return Channels.TryGetValue(typeof(T).ToString(), out var channel) ? channel as T : null;
    }
}
