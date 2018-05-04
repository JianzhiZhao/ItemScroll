using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ItemScroll : MonoBehaviour
{ 
    #region 卷軸物件
    public ScrollRect scrollRect;
    public GameObject ItemsParent;
    public GameObject SlotsParent;
    List<GameObject> Items = new List<GameObject>();
    List<GameObject> Slots = new List<GameObject>();
    #endregion

    #region 卷軸變數
    public int FocusIndex;
    float ScrollOffset;
    float NPosition;
    float midScrollPosition;
    int dataCount;
    #endregion

    #region 私有共用變數
    float slotIndexf;
    int slotIndexL;
    int slotIndexR;
    int dataIndex;
    float t;
    int tmp_index;
    RectTransform itemRT;
    RectTransform slotLRT;
    RectTransform slotRRT;
    #endregion

    #region 事件
    public Action<GameObject, int> OnSetItemData;
    #endregion

    public bool Loop = false;

    public void Initial(int _dataCount, int _focusIndex,Action<GameObject,int> onSetItemData)
    {
        Items.Clear();
        foreach (Transform item in ItemsParent.transform)
            Items.Add(item.gameObject);

        Slots.Clear();
        foreach (Transform slot in SlotsParent.transform)
            Slots.Add(slot.gameObject);

        dataCount = _dataCount;
        FocusIndex = _focusIndex;

        ScrollOffset = dataCount == 1 ? 0 : 1f / (dataCount - 1);
        midScrollPosition = Mathf.CeilToInt(Items.Count/2);

        OnSetItemData = onSetItemData;

        scrollRect.onValueChanged.RemoveListener(OnScrolling);
        scrollRect.onValueChanged.AddListener(OnScrolling);

        NPosition = GetNPByIndex(FocusIndex);
        scrollRect.horizontalNormalizedPosition = NPosition;
        OnScrolling(Vector2.zero);
    }

    void OnScrolling(Vector2 srollPosition)
    {
        if (Loop) scrollRect.horizontalNormalizedPosition = Mathf.Repeat(scrollRect.horizontalNormalizedPosition, dataCount * ScrollOffset);
        FocusIndexUpdate();
        MoveToTarget(scrollRect.horizontalNormalizedPosition);
    }

    void MoveToTarget(float NormalizePositionX)
    {
        float indexf = ScrollOffset == 0 ? 0 : NormalizePositionX / ScrollOffset;
        for(int i=0;i<Items.Count;i++)
        {
            itemRT = Items[i].GetComponent<RectTransform>();

            if(i > dataCount -1)
            {
                itemRT.localPosition = Slots[0].GetComponent<RectTransform>().localPosition;
                continue;
            }

            slotIndexf = midScrollPosition + i - indexf;
            int slotCount = (Slots.Count > dataCount) && Loop ? dataCount : Slots.Count;
            slotCount = dataCount == 1 ? Slots.Count : slotCount;
            slotIndexf = Mathf.Repeat(slotIndexf, slotCount);

            dataIndex = Mathf.RoundToInt(slotIndexf + indexf - midScrollPosition);

            if (Loop) dataIndex = (int)Mathf.Repeat(dataIndex, dataCount);

            if (dataIndex >= 0 && dataIndex < dataCount)
            {
                if (OnSetItemData != null)
                    OnSetItemData.Invoke(Items[i], dataIndex);
            }
            else
            {
                itemRT.localPosition = Slots[0].GetComponent<RectTransform>().localPosition;
                continue;
            }

            slotIndexL = Mathf.FloorToInt(slotIndexf);
            slotIndexR = Mathf.CeilToInt(slotIndexf);
            t = slotIndexf - slotIndexL;

            if(slotIndexL < 0 || slotIndexL >= Slots.Count)
            {
                itemRT.localPosition = Slots[0].GetComponent<RectTransform>().localPosition;
                continue;
            }
            else if(slotIndexR < 0 || slotIndexR >= Slots.Count)
            {
                itemRT.localPosition = Slots[Slots.Count - 1].GetComponent<RectTransform>().localPosition;
                continue;
            }

            slotLRT = Slots[slotIndexL].GetComponent<RectTransform>();
            slotRRT = Slots[slotIndexR].GetComponent<RectTransform>();
            itemRT.localPosition = Vector2.Lerp(slotLRT.localPosition,slotRRT.localPosition,t);
        }
    }

    void FocusIndexUpdate()
    {
        FocusIndex = GetFocusIndex(scrollRect.horizontalNormalizedPosition);
    }

    int GetFocusIndex(float NormalizedPosition)
    {
        float indexf = ScrollOffset == 0 ? 0 : NormalizedPosition / ScrollOffset;

        if (Loop) indexf = Mathf.Repeat(indexf,dataCount);

        tmp_index = Mathf.RoundToInt(indexf);

        if (Loop) tmp_index = (int)Mathf.Repeat(tmp_index,dataCount);

        if (tmp_index < 0) tmp_index = 0;
        if (tmp_index >= dataCount) tmp_index = dataCount - 1;
        return tmp_index;
    }

    float GetNPByIndex(int index)
    {
        float ret;
        float currentNP = scrollRect.horizontalNormalizedPosition;

        if (index == 0 && Loop && currentNP >= 1)
            ret = dataCount * ScrollOffset;
        else
            ret = index * ScrollOffset;
        return ret;
    }
}
