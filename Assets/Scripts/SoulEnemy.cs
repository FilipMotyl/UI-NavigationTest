using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulEnemy : MonoBehaviour, Enemy
{
    [SerializeField] private GameObject interactionPanelObject;
    [SerializeField] private GameObject actionsPanelObject;
    [SerializeField] private SpriteRenderer enemySpriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Button interactionButton;
    [SerializeField] private Button bowButton;
    private SpawnPoint EnemyPosition;

    public void SetupEnemy(Sprite sprite, SpawnPoint spawnPoint)
    {
        enemySpriteRenderer.sprite = sprite;
        EnemyPosition = spawnPoint;
        gameObject.SetActive(true);
    }

    public void DeactivateCombat()
    {
        if (actionsPanelObject.activeInHierarchy)
        {
            ActiveInteractionPanel(true);
            ActiveActionPanel(false);
            GUIController.Instance.SetCurrentSelectedButton(interactionButton);
        }
    }

    public SpawnPoint GetEnemyPosition()
    {
        return EnemyPosition;
    }

    public Animator GetEnemyAnimator()
    {
        return animator;
    }

    public Button GetInteractionButton()
    {
        return interactionButton;
    }

    public GameObject GetEnemyObject()
    {
        return this.gameObject;
    }

    private void ActiveCombatWithEnemy()
    {
        ActiveInteractionPanel(false);
        ActiveActionPanel(true);
    }

    private void ActiveInteractionPanel(bool active)
    {
        interactionPanelObject.SetActive(active);
        GUIController.Instance.IsCombatStateActive = !active;
    }

    private void ActiveActionPanel(bool active)
    {
        actionsPanelObject.SetActive(active);
        GUIController.Instance.SetCurrentSelectedButton(bowButton);
        GUIController.Instance.IsCombatStateActive = active;
    }

    private void UseBow()
    {
        // USE BOW
        GameEvents.EnemyKilled?.Invoke(this);
    }

    private void UseSword()
    {
        GameEvents.EnemyKilled?.Invoke(this);
        // USE SWORD
    }

    #region OnClicks
    public void Combat_OnClick()
    {
        ActiveCombatWithEnemy();
    }

    public void Bow_OnClick()
    {
        UseBow();
    }

    public void Sword_OnClick()
    {
        UseSword();
    }
    #endregion
}

public interface Enemy
{
    SpawnPoint GetEnemyPosition();
    GameObject GetEnemyObject();
}
