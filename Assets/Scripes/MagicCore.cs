using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using scriptStructs;

public class MagicCore : MonoBehaviour {
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public MagicCore()                                    //默认初始化
    {
        mLine = getInitLine();
        mPoint = getInitPoint();
        mRoute = new List<Move>();
        DragDoc = new List<Move>();
        subRoute = new List<int>();

        MaxHp = 100;
        MaxATK = 3;
        MaxDEF = 1;
        mPos = 0;

    }

    MagicCore(List<Line> lList, List<Point> pList)
    {

    }

    //全局参数
    protected int MaxHp;              //最大生命值
    protected int Hp;                 //当前HP
    protected int MaxATK;
    protected int MaxDEF;
    protected int ATK;                //攻击步数
    protected int DEF;                //防御步数
    protected int mPos;               //当前位置

    protected int turn;               //当前回合数
    protected int pointUsedCount;     //当前使用过的节点个数
    protected int paceCount;          //当前走过的路径数目

    protected List<Point> mPoint;     //节点列表
    protected List<Line> mLine;       //边列表
    protected List<Skill> mSkill;     //技能列表

    protected List<Move> mRoute;       //本回合已经走过的路径

    protected Magic skillReady;        //准备释放的技能
    protected List<int> subRoute;     //进入技能队列的节点

    //触发器状态
    ClickFlag cf;           //当前点击一个节点会发生什么

    //拖动记录
    List<Move> DragDoc;       //本回合已经走过的路径

    //人物的公用事件列表
    List<BuffBasic> buffList;

    int Adjacent(int p1, int p2)
    {
        int r = -1;
        if (p1 < 0 || p2 < 0 || p1 == p2)
            return -1;
        for (int i = 0; i < mPoint[p1].line.Count; i++)
        {
            int lp1 = mLine[mPoint[p1].line[i]].p1;
            int lp2 = mLine[mPoint[p1].line[i]].p2;

            if (lp1 == p2 || lp2 == p2)
                r = mLine[mPoint[p1].line[i]].roateID;
        }
        return r;
    }


    //自己用的工具
    void FreshSkillActivity()
    {
        for (int i = 0; i < mSkill.Count; i++)
        {
            Skill s = mSkill[i];
            List<PointColor> pc = s.mRequire;
            //正序
            if (pc[0] == mPoint[subRoute[0]].color && pc[pc.Count - 1] == mPoint[subRoute[subRoute.Count - 1]].color) //满足开头和节点要求
            {
                if (pc.Count > 2)
                {
                    //顺序搜索中间点
                    int pRoute = 1;
                    bool isActivity = true;
                    for (int cNo = 1; cNo < pc.Count - 1; ++cNo)
                    {
                        if (pRoute == subRoute.Count) //如果搜索完所有节点
                        {
                            isActivity = false;
                        }
                        if (pc[cNo] == mPoint[subRoute[pRoute]].color) //如果匹配的话
                        {
                            ++cNo;
                            ++pRoute;
                        }
                    }
                    s.useable = isActivity;
                }
            }
        }
    }

    void cosumeMagic(Magic m)
    {
        //消耗魔力(正序)
        List<int> sub = m.magicRoute;
        List<PointColor> pc = m.skill.mRequire;
        List<int> pr = m.skill.mRequireP;
        int pcID = 0;
        for (int i = 0; i < sub.Count; i++)
        {
            if (!mPoint[sub[i]].isBroken)
            {
                mPoint[sub[i]].magic -= 1;
                mPoint[sub[i]].isActivity = false;
                if (mPoint[sub[i]].color == pc[pcID])
                {
                    mPoint[sub[i]].magic -= pr[pcID];
                    ++pcID;
                }
                if (mPoint[sub[i]].magic < 0)
                {
                    mPoint[sub[i]].isBroken = true;
                }
            }
        }
    }

    //操作接口
    public void LclickP(int locate)             //左键点击节点时会发生的事件
    {
        if (cf == ClickFlag.normal)          //如果当前指令是通常状态
        {
            if (mPoint[locate].isActivity == true) //如果当前节点是激活状态
            {
                if (subRoute.Count == 0)    //如果当前节点为空，则添加节点1
                {
                    subRoute.Add(locate);
                }
                else                        //判断新节点是否与已有节点构成路径
                {
                    int pS = subRoute[0];
                    int pE = subRoute[subRoute.Count - 1];

                    for (int i = 0; i < mRoute.Count; ++i)
                    {
                        Move m = mRoute[i];
                        if (m.pEnd == locate && locate == pE + 1)    //如果是路径的后一个点
                        {
                            subRoute.Add(locate);
                            break;
                        }
                        else if (m.pStart == locate && locate == pS - 1)  //如果是路径内的前一个点
                        {
                            subRoute.Insert(0, locate);
                            break;
                        }
                    }
                }

                //完成后刷新所有技能的状态
                FreshSkillActivity();
            }
        }
    }

    public void LclickS(int skillNum)           //左键点击技能时会发生的事件
    {
        if (cf == ClickFlag.normal)
        {
            Skill s = mSkill[skillNum];
            if (s.useable == true)
            {
                skillReady.skill = s;      //保存准备释放的技能对象
                skillReady.magicRoute = subRoute;

                if (s.skillType != SkillType.allE || s.skillType != SkillType.randomE || s.skillType != SkillType.self)
                {
                    cf = ClickFlag.target;              //选择对象
                }
                else
                {
                    //释放技能
                    skillReady.skill.beforeDo(ref skillReady);
                    skillReady.skill.skillDo(ref skillReady);
                    //消耗魔力
                    cosumeMagic(skillReady);
                    //清空路径
                    pointUsedCount += subRoute.Count;
                    subRoute.Clear();
                }
            }

        }
    }

    public void LclockM(int monsterID)          //左键点击怪物时会发生的事件
    {
        if (cf == ClickFlag.target)       //设定目标完成，释放法术
        {
            skillReady.target = monsterID;
            //释放技能
            skillReady.skill.beforeDo(ref skillReady);
            skillReady.skill.skillDo(ref skillReady);
            //消耗魔力
            cosumeMagic(skillReady);
            //清空路径
            pointUsedCount += subRoute.Count;
            subRoute.Clear();
        }
    }

    public void drag(int locate)                //将当前位置节点拖动到其他节点时会发生的事件
    {
        int roadID = Adjacent(locate, mPos);

        if (ATK > 0 &&                            //只有攻击大于0才能移动
            roadID != -1 &&                       //只有相邻才能移动
            !mLine[roadID].isUnpassable &&        //只有连接路可以通过才能移动
            !mLine[roadID].isPassed &&            //只有连接路还没有走过才可以移动
            !mPoint[locate].isUnpassable &&       //只有目标节点可以移动才可以通过
            mPoint[locate].MaxMagic != 0)         //只有目标节点已经被点亮才可以通过
        {

            --ATK;

            Point p = mPoint[locate];
            p.isActivity = true;
            mPoint[locate] = p;

            Line l = mLine[roadID];
            l.isPassed = true;
            mLine[roadID] = l;

            Move m;
            m.pStart = mPos;
            m.pEnd = locate;
            m.moveLine = roadID;
            DragDoc.Add(m);

            mPos = locate;
        }
    }

    public void dragLoose()                     //松开拖动时的事件
    {
        //依次存入路径
        for (int i = 0; i < DragDoc.Count; ++i)
            mRoute.Add(DragDoc[i]);
        Debug.Log(mRoute.Count);
        paceCount += DragDoc.Count;
    }

    public void RclickP(int locate)             //鼠标右击时会发生的事件    
    {
        //按照拖动记录恢复上一部操作
        if (cf == ClickFlag.normal)
        {
            if (subRoute.Count == 0)            //如果没有选择节点操作
            {
                for (int i = DragDoc.Count - 1; i >= 0; --i)
                {
                    Move pMove = DragDoc[i];

                    Point p = mPoint[pMove.pEnd];
                    p.isActivity = false;
                    mPoint[pMove.pEnd] = p;

                    Line l = mLine[pMove.moveLine];
                    l.isPassed = false;
                    mLine[pMove.moveLine] = l;

                    //从mRoute清除路径
                    mRoute.RemoveAt(mRoute.Count - 1);

                    ++ATK;
                    mPos = pMove.pStart;
                }
                paceCount -= DragDoc.Count;
                DragDoc.Clear();
            }
            else                                  //清空节点选择
            {
                if (locate == -1)
                {
                    subRoute.Clear();
                    FreshSkillActivity();
                }
                else if (locate == subRoute[0])
                {
                    subRoute.RemoveAt(0);
                }
                else if (locate == subRoute[subRoute.Count - 1])
                {
                    subRoute.RemoveAt(subRoute.Count - 1);
                }

            }
        }

        //取消技能释放
        else if (cf == ClickFlag.target)
        {
            cf = ClickFlag.normal;
        }
    }

    //工具
    public void initCore(List<Point> pList, List<Line> lList)
    {
        mPoint = pList;
        mLine = lList;

        //moveEvent.Add(HAhaha, 1);
    }

    public void addBuff(BuffBasic buff, int pl)
    {
        //根据buff类型添加buff
        switch (buff.type)
        {
            case BuffType.pBuffBroken:
                mPoint[pl].buff.Add(buff);
                break;
            case BuffType.pBuffAttack:
                mPoint[pl].buff.Add(buff);
                break;
            case BuffType.pBuffDefence:
                mPoint[pl].buff.Add(buff);
                break;
            case BuffType.pBuffMoveIn:
                mPoint[pl].buff.Add(buff);
                break;
            case BuffType.pBuffMoveOut:
                mPoint[pl].buff.Add(buff);
                break;
            case BuffType.pBuffSkill:
                mPoint[pl].buff.Add(buff);
                break;

            case BuffType.lBuffDamage:
                mLine[pl].buff.Add(buff);
                break;
            case BuffType.lBuffDefence:
                mLine[pl].buff.Add(buff);
                break;
            case BuffType.lBuffPass:
                mLine[pl].buff.Add(buff);
                break;

            case BuffType.sBuffDamage:
                buffList.Add(buff);
                break;
            case BuffType.sBuffMove:
                buffList.Add(buff);
                break;
            case BuffType.sBuffSkill:
                buffList.Add(buff);
                break;
            case BuffType.sBuffStart:
                buffList.Add(buff);
                break;
            case BuffType.sBuffTurn:
                buffList.Add(buff);
                break;
            case BuffType.sBuffTurnEnd:
                buffList.Add(buff);
                break;
        }
    }

    public void addLineDefence(int lineID, float def)
    {
        Line l = mLine[lineID];
        l.def += def;
        mLine[lineID] = l;
    }

    public void startTurn()
    {
        ATK = MaxATK;
        DEF = MaxDEF;

        for (int i = 0; i < mPoint.Count; ++i)
        {
            Point p = mPoint[i];
            if (p.isActivity)
            {
                p.isActivity = false;
                if (p.magic < p.MaxMagic)
                    ++p.magic;
            }
            mPoint[i] = p;
        }

        for (int i = 0; i < mLine.Count; ++i)
        {
            Line l = mLine[i];
            l.isPassed = false;
            l.def = 0;
            mLine[i] = l;
        }

        mRoute.Clear();
        DragDoc.Clear();
    }

    //查询接口

    public Point getPoint(int pNo)
    {
        return mPoint[pNo];
    }

    public List<Line> getLine()
    {
        return mLine;
    }

    public Line getLine(int l)
    {
        return mLine[l];
    }

    public List<Line> getInitLine()
    {
        List<Line> r = new List<Line>();

        Line l = new Line(0, 0, 1);
        r.Add(l);

        l = new Line(1, 0, 2);
        r.Add(l);

        l = new Line(2, 0, 3);
        r.Add(l);

        l = new Line(3, 0, 4);
        r.Add(l);

        l = new Line(4, 0, 5);
        r.Add(l);

        l = new Line(5, 0, 6);
        r.Add(l);

        l = new Line(6, 1, 2);
        r.Add(l);

        l = new Line(7, 2, 3);
        r.Add(l);

        l = new Line(8, 3, 4);
        r.Add(l);

        l = new Line(9, 4, 5);
        r.Add(l);

        l = new Line(10, 5, 6);
        r.Add(l);

        l = new Line(11, 6, 1);
        r.Add(l);

        l = new Line(12, 1, 7);
        r.Add(l);

        l = new Line(13, 2, 7);
        r.Add(l);

        l = new Line(14, 2, 8);
        r.Add(l);

        l = new Line(15, 3, 8);
        r.Add(l);

        l = new Line(16, 3, 9);
        r.Add(l);

        l = new Line(17, 4, 9);
        r.Add(l);

        l = new Line(18, 4, 10);
        r.Add(l);

        l = new Line(19, 5, 10);
        r.Add(l);

        l = new Line(20, 5, 11);
        r.Add(l);

        l = new Line(21, 6, 11);
        r.Add(l);

        l = new Line(22, 6, 12);
        r.Add(l);

        l = new Line(23, 1, 12);
        r.Add(l);

        l = new Line(24, 12, 13);
        r.Add(l);

        l = new Line(25, 13, 8);
        r.Add(l);

        l = new Line(26, 8, 14);
        r.Add(l);

        l = new Line(27, 10, 14);
        r.Add(l);

        l = new Line(28, 10, 15);
        r.Add(l);

        l = new Line(29, 12, 15);
        r.Add(l);

        l = new Line(30, 16, 7);
        r.Add(l);

        l = new Line(31, 7, 17);
        r.Add(l);

        l = new Line(32, 9, 17);
        r.Add(l);

        l = new Line(33, 17, 18);
        r.Add(l);

        l = new Line(34, 11, 18);
        r.Add(l);

        l = new Line(35, 11, 16);
        r.Add(l);

        l = new Line(36, 0, 13);
        r.Add(l);

        l = new Line(37, 0, 14);
        r.Add(l);

        l = new Line(38, 0, 15);
        r.Add(l);

        return r;
    }

    public List<Point> getInitPoint()
    {
        List<Point> r = new List<Point>();

        Point p = new Point(0, PointColor.white, PointType.normal, 3, new List<int> { 0, 1, 2, 3, 4, 5 });
        r.Add(p);

        p = new Point(1, PointColor.black, PointType.normal, 1, new List<int> { 0, 6, 11, 12, 23 });
        r.Add(p);

        p = new Point(2, PointColor.black, PointType.normal, 1, new List<int> { 1, 6, 7, 13, 14 });
        r.Add(p);

        p = new Point(3, PointColor.black, PointType.normal, 1, new List<int> { 2, 7, 8, 15, 16 });
        r.Add(p);

        p = new Point(4, PointColor.black, PointType.normal, 1, new List<int> { 3, 8, 9, 17, 18 });
        r.Add(p);

        p = new Point(5, PointColor.black, PointType.normal, 1, new List<int> { 4, 9, 10, 19, 20 });
        r.Add(p);

        p = new Point(6, PointColor.black, PointType.normal, 1, new List<int> { 5, 10, 11, 21, 22 });
        r.Add(p);

        p = new Point(7, PointColor.red, PointType.normal, 0, new List<int> { 12, 13, 30, 31 });
        r.Add(p);

        p = new Point(8, PointColor.yellow, PointType.normal, 0, new List<int> { 14, 15, 25, 26 });
        r.Add(p);

        p = new Point(9, PointColor.blue, PointType.normal, 0, new List<int> { 16, 17, 23, 33 });
        r.Add(p);

        p = new Point(10, PointColor.red, PointType.normal, 0, new List<int> { 16, 17, 23, 33 });
        r.Add(p);

        p = new Point(11, PointColor.yellow, PointType.normal, 0, new List<int> { 16, 17, 23, 33 });
        r.Add(p);

        p = new Point(12, PointColor.blue, PointType.normal, 0, new List<int> { 13, 15, 22, 23 });
        r.Add(p);

        p = new Point(13, PointColor.black, PointType.normal, 0, new List<int> { 24, 25, 36 });
        r.Add(p);

        p = new Point(14, PointColor.black, PointType.normal, 0, new List<int> { 26, 27, 37 });
        r.Add(p);

        p = new Point(15, PointColor.black, PointType.normal, 0, new List<int> { 28, 29, 38 });
        r.Add(p);

        p = new Point(16, PointColor.white, PointType.normal, 0, new List<int> { 30, 35 });
        r.Add(p);

        p = new Point(17, PointColor.white, PointType.normal, 0, new List<int> { 31, 32 });
        r.Add(p);

        p = new Point(18, PointColor.white, PointType.normal, 0, new List<int> { 33, 34 });
        r.Add(p);

        return r;
    }

    public int getTurn() { return turn; }

    public List<Move> getRoute()
    {
        return mRoute;
    }

    public List<int> getSubRoute()
    {
        return subRoute;
    }

    public List<Point> getPoint()
    {
        return mPoint;
    }

    public int getPos()
    {
        return mPos;
    }

    public int getCurrentPos()
    {
        return mPos;
    }

    public PointColor getPointColor(int p)
    {
        return mPoint[p].color;
    }

    public lineState getLineState(int l)
    {
        lineState ls = lineState.normal;
        //查看这个节点是不是passed
        if (mLine[l].isPassed)
            ls = lineState.used;
        //看看这个边是不是拖动中
        for (int i = 0; i < DragDoc.Count; ++i)
        {
            if (DragDoc[i].moveLine == l)
                ls = lineState.drag;
        }
        for (int i = 0; i < mRoute.Count; ++i)
        {
            if (mRoute[i].moveLine == l)
                ls = lineState.light;
        }
        return ls;
    }

    public int getATK()
    {
        return ATK;
    }

    public int getDEF()
    {
        return DEF;
    }

    public int getMaxATK()
    {
        return MaxATK;
    }

    public int getMaxDEF()
    {
        return MaxDEF;
    }

    public int getHP()
    {
        return Hp;
    }

    public int getMaxHP()
    {
        return MaxHp;
    }

    public int getPaceCount()
    {
        return paceCount;
    }

    public int getUsedPCount()
    {
        return pointUsedCount;
    }

    //设置接口
    public void setATK(int a)
    {
        ATK = a;
    }

    public void setDEF(int d)
    {
        DEF = d;
    }

    public void setHP(int hp)
    {
        Hp = hp;
    }

    public void clearDragDoc()
    {
        DragDoc.Clear();
    }
}






