using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class TowerManager : Loader<TowerManager>
{
    TowerButton towerBtnPressed;
    SpriteRenderer spriteRenderer;
    [SerializeField] GameObject radiusImage;
    TowerControl towerControl;
    [SerializeField] GameObject setting;
    Text addDestroy;
    Text priceUpdate;
    List<TowerControl> towerList = new List<TowerControl>();
    List<Collider2D> buildList = new List<Collider2D>();
    Collider2D buildTile;
    GameObject towerBox;

    bool isTowerRadius;

    public TowerButton TowerBtnPresse
    {
        get
        {
            return towerBtnPressed;
        }
        set
        {
            towerBtnPressed = value;
        }
    }
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
        towerBox = GameObject.Find("Towers");
        spriteRenderer.enabled = false;
        isTowerRadius = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && towerBtnPressed != null && Manager.Instance.TotalMoneyThisLevel >= towerBtnPressed.TowerPrice)
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePoint, Vector2.zero);

            if(hit.collider.tag.Equals("TowerSide"))
            {
                buildTile = hit.collider;
                buildTile.tag = "TowerSideFull";
                RegisterBuildSite(buildTile);
                PlaceTower(hit);
                Debug.Log("Tower Plant");
            }
            else if(hit.collider.tag.Equals("Ground"))
            {
                Debug.Log("Grounnd");
            }
        }

        if(Input.GetMouseButtonDown(1) && towerBtnPressed == null)
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePoint, Vector2.zero);

            if(hit.collider.tag.Equals("Tower"))
            {
                if(Manager.Instance.Setting == null)
                {
                    Manager.Instance.Hit2D = hit;
                    Manager.Instance.Setting = Instantiate(setting, GameObject.FindGameObjectWithTag("Canvas").transform);

                    GameObject settingPos = Manager.Instance.Setting;
                    towerControl = hit.collider.GetComponent<TowerControl>();

                    priceUpdate = settingPos.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
                    priceUpdate.text = (towerControl.TowerPrice * 2).ToString();
                    addDestroy = settingPos.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
                    addDestroy.text = (towerControl.TowerPrice / 2).ToString();
                    settingPos.transform.position = towerControl.transform.position;

                    if(GameObject.Find("Circle(Clone)") == null)
                    {
                        Instantiate(towerControl.AttackRadiosSprite, towerControl.transform.position, transform.rotation);

                        GameObject radius = GameObject.Find("Circle(Clone)");
                        radius.transform.localScale = new Vector3(towerControl.AttackRadius * 2, towerControl.AttackRadius * 2, 1f);
                    }
                }
            }
        }

        if(spriteRenderer.enabled)
        {
            FollowMouse();
        }

        if(GameObject.Find("Circle(Clone)") != null && isTowerRadius)
        {
            GameObject radius = GameObject.Find("Circle(Clone)");
            radius.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            radius.transform.position = new Vector2(radius.transform.position.x, radius.transform.position.y);
        }
    }

    public void DestroySetting()
    {
        Destroy(Manager.Instance.Setting.gameObject);
        Manager.Instance.Setting = null;

        if(GameObject.Find("Circle(Clone)") != null)
        {
            Destroy(GameObject.Find("Circle(Clone)").gameObject);
        }
    }

    public void UpdateTower()
    {
        DestroySetting();
    }

    public void RegisterBuildSite(Collider2D buildTag)
    {
        buildList.Add(buildTag);
    }

    public void RegisterTower(TowerControl tower)
    {
        towerList.Add(tower);
        Debug.Log("register tower");
    }

    void UnregisterTower(TowerControl tower)
    {
        towerList.Remove(tower);
    }

    public void RenameTagBuildSite()
    {
        foreach(Collider2D buildTag in buildList)
        {
            buildTag.tag = "TowerSide";
        }

        buildList.Clear();
    }

    public void DestroyAllTowers()
    {
        foreach(TowerControl tower in towerList)
        {
            if(tower == null)
            {
                continue;
            }

            Destroy(tower.gameObject);
        }

        towerList.Clear();
    }

    public void PlaceTower(RaycastHit2D hit)
    {
        if(!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null)
        {
            TowerControl newTower = Instantiate(towerBtnPressed.TowerObject, towerBox.transform);
            newTower.transform.position = hit.transform.position;
            Manager.Instance.Source.PlayOneShot(SoundManager.Instance.TowerBuild, 0.05f);
            BuyTower(towerBtnPressed.TowerPrice);
            RegisterTower(newTower);
            DisableDrag();
            Debug.Log("Method tower place");
        }
    }

    public void BuyTower(int price)
    {
        Manager.Instance.SubtractMoney(price);
    }

    public void SelectedTower(TowerButton towerSelected)
    {
        if(towerSelected.TowerPrice <= Manager.Instance.TotalMoneyThisLevel)
        {
            towerBtnPressed = towerSelected;
            EnableDrag(towerBtnPressed.DragSprite);
            Debug.Log("Select Tower");
        }
    }

    public void ShowAttackRadius(TowerControl tower)
    {
        if(GameObject.Find("Circle(Clone)") != null)
        {
            Destroy(GameObject.Find("Circle(Clone)").gameObject);
        }

        if(GameObject.Find("Circle(Clone)") == null && tower.TowerPrice <= Manager.Instance.TotalMoneyThisLevel)
        {
            Instantiate(tower.AttackRadiosSprite, Camera.main.ScreenToWorldPoint(Input.mousePosition), transform.rotation);

            GameObject radius = GameObject.Find("Circle(Clone)");
            radius.transform.localScale = new Vector3(tower.AttackRadius * 2, tower.AttackRadius * 2, 1f);
            isTowerRadius = true;
        }
    }

    public void DeleteTower()
    {
        RaycastHit2D hit = Manager.Instance.Hit2D;

        if(hit.collider.tag.Equals("Tower"))
        {
            Debug.Log("Destroy Tower");

            if(towerControl == null)
            {
                towerControl = hit.collider.GetComponent<TowerControl>();
            }
            
            towerControl.TowerSide.tag = "TowerSide";
            Manager.Instance.AddMoney(towerControl.TowerPrice / 2);
            Destroy(hit.collider.gameObject);
            DestroySetting();
            UnregisterTower(towerControl);
            towerControl = null;
        }
    }

    public void FollowMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    public void EnableDrag(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
    }

    public void DisableDrag()
    {
        spriteRenderer.enabled = false;
        towerBtnPressed = null;

        if(GameObject.Find("Circle(Clone)") != null)
        {
            Destroy(GameObject.Find("Circle(Clone)").gameObject);
            isTowerRadius = false;
        }
    }
}
