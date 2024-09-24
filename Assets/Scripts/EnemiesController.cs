using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesController : MonoBehaviour
{
    [SerializeField] private List<Sprite> allEnemies;
    [SerializeField] private List<SpawnPoint> spawnPoints;
    [SerializeField] private GameObject enemyPrefab;
    private List<SoulEnemy> activeEnemies = new List<SoulEnemy>();

    private int maxEnemies = 3;
    private int currentEnemies = 0;

    private void Awake()
    {
        ConfigureEnemiesController();
        SpawnEnemies();
    }

    private void OnEnable()
    {
        AttachListeners();
    }

    private void OnDisable()
    {
        DettachListeners();
    }

    public void ExitCombatState()
    {
        foreach(SoulEnemy enemy in activeEnemies)
        {
            enemy.DeactivateCombat();
        }
    }

    public void PauseEnemyAnimations()
    {
        foreach(SoulEnemy enemy in activeEnemies)
        {
            enemy.GetEnemyAnimator().speed = 0;
        }
    }

    public void ResumeEnemyAnimations()
    {
        foreach (SoulEnemy enemy in activeEnemies)
        {
            enemy.GetEnemyAnimator().speed = 1;
        }
    }

    public Selectable GetFirstAvailiableSelectableEnemyUI()
    {
        float position = Mathf.Infinity;
        Selectable selectable = GUIController.Instance.GetMenuButton();
        foreach (SoulEnemy enemy in activeEnemies)
        {
            if (enemy.transform.position.x < position)
            {
                position = enemy.GetEnemyPosition().Position.position.x;
                selectable = enemy.GetInteractionButton();
            }
        }
        return selectable;
    }

    public Button TryFindButtonToTheRight(float currentButtonXPos)
    {
        return activeEnemies.Select(soul => soul
        .GetInteractionButton())
        .Where(button => button.transform.position.x > currentButtonXPos)
        .OrderBy(button => button.transform.position.x)
        .FirstOrDefault(); ;
    }

    public Button TryFindButtonToTheLeft(float currentButtonXPos)
    {
        return activeEnemies.Select(soul => soul
        .GetInteractionButton())
        .Where(button => button.transform.position.x < currentButtonXPos)
        .OrderByDescending(button => button.transform.position.x)
        .FirstOrDefault();
    }

    private void AttachListeners()
    {
        GameEvents.EnemyKilled += EnemyKilled;
    }

    private void DettachListeners()
    {
        GameEvents.EnemyKilled -= EnemyKilled;
    }

    private void EnemyKilled(Enemy enemy)
    {
        FreeSpawnPoint(enemy.GetEnemyPosition());
        DestroyKilledEnemy(enemy.GetEnemyObject());
        StartCoroutine(SpawnEnemyViaCor());
        GUIController.Instance.SetCurrentSelectedButton(null);
    }

    private void SpawnEnemies()
    {
        while (currentEnemies < maxEnemies)
        {
            SpawnEnemy();
        }
    }

    private IEnumerator SpawnEnemyViaCor()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if(currentEnemies >= maxEnemies)
        {
            Debug.LogError("Max Enemies reached! Kil some to spawn new");
            return;
        }

        int FreeSpawnPointIndex = -1;
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (!spawnPoints[i].IsOccupied)
            {
                FreeSpawnPointIndex = i;
                break;
            }
        }

        if (FreeSpawnPointIndex != -1)
        {
            spawnPoints[FreeSpawnPointIndex].IsOccupied = true;
            SoulEnemy Enemy = Instantiate(enemyPrefab, spawnPoints[FreeSpawnPointIndex].Position.position, Quaternion.identity, transform).GetComponent<SoulEnemy>();
            int SpriteIndex = Random.Range(0, allEnemies.Count);
            Enemy.SetupEnemy(allEnemies[SpriteIndex], spawnPoints[FreeSpawnPointIndex]);
            activeEnemies.Add(Enemy);
            currentEnemies++;
        }
    }

    private void DestroyKilledEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy.GetComponent<SoulEnemy>());
        Destroy(enemy);
    }
    private void FreeSpawnPoint(SpawnPoint spawnPoint)
    {
        for(int i =0;i<spawnPoints.Count;i++)
        {
            if(spawnPoint == spawnPoints[i])
            {
                spawnPoints[i].IsOccupied = false;
                currentEnemies--;
                break;
            }
        }
    }

    private void ConfigureEnemiesController()
    {
        if (spawnPoints != null)
        {
            maxEnemies = spawnPoints.Count;
        }
        else
        {
            maxEnemies = 3;
        }
    }
}

[System.Serializable]
public class SpawnPoint
{
    public Transform Position;
    public bool IsOccupied;
}
