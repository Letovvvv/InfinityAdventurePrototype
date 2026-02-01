using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Material deathMaterial;
    [SerializeField] private GameObject playerDeathVFXPrefab;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private FlashBlink _flashBlink;

    private const string IS_RUNNING = "IsRunning";
    private const string IS_DEATH = "IsDeath";
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _flashBlink = GetComponent<FlashBlink>();
    }

    private void Start()
    {
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
    }

    private void Player_OnPlayerDeath(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_DEATH, true);
        _flashBlink.StopBlinking();
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning()); // Устанавливает анимацию бега или idle

        spriteRenderer.flipX = Player.Instance.IsTurnLeft(); // Поворачивает игрока
    }

    private void ShowDeathVFX()
    {
        Instantiate(playerDeathVFXPrefab, _player.transform.position, Quaternion.identity);
    }

    private void SetDeathMaterial()
    {
        spriteRenderer.material = deathMaterial;
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
    }
}
