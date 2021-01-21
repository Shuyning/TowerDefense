using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum gameStatus
{
    next, play, gameover, win
}

public class Manager : Loader<Manager>
{
    GameObject spawnPoint;
    GameObject[] enemies;
    GameObject boxEnemies;
    GameObject enemiesThisLevel;
    Text totalMoneyLabel;
    Text currentWaveLabel;
    Text playBtnLabel;
    Text totalEscapedLabel;
    Text enemiesKilledThisWaveLabel;
    AudioSource source;

    [SerializeField] int totalEnemies;
    [SerializeField] int enemiesPerSpawn;
    [SerializeField] int totalWave;
    [SerializeField] int limitEscadep;
    [SerializeField] string nameNumberSpawn;
    [SerializeField] Button playBtn;
    [SerializeField] int totalMoneyThisLevel;

    public List<Enemy> enemyList = new List<Enemy>();
    int refreshTotalEnemies;
    int waveNumber = 0;
    int totalMoneySave = 160;
    int totalEscaped = 0;
    int roundEscaped = 0;
    int whichEnemiesToSpawn = 1;
    int totalKilled;
    int enemiesToSpawn = 0;
    const float spawnDelay = 0.6f;
    gameStatus currentStatus = gameStatus.play;

    RaycastHit2D hit2D;

    GameObject setting;

    public RaycastHit2D Hit2D
    {
        get
        {
            return hit2D;
        }
        set
        {
            hit2D = value;
        }
    }

    public GameObject Setting
    {
        get
        {
            return setting;
        }
        set
        {
            setting = value;
        }
    }

    public int TotalEscaped
    {
        get
        {
            return totalEscaped;
        }
        set
        {
            totalEscaped = value;
        }
    }

    public int RoundEscaped
    {
        get
        {
            return roundEscaped;
        }
        set
        {
            roundEscaped = value;
        }
    }

    public int TotalKilled
    {
        get
        {
            return totalKilled;
        }
        set
        {
            totalKilled = value;
        }
    }

    public int TotalMoneyThisLevel
    {
        get
        {
            return totalMoneyThisLevel;
        }
        set
        {
            totalMoneyThisLevel = value;
            totalMoneyLabel.text = totalMoneyThisLevel.ToString();
        }
    }

    public int RefreshTotalEnemies
    {
        get
        {
            return refreshTotalEnemies;
        }
    }

    public AudioSource Source
    {
        get
        {
            return source;
        }
    }

    void Start()
    {
        totalMoneyLabel = GameObject.FindGameObjectWithTag("MoneyText").GetComponent<Text>();
        currentWaveLabel = GameObject.FindGameObjectWithTag("CurrentWaveText").GetComponent<Text>();
        playBtnLabel = GameObject.FindGameObjectWithTag("WinText").GetComponent<Text>();
        totalEscapedLabel = GameObject.FindGameObjectWithTag("EscapedText").GetComponent<Text>();
        enemiesKilledThisWaveLabel = GameObject.FindGameObjectWithTag("EnemyKillText").GetComponent<Text>();
        source = GetComponent<AudioSource>();

        playBtn.gameObject.SetActive(false);

        enemiesThisLevel = GameObject.FindWithTag("EnemiesThisLevel");
        enemies = new GameObject[enemiesThisLevel.transform.childCount];
        Debug.Log(enemiesThisLevel.transform.childCount);

        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = enemiesThisLevel.transform.GetChild(i).gameObject;
        }

        boxEnemies = GameObject.FindWithTag(nameNumberSpawn);
        spawnPoint = GameObject.FindWithTag("Respawn");
        refreshTotalEnemies = totalEnemies;
        totalMoneySave = TotalMoneyThisLevel;

        currentWaveLabel.text = "Wave " + 1;
        totalMoneyLabel.text = TotalMoneyThisLevel.ToString();

        SetCurrentGameState();
        ShowMenu();
    }

    void Update() 
    {
        HandleEscape();
    }

    IEnumerator Spawn()
    {
        if(enemiesPerSpawn > 0 && enemyList.Count < totalEnemies)
        {
            for(int i = 0; i < enemiesPerSpawn; i++)
            {
                if(enemyList.Count < totalEnemies)
                {
                    GameObject newEnemy = Instantiate(enemies[Random.Range(0, enemiesToSpawn)], boxEnemies.transform) as GameObject;
                    newEnemy.transform.position = spawnPoint.transform.position;
                    newEnemy.SetActive(true);
                }
            }

            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(Spawn());
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        enemyList.Add(enemy);
    }
    

    public void UnregisterEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
        //totalEnemies--;
        Destroy(enemy.gameObject);
    }

    public void DestroyEnemies()
    {
        foreach(Enemy enemy in enemyList)
        {
            Destroy(enemy.gameObject);
        }

        enemyList.Clear();
    }

    public void AddMoney(int amount)
    {
        TotalMoneyThisLevel += amount;
    }

    public void SubtractMoney(int amount)
    {
        TotalMoneyThisLevel -= amount;
    }

    public void SetCurrentGameState()
    {
        if(totalEscaped >= limitEscadep)
        {
            currentStatus = gameStatus.gameover;
        }
        else if(waveNumber == 0 && (RoundEscaped + TotalKilled) == 0)
        {
            currentStatus = gameStatus.play;
        }
        else if(waveNumber >= totalWave)
        {
            currentStatus = gameStatus.win;
        }
        else
        {
            currentStatus = gameStatus.next;
        }
    }

    public  void IsWaveOver()
    {
        totalEscapedLabel.text = "Escaped " + totalEscaped + "/" + limitEscadep;
        enemiesKilledThisWaveLabel.text = totalKilled + " / " + totalEnemies;

        if((RoundEscaped + TotalKilled) == totalEnemies)
        {
            SetCurrentGameState();
            ShowMenu();
        }
    }

    public void PlayButtonPress()
    {
        switch(currentStatus)
        {
            case gameStatus.next:
                waveNumber++;
                totalEnemies += waveNumber;

                enemiesKilledThisWaveLabel.text = "0" + " / " + totalEnemies;
                DeleteProjectiles();
                if(enemiesToSpawn < enemies.Length)
                {
                    enemiesToSpawn++;
                }
                break;
            case gameStatus.play:
                TotalEscaped = 0;
                totalMoneyLabel.text = TotalMoneyThisLevel.ToString();
                totalEscapedLabel.text = "Escaped " + totalEscaped + "/" + limitEscadep;
                enemiesKilledThisWaveLabel.text = 0 + " / " + totalEnemies;
                DeleteProjectiles();
                break;
            default:
                totalEnemies = 15;
                TotalEscaped = 0;
                limitEscadep = 10;
                totalWave = 0;
                enemiesToSpawn = 0;
                TotalMoneyThisLevel = totalMoneySave;
                totalMoneyLabel.text = TotalMoneyThisLevel.ToString();
                totalEscapedLabel.text = "Escaped " + totalEscaped + "/" + limitEscadep;
                enemiesKilledThisWaveLabel.text = 0 + " / " + totalEnemies;
                source.PlayOneShot(SoundManager.Instance.NewGame, 0.05f);
                DeleteProjectiles();
                
                TowerManager.Instance.DestroyAllTowers();
                TowerManager.Instance.RenameTagBuildSite();
                break;
        }

        DestroyEnemies();
        TotalKilled = 0;
        RoundEscaped = 0;
        int currenWave = waveNumber;
        currentWaveLabel.text = "Wave " + (currenWave + 1);

        StartCoroutine(Spawn());
        playBtn.gameObject.SetActive(false);
    }

    public void DeleteProjectiles()
    {
        GameObject[] POnScene = GameObject.FindGameObjectsWithTag("Projectiles");

        foreach(GameObject P in POnScene)
        {
            Destroy(P);
        }
    }

    public void ShowMenu()
    {
        switch(currentStatus)
        {
            case gameStatus.gameover:
                playBtnLabel.text = "Play Again!";
                Source.PlayOneShot(SoundManager.Instance.GameOver, 0.05f);
                break;
            case gameStatus.next:
                playBtnLabel.text = "Next wave";
                break;
            case gameStatus.play:
                playBtnLabel.text = "Play game";
                break;
            case gameStatus.win:
                playBtnLabel.text = "Continue";
                break;
        }

        playBtn.gameObject.SetActive(true);
    }

    void HandleEscape()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            TowerManager.Instance.DisableDrag();
        }
    }
}
