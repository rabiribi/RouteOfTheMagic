using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using scriptStructs;

public class Clickcontrol : MonoBehaviour {
    public MagicCore magic;
    public GameObject node;
    public GameObject linePerb;
    private GameObject instance;
    public static bool isDrag;
    // Use this for initialization
    void Start () {
        magic = new MagicCore();
        isDrag = false;
        instance = node;
        for(int i=0;i < 6;++i)
        {
            InitPoint(3, 120 - i*60);
        }
        for (int i = 0; i < 6; ++i)
        {
            InitPoint(3*Mathf.Sqrt(3), 90 - i * 60);
        }
        for (int i = 0; i < 3; ++i)
        {
            InitPoint(9, 90 - i * 120);
        }
        for (int i = 0; i < 3; ++i)
        {
            InitPoint(9, 150 - i * 120);
        }
        InitLine();
    }
	
	// Update is called once per frame
	void Update () {
        // GameObject.Find("ATK").GetComponent<Text>().text=
        // GameObject.Find("ATK").GetComponent<Text>().text=
    }
    public void startinit()
    {
        magic.startTurn();
    }

    public void InitPoint(float radium,float angle)
    {
        //算位置
        Vector3 mPos;
        mPos.x = radium * Mathf.Cos(angle / 180.0f * Mathf.PI);
        mPos.y = radium * Mathf.Sin(angle / 180.0f * Mathf.PI);
        mPos.z = 0;
       
        //实例化
        instance=GameObject.Instantiate(instance, mPos,Quaternion.identity);
        instance.tag = (int.Parse(instance.tag) + 1).ToString();
        instance.name = "Point" + instance.tag;
    }

    public void InitLine()
    {
        List<Line> lineList = magic.getLine();
        List<Point> Plist = magic.getPoint();
        foreach (Line l in lineList)
        {
            //获取两端节点
            int p1 = l.p1;
            int p2 = l.p2;
            //查询节点坐标
            Vector3 pos1 = GameObject.FindGameObjectWithTag(p1.ToString()).transform.position;
            Vector3 pos2 = GameObject.FindGameObjectWithTag(p2.ToString()).transform.position;
            //生成节点
            linePerb.GetComponent<LineRenderer>().SetPosition(0, pos1);
            linePerb.GetComponent<LineRenderer>().SetPosition(1, pos2);
            if (Plist[p1].MaxMagic!=0 && Plist[p2].MaxMagic != 0)
            {
                GameObject.Instantiate(linePerb).name = "Line" + l.roateID;
            }
        }
    }
    
}
