using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    public ItemScroll scroll;
    public int[] Datas = new int[10];

	void Start () {

        for(int i=0;i<Datas.Length;i++)
        {
            Datas[i] = i;
        }
        scroll.Initial(Datas.Length, 0, (go, dataIndex) => { go.GetComponentInChildren<Text>().text = Datas[dataIndex].ToString(); });
	}
	
}
