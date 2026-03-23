using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private Button _buttonSound;
    [SerializeField] private Sprite _spriteSoundOn;
    [SerializeField] private Sprite _spriteSoundOff;
    [SerializeField] private Color _colorSoundOn = Color.white;
    [SerializeField] private Color _colorSoundOff = Color.gray;

    [Header("Vibration Settings")]
    [SerializeField] private Button _buttonVibration;
    [SerializeField] private Sprite _spriteVibrationOn;
    [SerializeField] private Sprite _spriteVibrationOff;
    [SerializeField] private Color _colorVibrationOn = Color.white;
    [SerializeField] private Color _colorVibrationOff = Color.gray;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        // Привязываем методы к кнопкам
        _buttonSound?.onClick.AddListener(SwitchSound);
        _buttonVibration?.onClick.AddListener(SwitchVibration);

        // Подписываемся на изменения настроек
        GameSettings.OnSoundChange += UpdateSoundButtonImage;
        GameSettings.OnVibrationChange += UpdateVibrationButtonImage;

        // Обновляем кнопки при запуске
        UpdateSoundButtonImage(GameSettings.IsSoundPlay);
        UpdateVibrationButtonImage(GameSettings.IsVibrationPlay);
    }

    private void UpdateSoundButtonImage(bool isSoundOn)
    {
        if (_buttonSound == null)
            return;

        _buttonSound.GetComponent<Image>().sprite = isSoundOn ? _spriteSoundOn : _spriteSoundOff;
        _buttonSound.GetComponent<Image>().color = isSoundOn ? _colorSoundOn : _colorSoundOff;
    }

    private void UpdateVibrationButtonImage(bool isVibrationOn)
    {
        if (_buttonVibration == null)
            return;

        _buttonVibration.GetComponent<Image>().sprite = isVibrationOn ? _spriteVibrationOn : _spriteVibrationOff;
        _buttonVibration.GetComponent<Image>().color = isVibrationOn ? _colorVibrationOn : _colorVibrationOff;
    }

    private void SwitchSound()
    {
        GameSettings.SetSound(!GameSettings.IsSoundPlay);
    }

    private void SwitchVibration()
    {
        GameSettings.SetVibration(!GameSettings.IsVibrationPlay);
    }

    private void OnEnable()
    {
        Open();
    }

    private void Open()
    {
        UpdateSoundButtonImage(GameSettings.IsSoundPlay);
        UpdateVibrationButtonImage(GameSettings.IsVibrationPlay);
    }

    private void OnDestroy()
    {
        // Отписываемся от событий, чтобы избежать утечек памяти
        GameSettings.OnSoundChange -= UpdateSoundButtonImage;
        GameSettings.OnVibrationChange -= UpdateVibrationButtonImage;
    }
}