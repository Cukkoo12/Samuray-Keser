using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Dalga Ayarları")]
    public float waveDuration = 60f;
    public float spawnInterval = 1.5f;
    public float breakDuration = 5f;

    [Header("Düşman ve Spawn")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Header("UI")]
    public TextMeshProUGUI waveInfoText;

    private int currentWaveNumber = 0;
    private int enemiesAlive = 0;
    private float waveTimer = 0f;
    private bool waveInProgress = false;

    void OnEnable()
    {
        EnemyHealth.OnAnyEnemyDied += HandleEnemyDied;
    }

    void OnDisable()
    {
        EnemyHealth.OnAnyEnemyDied -= HandleEnemyDied;
    }

    void Start()
    {
        StartCoroutine(RunWavesForever());
    }

    IEnumerator RunWavesForever()
    {
        while (true)
        {
            currentWaveNumber++;
            int enemyCount = currentWaveNumber;

            yield return StartCoroutine(RunWave(enemyCount));
            yield return StartCoroutine(BreakRoutine());
        }
    }

    IEnumerator RunWave(int enemyCount)
    {
        waveInProgress = true;
        enemiesAlive = enemyCount;
        waveTimer = waveDuration;

        StartCoroutine(SpawnEnemies(enemyCount));

        while (waveTimer > 0 && enemiesAlive > 0)
        {
            waveTimer -= Time.deltaTime;
            UpdateUI(enemyCount);
            yield return null;
        }

        waveInProgress = false;
    }

    IEnumerator SpawnEnemies(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (!waveInProgress) yield break;

            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, point.position, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator BreakRoutine()
    {
        float breakTimer = breakDuration;
        int nextWaveNo = currentWaveNumber + 1;

        while (breakTimer > 0)
        {
            breakTimer -= Time.deltaTime;
            waveInfoText.text = string.Format("Dalga {0} Hazırlanıyor  |  Mola: {1:0.0}s", nextWaveNo, breakTimer);
            yield return null;
        }
    }

    void HandleEnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }

    void UpdateUI(int totalInWave)
    {
        int displayMin = Mathf.FloorToInt(waveTimer / 60f);
        int displaySec = Mathf.FloorToInt(waveTimer % 60f);

        waveInfoText.text = string.Format(
            "Dalga {0}  |  {1:00}:{2:00}  |  {3}/{4}",
            currentWaveNumber,
            displayMin, displaySec,
            enemiesAlive, totalInWave
        );
    }
}