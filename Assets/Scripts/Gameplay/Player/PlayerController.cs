using Fusion;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 1000f;
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private TextMeshProUGUI playerNameText;

    [Networked] private NetworkString<_8> playerName { get; set; }
    [Networked] private NetworkButtons buttonsPrev { get; set; }
    private ChangeDetector changeDetector;

    private float horizontal;
    private Rigidbody2D rigidbody;
    private CinemachineCamera cinemachineCamera;
    
    private enum PlayerInputButtons
    {
        None,
        Jump
    }

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        rigidbody = GetComponent<Rigidbody2D>();

        SetPlayerName(playerName);
        SetLocalObjects();
    }

    private void SetLocalObjects()
    {
        if (!Object.HasInputAuthority)
            return;

        cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        cinemachineCamera.Follow = transform;

        playerCanvas.worldCamera = Camera.main;

        string localName =
            GlobalManagers.Instance.networkRunnerController.localPlayerName;

        RPC_SetNickname(localName);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetNickname(string nickname)
    {
        playerName = nickname;
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            if (change == nameof(playerName))
            {
                SetPlayerName(playerName);
            }
        }
    }

    private void SetPlayerName(NetworkString<_8> name)
    {
        playerNameText.text = $"{name} {Object.InputAuthority.PlayerId}";
    }

    public void BeforeUpdate()
    {
        if (!Object.HasInputAuthority)
            return;

        horizontal = 0f;

        if (Keyboard.current.aKey.isPressed)
            horizontal -= 1f;

        if (Keyboard.current.dKey.isPressed)
            horizontal += 1f;
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority,out PlayerData input))
        {
            rigidbody.linearVelocity = new Vector2(input.HorizontalInput * moveSpeed, rigidbody.linearVelocityY);
            CheckJumpInput(input);
        }
    }

    private void CheckJumpInput(PlayerData input)
    {
        NetworkButtons pressed = input.NetworkButtons.GetPressed(buttonsPrev);
        if (pressed.WasPressed(buttonsPrev, PlayerInputButtons.Jump))
        {
            rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
        }

        buttonsPrev = input.NetworkButtons;
    }

    public PlayerData GetPlayerNetworkInput()
    {
        PlayerData data = new PlayerData();
        data.HorizontalInput = horizontal;
        data.NetworkButtons.Set(PlayerInputButtons.Jump, Keyboard.current.spaceKey.isPressed);
        return data;
    }
}
