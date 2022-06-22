using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{

    public int level;
    public float currentXp;
    public float requiredXp;

    private float lerpTimer;
    private float delayTimer;
    [Header("UI")]
    public Image frontXpBar;
    public Image backXpBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    [Header("Multipliers")]
    [Range(1f,300f)]
    public float additionMultiplier = 300;
    [Range(2f, 4f)]
    public float powerMultiplier = 2;
    [Range(7f, 14f)]
    public float divisionMulitplier = 7;

    public UnityEvent LevelUpMenu;
    public static bool GameIsPaused = false;

    public GameObject LevelUpUI;
    public WinScreen winScreen;


    // Start is called before the first frame update
    void Start()
    {
        frontXpBar.fillAmount = currentXp / requiredXp;
        backXpBar.fillAmount = currentXp / requiredXp;
        requiredXp = CalculateRequiredXp();
        levelText.text = "Level " + level;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateXpUI();
        if (Input.GetKeyDown(KeyCode.Equals))
        {

            GainExperienceFlatRate(20);
        }

        if (currentXp > requiredXp)
        {
            LevelUp();
        }

    }
    public void UpdateXpUI()
    {
        float xpFraction = currentXp / requiredXp;
        float FXP = frontXpBar.fillAmount;
        if(FXP < xpFraction)
        {
            delayTimer += Time.deltaTime;
            backXpBar.fillAmount = xpFraction;
            if (delayTimer > 3)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / 4;
                frontXpBar.fillAmount = Mathf.Lerp(FXP, backXpBar.fillAmount, percentComplete);
            }


        }
        xpText.text = currentXp + "/" + requiredXp;
    }
    public void GainExperienceFlatRate(float xpGained)
    {
        currentXp += xpGained;
        lerpTimer = 0f;
        delayTimer = 0f;
    }

    public void GainExperienceScalable(float xpGained, int passedLevel)
    {
        if (passedLevel < level)
        {
            float multiplier = 1 + (level - passedLevel) * 0.1f;
            currentXp += xpGained * multiplier;

        }
        else
        {
            currentXp += xpGained;

        }
        lerpTimer = 0f;
        delayTimer = 0f;
    }

    public void LevelUp()
    {
        level++;
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        currentXp = Mathf.RoundToInt(currentXp - requiredXp);
        GetComponent<PlayerHealth>().IncreaseHealth(level);
        requiredXp = CalculateRequiredXp();
        levelText.text = "Level " + level;
        LevelUpMenu?.Invoke();
        Pause();
    }

    private int CalculateRequiredXp()
    {
        int solveForRequiredXp = 0;
        for (int levelCycle = 1; levelCycle <= level; levelCycle++ )
        {
            solveForRequiredXp += (int)Mathf.Floor(levelCycle + additionMultiplier * Mathf.Pow(powerMultiplier, levelCycle - divisionMulitplier));

        }
        return solveForRequiredXp / 4;
    }

    public void Resume()
    {

        LevelUpUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

    }

    public void Pause()
    {
        LevelUpUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
