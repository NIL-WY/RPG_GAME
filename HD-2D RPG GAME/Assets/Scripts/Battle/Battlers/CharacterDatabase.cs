using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    [System.Serializable]
    public class CharacterData
    {
        public string id;
        public GameObject prefab;
        [TextArea] public string description;
    }

    [Header("英雄数据")]
    public List<CharacterData> heroes = new();

    [Header("敌人数据")]
    public List<CharacterData> enemies = new();

    public GameObject GetPrefab(string id, bool isHero)
    {
        var targetList = isHero ? heroes : enemies;
        return targetList.Find(c => c.id == id)?.prefab;
    }
}