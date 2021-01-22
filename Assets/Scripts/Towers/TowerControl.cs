using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControl : MonoBehaviour
{
    [SerializeField] float timeBtwAttacks;
    [SerializeField] float attackRadius;
    [SerializeField] float speedProjectile;
    [SerializeField] Projectiles projectile;
    [SerializeField] int towerPrice;
    [SerializeField] GameObject attackRadiosSprite;

    GameObject towerSetting;
    Enemy targetEnemy = null;
    GameObject towerSide;
    GameObject projectileBox;
    List<Projectiles> projectileList = new List<Projectiles>();
    float attackCounter;
    bool isAttacking = false;

    public GameObject AttackRadiosSprite
    {
        get 
        {
            return attackRadiosSprite;
        }
    }

    public  GameObject TowerSide
    {
        get
        {
            return towerSide;
        }
    }

    public  GameObject TowerSetting
    {
        get
        {
            return towerSetting;
        }
    }

    public  int TowerPrice
    {
        get
        {
            return towerPrice;
        }
    }

    public float AttackRadius
    {
        get
        {
            return attackRadius;
        }
    }
    
    void Start()
    {
        projectileBox = GameObject.Find("ProjectileBox");
    }

    // Update is called once per frame
    void Update()
    {
        attackCounter -= Time.deltaTime;

        if(targetEnemy == null || targetEnemy.IsDead)
        {
            Enemy nearestEnemy = GetNearestEnemy();

            if(nearestEnemy != null && Vector2.Distance(transform.localPosition, nearestEnemy.transform.localPosition) <= attackRadius)
            {
                targetEnemy = nearestEnemy;
            }
        }
        else
        {
            if(attackCounter <= 0)
            {
                isAttacking = true;
                attackCounter = timeBtwAttacks;
            }
            else
            {
                isAttacking = false;
            }

            if(Vector2.Distance(transform.localPosition, targetEnemy.transform.localPosition) > attackRadius)
            {
                targetEnemy = null;
            }
        }  

        if(isAttacking == true)
        {
            Attack();
        }

        if(towerSide == null)
        {
            CheckSide();
        }
    }

    public void Attack()
    {
        isAttacking = false;
        Projectiles newProjectile = Instantiate(projectile, projectileBox.transform) as Projectiles;
        newProjectile.transform.localPosition = transform.localPosition;

        if(newProjectile.PType == projectileType.arrow)
        {
            Manager.Instance.Source.PlayOneShot(SoundManager.Instance.Arrow, 0.05f);
        }
        else if(newProjectile.PType == projectileType.rock)
        {
            Manager.Instance.Source.PlayOneShot(SoundManager.Instance.Rock, 0.05f);
        }
        else if(newProjectile.PType == projectileType.fireball)
        {
            Manager.Instance.Source.PlayOneShot(SoundManager.Instance.Fireball, 0.05f);
        }

        if(targetEnemy == null)
        {
            Destroy(newProjectile);
        }
        else
        {
            //move projectaile to enemy
            StartCoroutine(MoveProjectaile(newProjectile));
        }
    }

    void DeleteProjectileThisTower()
    {
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }
    }

    IEnumerator MoveProjectaile(Projectiles projectiles)
    {
        while(GetTargetDistanse(targetEnemy) > 0.2f && projectiles != null && targetEnemy != null)
        {
            var dir = targetEnemy.transform.localPosition - transform.localPosition;
            var angleDirection = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            projectiles.transform.rotation = Quaternion.AngleAxis(angleDirection, Vector3.forward);
            projectiles.transform.localPosition = Vector2.MoveTowards(projectiles.transform.localPosition, targetEnemy.transform.localPosition, speedProjectile * Time.deltaTime);
            yield return null;
        }

        if(projectiles != null || targetEnemy == null)
        {
            Destroy(projectiles.gameObject);
        }
    }

    float GetTargetDistanse(Enemy thisEnemy)
    {
        if(thisEnemy == null)
        {
            thisEnemy = GetNearestEnemy();

            if(thisEnemy == null)
            {
                return 0f;
            }
        }

        return Mathf.Abs(Vector2.Distance(transform.localPosition, thisEnemy.transform.localPosition));
    }

    private List<Enemy> GetEnemiesInRange()
    {
        List<Enemy> enemiesInRange = new List<Enemy>();

        foreach(Enemy enemy in Manager.Instance.enemyList)
        {
            if(!enemy.IsDead)
                {
                if(Vector2.Distance(transform.localPosition, enemy.transform.localPosition) <= attackRadius)
                    {
                        enemiesInRange.Add(enemy);
                    }
                }
        }

        return enemiesInRange;
    }

    Enemy GetNearestEnemy()
    {
        Enemy nearestEnemy = null;
        float smallestDistanse = float.PositiveInfinity;

        foreach(Enemy enemy in GetEnemiesInRange())
        {
            if(Vector2.Distance(transform.localPosition, enemy.transform.localPosition) < smallestDistanse)
            {                
                smallestDistanse = Vector2.Distance(transform.localPosition, enemy.transform.localPosition);
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    void CheckSide()
    {
        Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Physics2D.queriesHitTriggers = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        

        if(hit.collider.tag.Equals("TowerSideFull"))
        {
            Debug.Log("TowerSide find");
            towerSide = hit.collider.gameObject;
        }
        else
        {
            Debug.Log(hit.collider.gameObject);
        }

        Physics2D.queriesHitTriggers = true; 
    }
}
