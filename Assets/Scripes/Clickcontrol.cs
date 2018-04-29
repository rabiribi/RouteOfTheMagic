using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RouteOfTheMagic;

public class Clickcontrol : MonoBehaviour {

    public MagicCore magic;
    public Monster monster;
    public GameObject node;
    public GameObject linePerb;
    public GameObject monster0;
    public List<GameObject> skillList;
    private GameObject instance;
    private GameObject btnGameObject;
    private List<GameObject> lineGameObjectlist;

    public static bool isDrag;
    private bool isAttacking;
    // Use this for initialization
    void Start () {
        magic = MagicCore.Instance;
        monster = new Monster();
        lineGameObjectlist = new List<GameObject>();
        isDrag = false;
        isAttacking = false;
        instance = node;

        magic.addMonster(monster0.GetComponent<Monster>());
        magic.startTurn();

        //初始化节点位置
        InitPointPos();
        //初始化连线
        InitLine();
        
    }
	
	// Update is called once per frame
	void Update () {
        //显示ATK和DEF
        GameObject.Find("ATK").GetComponent<Text>().text = "ATK: "+magic.getATK().ToString();
        GameObject.Find("DEF").GetComponent<Text>().text = "DEF: "+magic.getDEF().ToString();
        GameObject.Find("HP").GetComponent<Text>().text = "HP:" + magic.getHP().ToString();
        //测试monster，获取血量等
        monster0.GetComponentInChildren<Text>().text = monster0.GetComponent<Monster>().monsterHP.ToString();
        //绘制连线颜色
        drawLineColor();

        //设定skill的状态
        skillStatus();

        //检查线上的信息
        lineStatus();
    }
    //初始化
    public void startinit()
    {
        btnGameObject = EventSystem.current.currentSelectedGameObject;
        if (magic.getFlag() == ClickFlag.defencer)
        {
            magic.startTurn();
            btnGameObject.GetComponent<Image>().color = Color.red;
        }
        else if (magic.getFlag() == ClickFlag.normal || magic.getFlag() == ClickFlag.target)
        {
            magic.endTurn();
            magic.setFlag(ClickFlag.defencer);
            btnGameObject.GetComponent<Image>().color = Color.green;
      
        }

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

    public void InitPointPos()
    {
        for (int i = 0; i < 6; ++i)
        {
            InitPoint(3, 120 - i * 60);
        }
        for (int i = 0; i < 6; ++i)
        {
            InitPoint(3 * Mathf.Sqrt(3), 90 - i * 60);
        }
        for (int i = 0; i < 3; ++i)
        {
            InitPoint(9, 90 - i * 120);
        }
        for (int i = 0; i < 3; ++i)
        {
            InitPoint(9, 150 - i * 120);
        }
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
            //生成线
            linePerb.GetComponent<LineRenderer>().SetPosition(0, pos1);
            linePerb.GetComponent<LineRenderer>().SetPosition(1, pos2);
           

            GameObject lineP = GameObject.Instantiate(linePerb);
            lineP.SetActive(false);
            lineGameObjectlist.Add(lineP);

            if (Plist[p1].MaxMagic!=0 && Plist[p2].MaxMagic != 0)
            {
                lineP.SetActive(true);
            }
                

        }
    }
    
    public Color toLineColor(lineState lineSt)
    {
        Color lineColor = new Color();
        switch (lineSt)
        {
            case lineState.drag:
                lineColor = Color.yellow;
                break;
            case lineState.light:
                lineColor = Color.blue;
                break;
            case lineState.normal:
                lineColor = Color.black;
                break;
            case lineState.used:
                lineColor = Color.red;
                break;
        }
        return lineColor;
    }

    //点击技能触发
    public void toSkill()
    {
        btnGameObject = EventSystem.current.currentSelectedGameObject;
        int skillID = int.Parse(btnGameObject.name);
        magic.LclickS(skillID);
    }

    //点击怪物
    public void toMonster()
    {
        btnGameObject = EventSystem.current.currentSelectedGameObject;
        int monsterID = int.Parse(btnGameObject.name);
        magic.LclickM(monsterID);
    }

    //绘制连线颜色
    public void drawLineColor()
    {
        for (int i = 0; i < lineGameObjectlist.Count; ++i)
        {
            Color temp = toLineColor(magic.getLineState(i));
            lineGameObjectlist[i].GetComponent<LineRenderer>().startColor = temp;
            lineGameObjectlist[i].GetComponent<LineRenderer>().endColor = temp;
        }
    }

    //设定skill的状态
    public void skillStatus()
    {
        foreach (GameObject sk in skillList)
        {
            if (!magic.getSkillActivity(int.Parse(sk.name)))
            {
                sk.GetComponent<Button>().interactable = false;
            }
            else
            {
                sk.GetComponent<Button>().interactable = true;
            }
            if (int.Parse(sk.name) + 1 > magic.getSkillCap())
            {
                sk.SetActive(false);
            }
            else
            {
                sk.SetActive(true);
            }
        }
    }
    

    //检查线上的信息
    public void lineStatus()
    {
        List<Line> lineList = magic.getLine();
        List<EDamage>edList = magic.getMonsterATK();
        foreach (EDamage ed in edList)
        {
            if (ed.damage != 0)
            {
                foreach (Transform child in lineGameObjectlist[ed.ID].transform)
                {
                    Vector3 pos1 = child.parent.GetComponent<LineRenderer>().GetPosition(0);
                    Vector3 pos2 = child.parent.GetComponent<LineRenderer>().GetPosition(1);
                    if (child.name == "Damage")
                    {
                        child.position = new Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2 + 0.5f, 0);
                        child.GetComponent<TextMesh>().text = ed.damage.ToString();
                    }
                    else if (child.name == "Defence")
                    {
                        child.position = new Vector3((pos1.x + pos2.x) / 2, (pos1.y + pos2.y) / 2 - 0.5f, 0);
                        child.GetComponent<TextMesh>().text = lineList[ed.ID].def.ToString();
                    }
                }
            }
            else
            {
                foreach (Transform child in lineGameObjectlist[ed.ID].transform)
                {
                    child.GetComponent<TextMesh>().text = null;
                }
            }
        }
    }
}
