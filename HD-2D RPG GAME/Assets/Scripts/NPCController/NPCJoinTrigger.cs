using System.Collections;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using UnityEngine;

public class NPCJoinTrigger : MonoBehaviour
{
    [Tooltip("NPC 对应的英雄 ID，确保在角色数据库中有对应预制体")]
    public string npcHeroID = "NPC_Hero";
    [SerializeField] DialogueTreeController dialogue;
    private bool hasJoined = false;
    [SerializeField] Blackboard bb;
    private void OnTriggerStay(Collider other)
    {
        if (hasJoined) return;
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                dialogue.StartDialogue();
            }
        }
    }

    public void Join()
    {

        PartyData party = PartyManager.Instance.currentParty;
        List<string> members = new List<string>(party.memberIDs);

        if (members.Count < 2 && !members.Contains(npcHeroID))
        {
            members.Add(npcHeroID);
            party.memberIDs = members.ToArray();
            Debug.Log("NPC 加入队伍: " + npcHeroID);
            hasJoined = true;

            // 为 NPC 添加跟随组件
            // 假设 NPC 的触发器为其子物体，NPC 根对象为 transform.parent
            Transform npcRoot = transform.parent;
            if (npcRoot != null)
            {
                NPCFollower follower = npcRoot.GetComponent<NPCFollower>();
                if (follower == null)
                {
                    follower = npcRoot.gameObject.AddComponent<NPCFollower>();
                    // 设置跟随目标为带有 "Player" 标签的对象
                    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                    if (playerObj != null)
                    {
                        follower.target = playerObj.transform;
                    }
                }
            }

            // 销毁触发器区域对象（仅销毁该子物体）
            Destroy(gameObject);
        }
    }
}
