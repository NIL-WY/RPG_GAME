using System.Collections;
using System.Collections.Generic;
using NodeCanvas.DialogueTrees;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    [SerializeField] DialogueTreeController dialogue;
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            dialogue.StartDialogue();
        }
    }
}
