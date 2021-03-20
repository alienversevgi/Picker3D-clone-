using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GameData : ScriptableObject
{
    public int CurrentLevelIndex;
    public bool AreAllLevelCompleted;

    public List<LevelData> Levels;
}
