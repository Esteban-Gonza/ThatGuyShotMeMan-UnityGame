
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateNicknamePanel : LobbyPanelBase
{
    [Space(5f)]
    [Header("Local Variables")]
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private Button createNicknameBtn;

    [Header("Special Nickname Effect")]
    [SerializeField] private ParticleSystem heartFountainFX;

    private const int MIN_CHAR_NICKNAME = 2;
    private const int MAX_CHAR_NICKNAME = 6;
    private const float EFFECT_DISPLAY_TIME = 1.2f;

    private static readonly string[] SpecialNicknames =
    {
        "Pau",
        "Puala",
        "Paula"
    };

    private bool isSubmitting;

    public override void InitPanel(LobbyUIManager uiManager)
    {
        base.InitPanel(uiManager);

        nicknameInputField.characterLimit = MAX_CHAR_NICKNAME;
        createNicknameBtn.interactable = false;

        createNicknameBtn.onClick.AddListener(OnClickCreateNickname);
        nicknameInputField.onValueChanged.AddListener(OnInputValueChange);
    }

    private void OnClickCreateNickname()
    {
        if (isSubmitting)
            return;

        string nickname = nicknameInputField.text.Trim();

        if (nickname.Length < MIN_CHAR_NICKNAME)
            return;

        isSubmitting = true;
        createNicknameBtn.interactable = false;

        if (IsSpecialNickname(nickname) && heartFountainFX != null)
        {
            StartCoroutine(PlayEffectAndContinue());
        }
        else
        {
            ContinueToRegisteredSection();
        }
    }

    private IEnumerator PlayEffectAndContinue()
    {
        heartFountainFX.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        heartFountainFX.Play(true);

        yield return new WaitForSeconds(EFFECT_DISPLAY_TIME);

        ContinueToRegisteredSection();
    }

    private bool IsSpecialNickname(string nickname)
    {
        foreach (string specialName in SpecialNicknames)
        {
            if (string.Equals(
                nickname,
                specialName,
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private void ContinueToRegisteredSection()
    {
        base.ClosePanel();

        lobbyUIManager.ShowPanel(
            LobbyPanelType.RegisteredSectionPanel
        );
    }

    private void OnInputValueChange(string value)
    {
        if (isSubmitting)
            return;

        createNicknameBtn.interactable =
            value.Trim().Length >= MIN_CHAR_NICKNAME;
    }

    private void OnDestroy()
    {
        if (createNicknameBtn != null)
            createNicknameBtn.onClick.RemoveListener(OnClickCreateNickname);

        if (nicknameInputField != null)
            nicknameInputField.onValueChanged.RemoveListener(OnInputValueChange);
    }
}
