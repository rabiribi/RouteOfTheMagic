using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RouteOfTheMagic;

public class MagicCore{
    public MagicCore()                                    //默认初始化
    {
        if (instance == null)
            instance = this;
        mLine = getInitLine();
        mPoint = getInitPoint();
        mRoute = new List<Move>();
        DragDoc = new List<Move>();

        skillTool = new SkillTool();
        skillTool.magicCore = this;
        mSkill = skillTool.getInitSkills();
        mMonster = new List<Monster>();
        mMonsterAttack = new List<EDamage>();

        MaxHp = 100;
        MaxATK = 10;
        MaxDEF = 1;
        Hp = MaxHp;
        mPos = 0;

        skillPoint = 3;

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

    protected SkillTool skillTool = new SkillTool();    //技能工具

    protected List<Point> mPoint;     //节点列表
    protected List<Line> mLine;       //边列表
    protected List<Skill> mSkill;     //技能列表
    protected List<Monster> mMonster; //怪物列表
    protected List<EDamage> mMonsterAttack;  //怪物攻击列表

    protected List<Move> mRoute;       //本回合已经走过的路径

    protected Magic skillReady;        //准备释放的技能

    //触发器状态
    ClickFlag cf;           //当前点击一个节点会发生什么

    //拖动记录
    List<Move> DragDoc;       //本回合已经走过的路径

    //人物的公用事件列表
    List<BuffBasic> buffList;

    //全局变量
    public int skillPoint; //剩余技能点数
    public int Money;      //金钱

    private static MagicCore instance;

    public static MagicCore Instance
    {
        get
        {
            if (instance == null)
            {
                instance =new MagicCore();
                Debug.Log("shilighua");
            }
            return instance;
        }
    }

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

            if (getSuitRoute(pc, s.skillDoType).Count != 0)
            {
                s.useable = true;
            }
            else
            {
                s.useable = false;
            }

        }
    }

    List<int> getSuitRoute(List<PointColor> pc, SkillDoType sdt)
    {
        List<int> subRoute = new List<int>();
        List<PointColor> require = new List<PointColor>();
        foreach (PointColor p in pc)
        {
            require.Add(p);
        }

        //首先正序判断一次=======================================================================================================
        if (sdt == SkillDoType.oneWay || sdt == SkillDoType.twoWay)
        {
            int subRstart = -1;
            int subRend = -1;

            //找到开始节点
            for (int i = 0; i < mRoute.Count; ++i)
            {
                if (subRstart != -1)
                    break;
                if (mPoint[mRoute[i].pEnd].color == require[0] && !mPoint[mRoute[i].pEnd].isBroken)
                {
                    subRstart = i;
                    require.RemoveAt(0);
                }
            }
            if (subRstart != -1)    //如果找到，才继续
            {

                //依次判断中间节点
                for (int i = subRstart; i < mRoute.Count; ++i)
                {
                    if (require.Count == 1) //如果所有中间节点已经处理完毕
                        break;
                    if (mPoint[mRoute[i].pEnd].color == require[0] && !mPoint[mRoute[i].pEnd].isBroken)
                    {
                        subRstart = i;
                        require.RemoveAt(0);
                    }
                }
                if (require.Count == 1)  //如果中间点都满足，才继续
                {
                    //判断终点
                    for (int i = subRstart + 1; i < mRoute.Count; ++i)
                    {
                        if (mPoint[mRoute[i].pEnd].color == require[0] && !mPoint[mRoute[i].pEnd].isBroken)
                        {
                            subRend = i;
                        }
                    }
                    if (subRend != -1)   //找到才添加
                    {
                        //添加
                        subRoute.Add(subRstart);
                        subRoute.Add(subRend);
                        subRoute.Add(0);
                    }
                }
            }
        }

        //给可以倒叙的一次机会判断下倒叙=======================================================================================================
        if (sdt == SkillDoType.twoWay)
        {
            int subRstart = -1;
            int subRend = -1;

            //找到开始节点
            for (int i = 0; i < mRoute.Count; ++i)
            {
                if (subRstart != -1)
                    break;
                if (mPoint[mRoute[i].pEnd].color == require[require.Count - 1] && !mPoint[mRoute[i].pEnd].isBroken)
                {
                    subRstart = i;
                    require.RemoveAt(require.Count - 1);
                }
            }
            if (subRstart == -1)    //如果没找到，就直接退出
            {
                return subRoute;
            }

            //依次判断中间节点
            for (int i = subRstart; i < mRoute.Count; ++i)
            {
                if (require.Count == 1) //如果所有中间节点已经处理完毕
                    break;
                if (mPoint[mRoute[i].pEnd].color == require[require.Count - 1] && !mPoint[mRoute[i].pEnd].isBroken)
                {
                    subRstart = i;
                    require.RemoveAt(require.Count - 1);
                }
            }
            if (require.Count > 1)
            {
                return subRoute;   //如果遍历完了还是无法满足中间节点的要求，退出
            }

            //判断终点
            for (int i = subRstart + 1; i < mRoute.Count; ++i)
            {
                if (mPoint[mRoute[i].pEnd].color == require[require.Count - 1] && !mPoint[mRoute[i].pEnd].isBroken)
                {
                    subRend = i;
                }
            }
            if (subRend == -1) //如果终点判定不通过，退出
            {
                return subRoute;
            }

            //判断是否要添加
            if (subRoute.Count == 2 && subRend - subRstart > subRoute[1] - subRoute[0])
            {
                subRoute.Clear();
                subRoute.Add(subRstart);
                subRoute.Add(subRend);
                subRoute.Add(1);
            }
        }
        //如果要求无序的话===================================================================================
        if (sdt == SkillDoType.unorder)
        {
            int uoS = -1;
            int uoE = -1;
            int isPos = -1;

            //正着找
            {
                int pS = -1;
                int pE = -1;

                //找头
                for (int i = 0; i < mRoute.Count; ++i)
                {
                    for (int j = 0; j < require.Count; ++j)
                        if (mPoint[mRoute[i].pEnd].color == require[j] && !mPoint[mRoute[i].pEnd].isBroken)
                        {
                            require.RemoveAt(j);
                            pS = i;
                        }
                }
                //找尾
                for (int i = mRoute.Count - 1; i >= 0; --i)
                {
                    for (int j = 0; j < require.Count; ++j)
                        if (mPoint[mRoute[i].pEnd].color == require[j] && !mPoint[mRoute[i].pEnd].isBroken)
                        {
                            require.RemoveAt(j);
                            pE = i;
                        }
                }
                if (pS != -1 && pE != -1 && pE >= pS && require.Count > 0) //如果找到了合适的头尾点,并且还需要判断中间点
                {
                    //识别中间点
                    for (int i = pS + 1; i < pE; ++i)
                    {
                        for (int j = 0; j < require.Count; ++j)
                            if (mPoint[mRoute[i].pEnd].color == require[j] && !mPoint[mRoute[i].pEnd].isBroken)
                            {
                                require.RemoveAt(j);
                            }
                    }
                    if (require.Count == 0)
                    {
                        //完全符合条件
                        uoS = pS;
                        uoE = pE;
                        isPos = 0;
                    }
                }
            }

            //倒着找
            {
                int pS = -1;
                int pE = -1;

                //找尾
                for (int i = mRoute.Count - 1; i >= 0; --i)
                {
                    for (int j = 0; j < require.Count; ++j)
                        if (mPoint[mRoute[i].pEnd].color == require[j] && !mPoint[mRoute[i].pEnd].isBroken)
                        {
                            require.RemoveAt(j);
                            pE = i;
                        }
                }
                //找头
                for (int i = 0; i < mRoute.Count; ++i)
                {
                    for (int j = 0; j < require.Count; ++j)
                        if (mPoint[mRoute[i].pEnd].color == require[j] && !mPoint[mRoute[i].pEnd].isBroken)
                        {
                            require.RemoveAt(j);
                            pS = i;
                        }
                }
                if (pS != -1 && pE != -1 && pE >= pS && require.Count > 0) //如果找到了合适的头尾点,并且还需要判断中间点
                {
                    //识别中间点
                    for (int i = pS + 1; i < pE; ++i)
                    {
                        for (int j = 0; j < require.Count; ++j)
                            if (mPoint[mRoute[i].pEnd].color == require[j] && !mPoint[mRoute[i].pEnd].isBroken)
                            {
                                require.RemoveAt(j);
                            }
                    }
                    if (require.Count == 0)                        //完全符合条件
                    {
                        if (pE - pS > uoE - uoS)         //并且长度较大
                        {
                            uoS = pS;
                            uoE = pE;
                            isPos = 1;
                        }
                    }
                }
            }
            if (uoE != -1 && uoS != -1)
            {
                subRoute.Add(uoS);
                subRoute.Add(uoE);
                subRoute.Add(isPos);
            }

        }
        if (sdt == SkillDoType.single)
        {
            //顺序找第一个点
            for (int i = 0; i < mRoute.Count; ++i)
            {
                if (mPoint[mRoute[i].pEnd].color == pc[0] && !mPoint[mRoute[i].pEnd].isBroken)
                {
                    subRoute.Add(i);
                    subRoute.Add(i);
                    subRoute.Add(0);
                    break;
                }
            }
        }
        if (sdt == SkillDoType.norequire)
        {
            subRoute.Add(subRoute.Count - 1);
            subRoute.Add(subRoute.Count - 1);
            subRoute.Add(0);
        }

        return subRoute;
    }

    void cosumeMagic(Magic m)
    {
        int RStart = m.magicRoute[0];
        int REnd = m.magicRoute[1];
        int isPos = m.magicRoute[2];
        List<PointColor> pc = new List<PointColor>();
        foreach (PointColor c in m.skill.mRequire)
        {
            pc.Add(c);
        }
        List<int> pr = new List<int>();
        foreach (int c in m.skill.mRequireP)
        {
            pr.Add(c);
        }
        int pcID = 0;

        //恢复魔力
        for (int i = 0; i < RStart; ++i)
        {
            recoverMagic(mRoute[i].pEnd);
        }

        //判断方向
        if (RStart == REnd) //如果只有一个节点
        {
            mPoint[mRoute[REnd].pEnd].magic -= pr[pr.Count];
            mPoint[mRoute[REnd].pEnd].magic -= 1;
        }
        //如果是正序
        if (isPos == 0)
        {
            //释放末尾和开头
            mPoint[mRoute[REnd].pEnd].magic -= pr[pr.Count - 1];
            mPoint[mRoute[REnd].pEnd].magic -= 1;
            pc.RemoveAt(pc.Count - 1);
            pr.RemoveAt(pr.Count - 1);
            

            mPoint[mRoute[RStart].pEnd].magic -= pr[0];
            mPoint[mRoute[RStart].pEnd].magic -= 1;
            pc.RemoveAt(0);
            pr.RemoveAt(0);

            //如果不是无序就按顺序释放

            if (m.skill.skillDoType != SkillDoType.unorder)
            {
                pcID = 0;
                for (int i = RStart + 1; i < REnd; ++i)
                {
                    mPoint[mRoute[i].pEnd].magic -= 1;
                    if (pc.Count != 0)
                        if (mPoint[mRoute[i].pEnd].color == pc[pcID])
                        {
                            mPoint[mRoute[i].pEnd].magic -= pr[pcID];
                            ++pcID;
                            if (pcID == pc.Count)
                                break;
                        }
                }
            }
            else  //否则随便释放
            {
                for (int i = RStart + 1; i < REnd; ++i)
                {
                    for (int j = 0; j < pc.Count; ++j)
                    {
                        mPoint[mRoute[i].pEnd].magic -= 1;
                        if (pc.Count != 0)
                            if (mPoint[mRoute[i].pEnd].color == pc[j])
                            {
                                mPoint[mRoute[i].pEnd].magic -= pr[j];
                                pc.RemoveAt(j);
                                pr.RemoveAt(j);
                                --j;
                            }
                    }
                }
            }
        }
        else   //如果是倒序
        {
            //释放末尾和开头
            mPoint[mRoute[REnd].pEnd].magic -= pr[pr.Count - 1];
            mPoint[mRoute[REnd].pEnd].magic -= 1;
            pc.RemoveAt(pc.Count - 1);
            pr.RemoveAt(pr.Count - 1);

            mPoint[mRoute[RStart].pEnd].magic -= pr[0];
            mPoint[mRoute[RStart].pEnd].magic -= 1;
            pc.RemoveAt(0);
            pr.RemoveAt(0);

            //如果不是无序就按顺序释放
            if (m.skill.skillDoType != SkillDoType.unorder)
            {
                pcID = 0;
                for (int i = RStart + 1; i < REnd; ++i)
                {
                    mPoint[mRoute[i].pEnd].magic -= 1;
                    if (pc.Count != 0)
                        if (mPoint[mRoute[i].pEnd].color == pc[pcID])
                        {
                            mPoint[mRoute[i].pEnd].magic -= pr[pcID];
                            ++pcID;
                            if (pcID == pc.Count)
                                break;
                        }
                }
            }
            else  //否则随便释放
            {
                for (int i = RStart + 1; i < REnd; ++i)
                {
                    for (int j = 0; j < pc.Count; ++j)
                    {
                        mPoint[mRoute[i].pEnd].magic -= 1;
                        if (pc.Count != 0)
                            if (mPoint[mRoute[i].pEnd].color == pc[j])
                            {
                                mPoint[mRoute[i].pEnd].magic -= pr[j];
                                pc.RemoveAt(j);
                                pr.RemoveAt(j);
                                --j;
                            }
                    }
                }
            }
        }

        //更新mRoute
        for (int i = 0; i < REnd; ++i)
        {
            mRoute.RemoveAt(0);
        }
        Move move = mRoute[0];
        move.pStart = move.pEnd;
        move.moveLine = -1;
        mRoute[0] = move;
    }

    void recoverMagic(int id)
    {
        //回复魔力
        mPoint[id].magic += 1;
        if (mPoint[id].magic > mPoint[id].MaxMagic)
        {
            mPoint[id].magic = mPoint[id].MaxMagic;
        }
        //取消激活
        mPoint[id].isActivity = false;
        //魔力放出伤害
        //执行回复魔力事件
    }

    void detectPointBroken()
    {
        foreach (Point p in mPoint)
        {
            if (p.magic < 0)
            {
                p.isBroken = true;
            }
        }
    }

    void initMonsterAttack()
    {
        mMonsterAttack.Clear(); // 清空
        for(int mCount = 0;mCount < mMonster.Count;++mCount)
        {
            Monster m = mMonster[mCount];
            if (m.monsterHP > 0)
            {
                List<int> atkList = m.attackDeclaration();
                int power = m.attackValue;

                for (int i = 0; i < mLine.Count; ++i)
                {
                    EDamage ed = new EDamage();
                    ed.damage = 0;
                    ed.ID = i;
                    ed.sorce = new List<int>();
                    foreach (int atkline in atkList)
                    {
                        if (atkline == i)
                        {
                            ed.damage = power;
                            ed.sorce.Add(mCount);
                        }
                    }
                    mMonsterAttack.Add(ed);
                }
                
            }
        }
        
    }

    void freshMonsterAttack()
    {
        for (int mCount = 0; mCount < mMonster.Count; ++mCount)
        {
            Monster m = mMonster[mCount];
            if (m.monsterHP <= 0)
            {
                
                int power = m.attackValue;
                foreach (EDamage ed in mMonsterAttack)
                {
                    for (int i = 0; i < ed.sorce.Count; ++i)
                    {
                        if (ed.sorce[i] == mCount)
                        {
                            ed.sorce.RemoveAt(i);
                            ed.damage -= power;
                            if (ed.damage <= 0)
                            {
                                ed.damage = 0;
                            }
                        }
                    }
                }
            }
        }
    }

    //操作接口
    public void doAttackToMonster(int monsterID, int count, int damage)
    {
        for (int i = 0; i < count; ++i)
        {
            mMonster[monsterID].getDamage(damage);
        }
    }

    public void doDefence()
    {
        foreach (EDamage ed in mMonsterAttack)
        {
            if (ed.damage != 0)
            {
                int mATK = ed.damage;
                int i = ed.ID;

                if (mLine[i].def > mATK)
                {
                    //消耗防御力
                    mLine[i].def -= mATK;
                }
                else
                {
                    int p1M = -1, p2M = -1;
                    if (mPoint[mLine[i].p1].isDefence && !mPoint[mLine[i].p1].isBroken)
                    {
                        p1M = mPoint[mLine[i].p1].magic;
                    }
                    if (mPoint[mLine[i].p2].isDefence && !mPoint[mLine[i].p2].isBroken)
                    {
                        p2M = mPoint[mLine[i].p2].magic;
                    }

                    if (p1M == -1 && p2M == -1)
                    {
                        int dam = mATK - mLine[i].def;
                        Hp -= dam;

                        //执行伤害事件
                    }
                    else if (p1M > p2M)
                    {
                        mPoint[mLine[i].p1].magic -= 1;
                    }
                    else if (p1M < p2M)
                    {
                        mPoint[mLine[i].p2].magic -= 1;
                    }
                    else
                    {
                        if (mPoint[mLine[i].p1].color == PointColor.black)
                            mPoint[mLine[i].p1].magic -= 1;
                        else
                            mPoint[mLine[i].p2].magic -= 1;
                    }
                }
            }
        }

        detectPointBroken();
    }

    public void LclickP(int locate)             //左键点击节点时会发生的事件
    {
        if (cf == ClickFlag.normal)          //如果当前指令是通常状态
        {
            if (mPoint[locate].isActivity == true) //如果当前节点是激活状态
            {
                //在mRoute里搜索
                int Loc = -1;
                for (int i = 0; i < mRoute.Count; ++i)
                {
                    if (mRoute[i].pEnd == locate)
                    {
                        Loc = i;
                        break;
                    }
                }
                if (Loc != -1)
                {
                    for (int i = 0; i <= Loc; ++i)
                    {
                        recoverMagic(mRoute[0].pEnd);
                        mRoute.RemoveAt(0);
                    }

                    if (mRoute.Count != 0)
                    {
                        Move m = mRoute[0];
                        m.pStart = m.pEnd;
                        m.moveLine = -1;
                        mRoute[0] = m;
                    }
                    else
                    {
                        Move m = new Move();
                        m.pStart = mPos;
                        m.pEnd = mPos;
                        m.moveLine = -1;
                        mRoute.Add(m);
                    }

                    foreach (Move m in mRoute)
                    {
                        if (m.pEnd == locate)
                            mPoint[locate].isActivity = true;
                    }
                }
            }

            FreshSkillActivity();
        }
        if (cf == ClickFlag.defencer)
        {
            if (!mPoint[locate].isBroken && (Adjacent(locate, mPos) != -1||locate == mPos) && DEF > 0)
            {
                mPoint[locate].isDefence = true;
                --DEF;
            }
        }
        if (cf == ClickFlag.upgrade)
        {
            if(isPointUpgradable(locate))
                pointUpgrade(locate);

        }
        if (cf == ClickFlag.transToBlue)
        {
            if (isPointTransable(locate))
                pointTrans(locate, PointColor.blue);
        }
        if (cf == ClickFlag.transToRed)
        {
            if (isPointTransable(locate))
                pointTrans(locate, PointColor.red);
        }
        if (cf == ClickFlag.transToYellow)
        {
            if (isPointTransable(locate))
                pointTrans(locate, PointColor.yellow);
        }
        if (cf == ClickFlag.TransToWhite)
        {
            if (isPointTransable(locate))
                pointTrans(locate, PointColor.white);
        }
    }

    public void LclickS(int skillNum)           //左键点击技能时会发生的事件
    {
        if (cf == ClickFlag.normal && skillNum < mSkill.Count)
        {
            Skill s = mSkill[skillNum];
            if (s.useable == true)
            {


                skillReady.skill = s;               //保存准备释放的技能对象
                skillReady.magicRoute = getSuitRoute(s.mRequire, s.skillDoType);   //获取技能的子路径

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
                    s.useable = false;
                    cosumeMagic(skillReady);
                    detectPointBroken();
                    //刷新怪物攻击
                    freshMonsterAttack();
                    //统计变化
                    pointUsedCount += skillReady.magicRoute[1] - skillReady.magicRoute[0] + 1;
                }
            }

        }
    }

    public void LclickM(int monsterID)          //左键点击怪物时会发生的事件
    {
        if (cf == ClickFlag.target)       //设定目标完成，释放法术
        {
            skillReady.target = monsterID;
            //释放技能
            //skillReady.skill.beforeDo(ref skillReady);
            skillReady.skill.skillDo(ref skillReady);
            skillReady.skill.useable = false;
            //消耗魔力
            cosumeMagic(skillReady);
            detectPointBroken();
            //刷新怪物攻击
            freshMonsterAttack();
            //清空路径
            pointUsedCount += skillReady.magicRoute[1] - skillReady.magicRoute[0] + 1;
            //改变点击状态
            cf = ClickFlag.normal;
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

        paceCount += DragDoc.Count;
        DragDoc.Clear();
        FreshSkillActivity();
    }

    public void RclickP(int locate)             //鼠标右击时会发生的事件    
    {
        //按照拖动记录恢复上一部操作
        if (cf == ClickFlag.normal)
        {
            while (DragDoc.Count > 0)
            {
                Move m = DragDoc[DragDoc.Count - 1];
                mPoint[m.pEnd].isActivity = false;
                mLine[m.moveLine].isPassed = false;
                mPos = m.pStart;

                DragDoc.RemoveAt(DragDoc.Count - 1);
                ++ATK;
            }
        }

        if (cf == ClickFlag.defencer)
        {
            foreach (Point p in mPoint)
            {
                p.isDefence = false;
                ++DEF;
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

    public void addLineDefence(int lineID, int def)
    {
        if (lineID != -1)
        {
            Line l = mLine[lineID];
            l.def += def;
            mLine[lineID] = l;
        }
    }

    public void startTurn()
    {
        //挨打
        doDefence();

        cf = ClickFlag.normal;
        //回合开始========================================
        ATK = MaxATK;
        DEF = MaxDEF;

        //存入初始路径
        Move m;
        m.pStart = mPos;
        m.pEnd = mPos;
        m.moveLine = -1;

        mRoute.Add(m);

        mPoint[mPos].isActivity = true;

        foreach (Point p in mPoint)
        {
            p.isDefence = false;
        }


        //生成怪物攻击
        initMonsterAttack();

        //执行开始事件

        //刷新怪物攻击
        freshMonsterAttack();
    }

    public void endTurn()
    {
        
        //回复魔力
        foreach (Move m in mRoute)
        {
            recoverMagic(m.pEnd);
        }
        
        //恢复节点状态
        for (int i = 0; i < mLine.Count; ++i)
        {
            Line l = mLine[i];
            l.isPassed = false;
            l.def = 0;
            mLine[i] = l;
        }

        foreach (Point p in mPoint)
        {
            p.isActivity = false;
            p.isDefence = false;
        }

       

        FreshSkillActivity();
        mRoute.Clear();
        DragDoc.Clear();
    }

    public List<EDamage> getMonsterATK()
    { 
        return mMonsterAttack;
    }

    public void Victory()
    {
        //清除所有的战斗状态（不含道具）
        for (int i = 0; i < buffList.Count; ++i)
        {
            BuffBasic b = buffList[i];
            if (b.turn > 0)
            {
                buffList.RemoveAt(i);
            }
        }

        //清除怪物列表
        mMonster.Clear();
        mMonsterAttack.Clear();

        //将魔术盘回复到完整状态
        foreach (Point p in mPoint)
        {
            p.buff.Clear();
            p.isActivity = false;
            p.isBroken = false;
            p.isDefence = false;
            p.isUnpassable = false;
            p.magic = p.MaxMagic;
        }

        foreach (Line l in mLine)
        {
            l.isPassed = false;
            l.isUnpassable = false;
        }

        //获取战斗胜利的奖励
        skillPoint += 1;
        Money += 10;
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
        if (cf == ClickFlag.defencer)
        {
            return ls;
        }
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

    public bool getPointBroked(int i)
    {
        return mPoint[i].isBroken;
    }

    public bool getSkillActivity(int skillID)
    {
        bool r = false;
        if(skillID < mSkill.Count)
            r = mSkill[skillID].useable;
        return r;
    }

    public int getSkillCap()
    {
        return mSkill.Count;
    }

    public ClickFlag getFlag()
    {
        return cf;
    }

    public bool isMonsterLive(int i)
    {
        bool r = false;
        if (i < mMonster.Count)
        {
            if (mMonster[i].monsterHP > 0)
                r = true;
        }
        return r;
    }

    public bool isDefencer(int i)
    {
        return mPoint[i].isDefence;
    }

    public bool isDragChangged()
    {
        bool r = false;
        if (DragDoc.Count > 0)
        {
            r = true;
        }
        return r;
    }

    /// <summary>
    /// 节点可以升级么
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool isPointUpgradable(int id)
    {
        Point p = mPoint[id];
        int need = 1;
        if (p.color != PointColor.black)
        {
            need = 2;
        }
        return skillPoint >= need;
    }

    //查询节点是否可以变质
    public bool isPointTransable(int id)
    {
        bool r = false;
        if (mPoint[id].color == PointColor.black && mPoint[id].MaxMagic >= 5 && skillPoint >= 3)
        {
            r = true;
        }
        return r;
    }

   /// <summary>
   /// 升级节点
   /// </summary>
   /// <param name="id"></param>
    public void pointUpgrade(int id)
    {
        Point p = mPoint[id];
        if (p.color == PointColor.black)
        {
            if (skillPoint >= 1)
            {
                --skillPoint;
                ++p.MaxMagic;
                p.magic = p.MaxMagic;
            }
        }
        else if (skillPoint >= 2)
        {
            skillPoint -= 2;
            if (p.color == PointColor.white)
            {
                p.MaxMagic += 3;
            }
            else
            {
                p.MaxMagic += 2;
            }
            p.magic = p.MaxMagic;
        }
    }

    /// <summary>
    /// 转化节点
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pc"></param>
    public void pointTrans(int id, PointColor pc)
    {
        Point p = mPoint[id];
        if (skillPoint >= 3 && p.color == PointColor.black && p.MaxMagic >= 5)
        {
            p.color = pc;
            skillPoint -= 3;
        }
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

    public void addMonster(Monster m)
    {
        mMonster.Add(m);
    }

    public void setFlag(ClickFlag c)
    {
        cf = c;
    }
}






