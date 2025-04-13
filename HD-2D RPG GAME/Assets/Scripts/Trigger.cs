using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Trigger : MonoBehaviour
{
    private const string BATTLE = "Battle";
    AsyncOperation async;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("转到战斗场景");
        StartBattle();
    }
    void StartBattle()
    {
        var config = new BattleConfig
        {
            heroIDs = new[] { "Fox", "Dual" }, // 最多2个
            enemyIDs = new[] { "Crow", "Crow" },   // 最多3个
            keepHeroCorpse = true
        };

        BattleSetupManager.Instance.PrepareBattle(config);
        async = SceneManager.LoadSceneAsync(BATTLE);
        async.allowSceneActivation = true;
    }
    
}
