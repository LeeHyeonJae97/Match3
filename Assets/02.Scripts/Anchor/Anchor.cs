using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// anchor to keep reference between scenes
/// </summary>
/// <typeparam name="T"></typeparam>
public class Anchor<T> : ScriptableObject where T : Object
{
    [field: SerializeField] public T Value { get; set; }
}
