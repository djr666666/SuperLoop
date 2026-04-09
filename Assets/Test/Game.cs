using SuperScrollView;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Game : MonoBehaviour
{
    public LoopGridView mLoopGridView; //无线列表对象
    private int creatNum = 200;
    [Header("定位测试")]public  int Positioning = 0; //定位

    public Button btn_SetNum;
    public Button btn_UpdateOne;
    public Button btn_Positioning;
    private bool isUpadateOne = false;
    private int testType = 0;


    public void Start()
    {
        mLoopGridView.InitGridView(creatNum, OnGetItemByRowColumn);  //初始化无限列表
        ButtonFun();
    }
    //********************** 用法 不需要对 滑动列表的content 改在任何脚本
    // 在 scrollview  挂在 LoopGridView
    // 在 子物体挂在  LoopGridViewItem






    //********************** 当我们不是无线列表的时候我们用字典存生成数据可以直接存子物体对象脚本去处理单个刷新但 无线列表不一样
    //无线列表复用 item 每次上下滑动会重新赋值 item 和他的脚本，如果你不存数据 
    //例 ： 你点击刷新 index 是 5 无线列表复用item 下滑到 20 原来的5被替换20 ，拉回到 5 从新走 show 导致你刷新出问题
    //所以需要第三方ItemData 数据类，区别于 无线列表  和 item ，item脚本根据index 处理 itemdata数据和方法
    //你需要缓存每个index的数据状态，而不仅仅是组件引用

    //最终偷懒方法，直接全刷，数据一改重新走，因为无线列表就是需要反复调用OnGetItemByRowColumn（）函数做复用
    //核心 ： 有限列表可以对原本子物体脚本做刷新，因为 （子物体不服用） 无线列表 必须需要数据层做刷新因为，数据层不会表，（子物体会复用）







    //例
    public class ItemData
    {
        public int Index;
        public bool IsUpdated;
        public int UpdateIndex;

        public ItemData(int index)
        {
            Index = index;
            IsUpdated = false;
        }

        public string GetDisplayText()
        {
            if (IsUpdated && Index == UpdateIndex)
            {
                return "刷新" + Index.ToString();
            }
            return "下标" + Index.ToString();
        }
    }

    private Dictionary<int, ItemData> _dic = new Dictionary<int, ItemData>();

    private LoopGridViewItem OnGetItemByRowColumn(LoopGridView view, int index, int row, int column)
    {
        if (index >= creatNum || index < 0) return null; //判断下标
        LoopGridViewItem item = view.NewListViewItem("Image"); //Image获得子物体名字生成
        Debug.Log($"无线列表生成 下标 {index}  row ={row}  column  {column}");
        Preb_Item com = item.GetComponent<Preb_Item>();

        if (!_dic.TryGetValue(index, out ItemData data))
        {
            data = new ItemData(index);
            _dic.Add(index, data);
        }
        com.Show(data);

        return item;
    }



    // 定位
    public void LoopScrollTo(LoopGridView loopscroll, int itemIndex)
    {
        itemIndex = Positioning;
        loopscroll.MovePanelToItemByIndex(itemIndex, 0);
    }

    // 刷新一个
    public void UpdateLoopScrllByIndex(LoopGridView loopscroll, int itemIndex)
    {
        // 更新数据
        if (_dic.TryGetValue(itemIndex, out ItemData data))
        {
            data.IsUpdated = true;
            data.UpdateIndex = itemIndex;
        }


        loopscroll.RefreshItemByItemIndex(itemIndex);
    }

    // 刷新全部
    public void UpdateLoopScrllAllItem(LoopGridView loopscroll)
    {
        loopscroll.RefreshAllShownItem(); //刷新会重新走  OnGetItemByRowColumn 这个函数
    }


    // 此方法可用于在运行时设置GridView的项目总数。该参数的取值必须>=0,ItemIndex取值范围为0 ~itemCount -1。如果resetPos设置为false，则在此方法完成后，ScrollRect的内容位置不会改变。
    public void LoopScrollSetCount(LoopGridView loopscroll, int count)
    {
        //比如我原来有道具200 现在变成88 应该去掉数据112 而不是直接clear 
        // OnGetItemByRowColumn 不要被这个重新new 迷惑数据只有一份，如果后端告诉你少了什么 道具直接删，根据实际情况是覆盖，还是 单独处理
        //if (!_dic.TryGetValue(index, out ItemData data))
        //{
        //    data = new ItemData(index);
        //    _dic.Add(index, data);
        //}
        loopscroll.SetListItemCount(count);
    }

    public void LoopScrollUpdate(LoopGridView loopscroll)
    {
        loopscroll.RefreshAllShownItem();
    }

    // 选中全部
    public void OnSelectAllBtnClicked(LoopGridView loopscroll)
    {
        loopscroll.RefreshAllShownItem();
    }

    public void ButtonFun()
    {
        btn_SetNum.onClick.AddListener(() => {
            LoopScrollSetCount(mLoopGridView, 88);
            UpdateLoopScrllAllItem(mLoopGridView);
        });

        btn_UpdateOne.onClick.AddListener(() => {

            isUpadateOne = true;
            UpdateLoopScrllByIndex(mLoopGridView, 5);
        });

        btn_Positioning.onClick.AddListener(() => {

            LoopScrollTo(mLoopGridView, Positioning); //定位
        });

    }
}
