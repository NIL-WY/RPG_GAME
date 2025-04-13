using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvaChange : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    // Start is called before the first frame update
    public void Loadforest()
    {
        SceneManager.LoadScene("forest");
    }
    public void LoadMain()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void ActivateEndPanel()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true);
            // 扩展点：可在此处添加音效触发/暂停游戏逻辑
            // Time.timeScale = 0f; 
        }
        else
        {
            Debug.LogError("End Panel reference is missing!");
        }
    }

}
