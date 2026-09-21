using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateNicknamePanel : LobbyPanelBase
{
    [Space(5f)]
    [Header("Local Variables")]
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private Button createNicknameBtn;

    private const int MIN_CHAR_NICHNAME = 2;

    public override void InitPanel(LobbyUIManager uiManager)
    {
        base.InitPanel(uiManager);
        createNicknameBtn.interactable = false;
        createNicknameBtn.onClick.AddListener(OnClickCreateNickname);
        nicknameInputField.onValueChanged.AddListener(OnInputValueChange);
    }

    private void OnClickCreateNickname()
    {
        string nickName = nicknameInputField.text;

        if (nickName.Length >= MIN_CHAR_NICHNAME)
        {
            GlobalManagers.Instance.networkRunnerController.SetPlayerName(nickName);

            base.ClosePanel();
            lobbyUIManager.ShowPanel(LobbyPanelType.RegisteredSectionPanel);
        }
    }

    private void OnInputValueChange(string arg)
    {
        createNicknameBtn.interactable = arg.Length >= MIN_CHAR_NICHNAME;
    }
}
