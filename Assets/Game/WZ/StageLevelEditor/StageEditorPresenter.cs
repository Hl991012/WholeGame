using System;
using UnityEditor;
using UnityEngine;

public class StageEditorPresenter : MonoBehaviour
{
    public enum TempEnum
    {
        Wall = 1,
        Stab = 2,
        Electricity = 3,
        Bow = 4,
        Ghost = 5,
        Poison = 6,
        Star = 7,
        Coin = 8,
        
        Spring1 = 10,
        Spring2 = 11,
        Spring3 = 12,
        Spring4 = 13,
        
        Door = 100,
        RevivePoint = 101,
    }
    
    public new Camera camera;
    public GameObject wallPrefab;
    public GameObject stabPrefab;
    public GameObject electricityPrefab;
    public GameObject bowPrefab;
    public GameObject ghostPrefab;
    public GameObject poisonPrefab;
    public GameObject starPrefab;
    public GameObject coinPrefab;
    
    public GameObject spring1Prefab;
    public GameObject spring2Prefab;
    public GameObject spring3Prefab;
    public GameObject spring4Prefab;

    public GameObject doorPrefab;
    public GameObject revivePointPrefab;


    private Transform wallParent;

    
    public  void Awake()
    {
        wallParent = GameObject.Find("PrefabParent").transform;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Create(TempEnum.RevivePoint);
            return;
        }
        
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            var tempWallParent = (new GameObject("PrefabParent")).transform;
            tempWallParent.position = Vector3.zero;
            tempWallParent.SetParent(wallParent.parent);
            for (var i = 0; i < wallParent.childCount; i++)
            {
                var tempEnum = (TempEnum)Enum.Parse(typeof(TempEnum), wallParent.GetChild(i).name);
                // if(tempEnum == TempEnum.Ghost) return;
                var tempWall = Create(tempEnum, tempWallParent);
                if (tempWall != null)
                {
                    tempWall.position = wallParent.GetChild(i).position;
                }
            }
            if (wallParent != null)
            {
                Destroy(wallParent.gameObject);
            }
            wallParent = tempWallParent;
            Debug.LogError("替换完成");
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Create(TempEnum.Door);
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Create(TempEnum.Spring1);
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Create(TempEnum.Spring2);
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Create(TempEnum.Spring3);
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Create(TempEnum.Spring4);
                return;
            }
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Create(TempEnum.Wall);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Create(TempEnum.Stab);
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Create(TempEnum.Electricity);
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
            Create(TempEnum.Bow);
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
            Create(TempEnum.Ghost);
        
        if (Input.GetKeyDown(KeyCode.Alpha6))
            Create(TempEnum.Poison);
        
        if (Input.GetKeyDown(KeyCode.Alpha7))
            Create(TempEnum.Star);
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
            Create(TempEnum.Coin);
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var temp = Physics2D.RaycastAll(camera.ScreenPointToRay(Input.mousePosition).origin,
                camera.ScreenPointToRay(Input.mousePosition).direction);
            if (temp != null && temp.Length > 0)
            {
                foreach (var item in temp)
                {
                    if (item.transform.name is "Wall" or "Stab" or "Electricity" or "Bow" or "Ghost" or "Poison"
                        or "Spring")
                    {
                        Destroy(item.transform.gameObject);
                        Debug.Log("删除 " + item.transform.name);
                    }

                    if (item.transform.name is "EditorBox2D")
                    {
                        Destroy(item.transform.parent.gameObject);
                        Debug.Log("删除 " + item.transform.parent.name);
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.D))
            camera.transform.Translate(Vector3.left * Time.deltaTime * 10);
        
        if (Input.GetKey(KeyCode.A))
            camera.transform.Translate(Vector3.right * Time.deltaTime * 10);
        
        if (Input.GetKey(KeyCode.S))
            camera.transform.Translate(Vector3.up * Time.deltaTime * 10);
        
        if (Input.GetKey(KeyCode.W))
            camera.transform.Translate(Vector3.down * Time.deltaTime * 10);
    }
    
    private Transform Create(TempEnum @enum, Transform parent = null)
    {
        var obj = @enum switch
        {
            TempEnum.Wall => wallPrefab,
            TempEnum.Stab => stabPrefab,    
            TempEnum.Electricity => electricityPrefab,
            TempEnum.Bow => bowPrefab,
            TempEnum.Ghost => ghostPrefab,
            TempEnum.Poison => poisonPrefab,
            TempEnum.Spring1 => spring1Prefab,
            TempEnum.Spring2 => spring2Prefab,
            TempEnum.Spring3 => spring3Prefab,
            TempEnum.Spring4 => spring4Prefab,
            TempEnum.Star => starPrefab,
            TempEnum.Coin => coinPrefab,
            TempEnum.Door => doorPrefab,
            TempEnum.RevivePoint => revivePointPrefab,
            _ => null,
        };

        if (obj != null)
        {
            var tempObj = Instantiate(obj, parent == null ? wallParent : parent);

            tempObj.name = @enum.ToString();
            
            var pos = camera.ScreenToWorldPoint(Input.mousePosition);
            tempObj.transform.position = new Vector3(Mathf.Round(pos.x * 2) / 2f, Mathf.Round(pos.y * 2) / 2f, 0);
            return tempObj.transform;
        }
        
        return  null;
    }
}
