using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    static UIManager instence;
    public TextMeshProUGUI deathText, orbText, timeText, gameOverText;
    void Awake()
    {
        if (instence != null)
        {
            Destroy(gameObject);
            return;
        }
        instence = this;

        DontDestroyOnLoad(gameObject);
    }

    public static void UpdateOrbUI(int orbCount) {
        instence.orbText.text = orbCount.ToString();
    }

    public static void UpdateDeathUI(int deathCount)
    {
        instence.deathText.text = deathCount.ToString(); 
    }

    public static void UpdateTimeUI(float times)
    {
        int minutes = (int)(times / 60);
        float seconds = times % 60;
        instence.timeText.text = minutes.ToString("00")+":"+seconds.ToString("00");
    }

    public static void DisplayGameOver() {
        instence.gameOverText.enabled = true;
    }
}
