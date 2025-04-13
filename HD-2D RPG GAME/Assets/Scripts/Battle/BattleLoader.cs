using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleLoader : MonoBehaviour
{
    private const string BATTLE_SCENE = "Battle";

    /// <summary>
    /// 重新加载战斗场景，确保每次加载前先卸载已存在的战斗场景
    /// </summary>
    public IEnumerator ReloadBattleScene()
    {
        // 确保战斗场景完全卸载
        if (SceneManager.GetSceneByName(BATTLE_SCENE).isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(BATTLE_SCENE);
            while (!unloadOp.isDone)
                yield return null;
        }

        // 加载新战斗场景
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(BATTLE_SCENE, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = true;
        while (!loadOp.isDone)
            yield return null;

        // 确保场景激活
        Scene battleScene = SceneManager.GetSceneByName(BATTLE_SCENE);
        SceneManager.SetActiveScene(battleScene);
    }
}
