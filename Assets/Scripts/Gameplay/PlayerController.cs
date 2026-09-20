using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 1000f;

    [Networked] private NetworkButtons buttonsPrev { get; set; }
    private float horizontal;
    private Rigidbody2D rigidbody;

    private enum PlayerInputButtons
    {
        None,
        Jump
    }

    public override void Spawned()
    {
        rigidbody = GetComponent<Rigidbody2D>();
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
