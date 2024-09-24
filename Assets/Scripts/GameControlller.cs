using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameLocalization
{
    SWAMPS,
    DUNGEON,
    CASTLE,
    CITY,
    TOWER
}

public class GameControlller : MonoBehaviour
{
    #region Singleton
    private static GameControlller instance;

    public static GameControlller Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameControlller>();
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [SerializeField] private GameLocalization currentGameLocalization;
    [SerializeField] private EnemiesController enemiesController;

    public GameLocalization CurrentGameLocalization
    {
        get
        {
            return currentGameLocalization;
        }

        set
        {
            currentGameLocalization = value;
        }
    }

    private bool isPaused;

    public bool IsPaused
    {

        get
        {
            return isPaused;
        }
        set
        {
            isPaused = value;
            if (isPaused)
            {
                SetPausedState(true);
            }
            else
            {
                SetPausedState(false);
            }
        }
    }

    public bool IsCurrentLocalization(GameLocalization localiztion)
    {
        return CurrentGameLocalization == localiztion;
    }
 

    private void SetPausedState(bool paused)
    {
        if (paused)
        {
            enemiesController.PauseEnemyAnimations();
        }
        else
        {
            enemiesController.ResumeEnemyAnimations();
        }
    }
}