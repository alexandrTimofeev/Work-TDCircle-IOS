using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildButton : MonoBehaviour
{
    [Space]
    [SerializeField] private Image image;
    [SerializeField] private GameObject GoTextEnergy;
    [SerializeField] private GameObject GoTextMaterial;
    [SerializeField] private TMP_Text textEnergy;
    [SerializeField] private TMP_Text textMaterial;

    public event Action<UIBuildButton> OnClickButton;
    public event Action<bool> OnChangeInteractable;

    public bool IsInteractable => button.interactable;

    private BuildDataPreset preset;
    private Button button;
    private int count;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnClickButton?.Invoke(this));
    }

    public BuildDataPreset GetData()
    {
        return preset;
    }

    public void Init(BuildDataPreset preset)
    {
        this.preset = preset;

        image.sprite = preset.Icon;

        UpdateData();
    }

    public void UpdateData(int count = -1)
    {
        if (count < 0)
            count = this.count;

        int canEnergy = preset.GetResourceCondition("Energy", count);
        int canMaterial = preset.GetResourceCondition("Material", count);
        textEnergy.text = "" + canEnergy;
        textMaterial.text = "" + canMaterial;
        GoTextEnergy.gameObject.SetActive(canEnergy > 0);
        GoTextMaterial.gameObject.SetActive(canMaterial > 0);

        SetInterctible(preset.TestData(count));
    }

    private void SetInterctible(bool interct)
    {
        button.interactable = interct;
        OnChangeInteractable?.Invoke(interct);

        image.color = interct ? new Color(1f, 1f, 1f, image.color.a) : new Color(0.5f, 0.5f, 0.5f, image.color.a);
    }
}