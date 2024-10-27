using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RemoveBlockGamePlayRoom : MonoBehaviour
{
    [SerializeField] private Transform blockItemParent;
    
    public Camera mainCamera;

    private Vector3 downPos;

    private bool isDragging;

    private SingleBlockItem chooseBlockItem;

    private void Start()
    {
        LoadGame(5);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            chooseBlockItem = null;
            isDragging = false;
            downPos = Input.mousePosition;
            var ray = mainCamera.ScreenPointToRay(downPos);
            var rayCastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
            if (rayCastHit2D.transform != null)
            {
                rayCastHit2D.transform.parent.TryGetComponent(out chooseBlockItem);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (chooseBlockItem != null)
            {
                // 当前位置与按下位置距离大于5时视为拖拽游戏物体移动，否则视为点击
                if (Vector3.Distance(downPos, Input.mousePosition) > 5)
                {
                    // 开始移动
                    isDragging = true;
                    var tempPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    // 现在游戏物体只能在父物体范围内移动
                    tempPos.z = 0;
                    tempPos.x = Mathf.Clamp(tempPos.x, -1.73f, 1.73f);
                    tempPos.y = Mathf.Clamp(tempPos.y, -2.21f, 3.07f);
                    chooseBlockItem.SetPos(tempPos, false);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (chooseBlockItem != null && !isDragging)
            {
                ChooseBlockItem(chooseBlockItem);
            }
            
            chooseBlockItem = null;
            isDragging = false;
        }
    }

    private void ChooseBlockItem(SingleBlockItem singleBlockItem)
    {
        if(singleBlockItem == null) return;
        Debug.LogError("选择");
    }

    // 根据需要生成组的数量(一个游戏物体需要生成三个，因为消除时需要三个匹配才能消除)，随机生成游戏物体在场景中
    private void LoadGame(int loadGroupCount)
    {
        // var tempList = new List<SingleBlockItem>(loadGroupCount * 3);
        for (var i = 0; i < loadGroupCount; i++)
        {
            // 随机游戏物体的颜色和形状
            var randomColor = RemoveBlockGameConfig.Instance.ColorConfigs[Random.Range(0, RemoveBlockGameConfig.Instance.ColorConfigs.Count)];
            var randomShape = RemoveBlockGameConfig.Instance.BlockItems[Random.Range(0, RemoveBlockGameConfig.Instance.BlockItems.Count)];
            for (var j = 0; j < 3; j++)
            {
                var blockItem = Instantiate(randomShape, blockItemParent);
                // 随机游戏物体的位置,限制在父物体的范围内
                var randomPos = new Vector3(Random.Range(-1.73f, 1.73f), Random.Range(-2.21f, 3.07f), 0);
                blockItem.SetPos(randomPos, false);
                // 随机设置旋转和颜色
                blockItem.SetRotation(new Vector3(0, 0, Random.Range(0, 360f)));
                blockItem.SetColor(randomColor);
                // tempList.Add(blockItem);
            }
        }

        // 再随机设置生成物体的覆盖层级
        for (var i = 0; i < blockItemParent.childCount; i++)
        {
            blockItemParent.GetChild(i).transform.SetSiblingIndex(Random.Range(0, blockItemParent.childCount - 1));
        }
    }
}
