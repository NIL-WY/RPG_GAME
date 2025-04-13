using System.Collections;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTrigger : MonoBehaviour
{
    [Tooltip("预设的敌人ID列表（可在 Inspector 中设置）")]
    public string[] enemyIDs = new string[] { "Crow", "Crow" };
    public GameObject player;
    [SerializeField] DialogueTreeController dialogue;
    private void OnTriggerEnter(Collider other)
    {
       
            dialogue.StartDialogue();
        
    }
    public void battlestart()
    {
        // 记录主角进入战斗前的位置
        
        PreBattleData.playerPosition = player.transform.position;


        // 遍历队伍中其他成员，假设使用 PartyManager 中记录的 ID查找场景中的对象
        PartyData party = PartyManager.Instance.currentParty;
        // 构造战斗配置
        BattleConfig config = new BattleConfig();
        config.heroIDs = party.memberIDs; // 队伍中所有英雄
        config.enemyIDs = enemyIDs;       // 设置战斗中生成的敌人
        BattleSetupManager.Instance.PrepareBattle(config);
        Debug.Log("战斗配置准备完成，英雄数：" + config.heroIDs.Length);

        // 使用 BattleLoader 重载战斗场景
        BattleLoader loader = FindObjectOfType<BattleLoader>();
        if (loader == null)
        {
            // 如果场景中没有 BattleLoader 对象，则创建一个临时对象
            GameObject obj = new GameObject("BattleLoader");
            loader = obj.AddComponent<BattleLoader>();
        }
        StartCoroutine(loader.ReloadBattleScene());

        // 销毁触发器防止重复触发
        Destroy(gameObject);
    }
}
