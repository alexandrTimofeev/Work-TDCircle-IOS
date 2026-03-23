using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectWindow : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private Sprite levelLockSprite;
    [SerializeField] private Sprite levelUnLockSprite;
    [SerializeField] private Sprite levelNextSprite;

    [Space]
    [SerializeField] private Color levelLockColor = Color.gray;
    [SerializeField] private Color levelUnLockColor = Color.white;
    [SerializeField] private Color levelNextColor = Color.yellow;

    public static int CurrentLvl { get; private set; } = 0;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        int lastLvl = TestBooleans.GetValue("AllLevel") ? 99 : PlayerPrefs.GetInt("LastLvl", 0);
        Debug.Log($"LastLvl {lastLvl}");
        for (int i = 0; i < buttons.Length; i++)
        {
            InitButton(buttons[i], i, lastLvl);
        }
    }

    private void InitButton(Button button, int i, int lastLvl)
    {
        int n = i;
        button.onClick.AddListener(() => LoadLevel(n));
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = LevelData.LoadFromResources(n).ID;
        button.transform.GetComponent<Image>().sprite = LevelData.LoadFromResources(n).Background;

        Image image = button.transform.GetChild(1).GetComponent<Image>();
        if (i < lastLvl)
        {
            image.sprite = levelUnLockSprite;
            image.color = levelUnLockColor;
            return;
        }
        else if (i == lastLvl)
        {
            image.sprite = levelNextSprite;
            image.color = levelNextColor;
            return;
        }

        image.sprite = levelLockSprite;
        image.color = levelLockColor;
        button.interactable = false;
    }

    public static void LoadLevel(int n)
    {
        CurrentLvl = n;
        //GameEntryGameplayCCh.DataLevel = MyResources.Load<LevelData>($"Levels/Level_{n}");
        GameSceneManager.LoadGame();
    }

    public void StartLastLevel()
    {
        int lastLvl = Mathf.Clamp(PlayerPrefs.GetInt("LastLvl", 0), 0, buttons.Length - 1);
        LoadLevel(lastLvl);
    }

    public static void CompliteLvl()
    {
        int lastLvl = PlayerPrefs.GetInt("LastLvl", 0);
        if (CurrentLvl == lastLvl)
            PlayerPrefs.SetInt("LastLvl", lastLvl + 1);
    }
}
