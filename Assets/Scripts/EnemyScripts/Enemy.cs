using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float navigation;
    [SerializeField] float health;
    [SerializeField] int rewardAmount;
    SpriteRenderer sprite;
    Transform exit;
    Transform[] wayPoints;
    GameObject parentWayPoints;
    Transform enemy;
    Collider2D enemyCollider;
    Animator anim;
    float navigationTime = 0;
    int target = 0;
    bool isDead = false;

    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }
    
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
        parentWayPoints = GameObject.FindWithTag("WayPoints");
        wayPoints = new Transform[parentWayPoints.transform.childCount];
        Debug.Log(parentWayPoints.transform.childCount);

        for(int i = 0; i < wayPoints.Length; i++)
        {
            wayPoints[i] = parentWayPoints.transform.GetChild(i);
        }

        enemy = GetComponent<Transform>();
        exit = GameObject.FindWithTag("Finish").transform;
        Manager.Instance.RegisterEnemy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(wayPoints != null && !isDead)
        {
            navigationTime += Time.deltaTime;

            if(navigationTime > navigation)
            {
                if(target < wayPoints.Length)
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, wayPoints[target].position, navigationTime);
                }
                else
                {
                    enemy.position = Vector2.MoveTowards(enemy.position, exit.position, navigationTime);
                }

                navigationTime = 0;
            }
        }
    }

    public void EnemyHit(float hitPoints)
    {
        if(health > hitPoints)
        {
            health -= hitPoints;
            Manager.Instance.Source.PlayOneShot(SoundManager.Instance.Hit, 0.1f);
            anim.Play("HurtEnemy1");
        }
        else
        {
            anim.SetTrigger("didDie");
            Die();
        }
    }

    public void Die()
    {
        if(!isDead)
        {
            Manager.Instance.Source.PlayOneShot(SoundManager.Instance.Death, 0.1f);
            Manager.Instance.TotalKilled++;
            Manager.Instance.AddMoney(rewardAmount);
            Manager.Instance.IsWaveOver();
        }
        isDead = true;
        enemyCollider.enabled = false;
        sprite.sortingOrder = 2;
        Invoke("DeleteThis", 5f);
    }

    void DeleteThis()
    {
        Manager.Instance.UnregisterEnemy(this);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "MovingPoint")
        {
            target++;
        }
        else if(other.gameObject.tag == "Finish")
        {
            Manager.Instance.RoundEscaped++;
            Manager.Instance.TotalEscaped++;
            Manager.Instance.UnregisterEnemy(this);
            Manager.Instance.IsWaveOver();
        }
        else if(other.gameObject.tag.Equals("Projectiles"))
        {
            Projectiles newP = other.gameObject.GetComponent<Projectiles>();
            Destroy(other.gameObject);
            EnemyHit(newP.AttackDamage);
        }
    }
}
