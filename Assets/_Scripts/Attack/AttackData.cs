using System.Collections.Generic;
using UnityEngine;

public abstract class AttackData : ScriptableObject
{
    public string attackId;
    public string attackName;
    [SerializeField] private List<string> levelDescriptions = new();
    public Sprite icon;
    public Attack prefab;

    public string GetLevelDescription(int level) => levelDescriptions[Mathf.Clamp(level, 0, levelDescriptions.Count - 1)];
    public int maxLevel => levelDescriptions.Count - 1;
}
