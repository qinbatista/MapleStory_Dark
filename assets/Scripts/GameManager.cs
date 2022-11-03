using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//重置场景需要的类

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    static GameManager instence;
    Fader fader;
    List<shrineCollecte> orbs;

    PlayerHeath playerHeath;

    GateAnimation gate;
    float gameTime;
    bool isGameOver;

    public int DeathNums;

    void Awake()
    {
        if (instence != null) {
            Destroy(gameObject);
            return;
        }
        instence = this;

        orbs = new List<shrineCollecte>();

        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (instence.isGameOver)
            return;

        if (instence.orbs.Count == 0 && gate.isOpen==false && !instence.playerHeath.isDead) {
            gate.Open();
        }
        gameTime += Time.deltaTime;

        UIManager.UpdateTimeUI(gameTime);
    }
    //注册的方式获取Fader
    public static void RegisterFader(Fader obj) {
        instence.fader = obj;

     }

    public static void RegisterOrb(shrineCollecte obj)
    {
        //当前列表是否包含注册的orb对象，不包含则添加
        if (!instence.orbs.Contains(obj)) 
            instence.orbs.Add(obj);

        UIManager.UpdateOrbUI(instence.orbs.Count);
    }
    public static void PlayerGetOrbs(shrineCollecte obj)
    {
        //当前列表是否包含注册的orb对象，不包含则添加
        if (!instence.orbs.Contains(obj))
            return;
        instence.orbs.Remove(obj);
        UIManager.UpdateOrbUI(instence.orbs.Count);
    }

    public static void RegisterGate(GateAnimation obj) {
        instence.gate = obj;
    }

    public static void RegisterPlayerHeath(PlayerHeath obj)
    {
        instence.playerHeath = obj;
    }

    public static void DeadRest()
    {

        instence.orbs.Clear();
        instence.DeathNums++;
        UIManager.UpdateDeathUI(instence.DeathNums);
        instence.Invoke("ResetScene", 1f);
        AudioManger.PlayLevelAudio();
    }
    public static bool isGameOverBool() {
        return instence.isGameOver;
    }
    public static void PlayerWin() {
        instence.isGameOver = true;
        UIManager.DisplayGameOver();
        AudioManger.PlayWinAudio();
    }

    private void ResetScene()
    {
        instence.fader.FadeOut();
        //重新加载场景loadscene，参数（场景编号）需要在界面buildSetting中添加当前场景

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
 
}
