using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RouteOfTheMagic
{
    /// <summary>
    /// 节点颜色
    /// </summary>
    public enum PointColor
    {
        black = 0,
        white = 1,
        red = 2,
        blue = 3,
        yellow = 4,
        count
    };

    /// <summary>
    /// 节点类型
    /// </summary>
    public enum PointType
    {
        normal = 0,
        transparent = 1,
        iron = 2,
        source = 3,
        count
    };

    public enum lineState
    {
        normal = 0, //黑色
        drag = 1,   //橙色
        light = 2,  //白色
        used = 3,   //红色
        count
    };

    public enum SkillName
    {
        魔力放射 = 0,
        魔法飞弹 = 1,
        火球术 = 2,
        火焰风暴 = 3,
        爆裂魔弹 = 4,
        火焰路径 = 5,
        一触即发 = 6,
        超频 = 7,
        燃烧潜能 = 8,
        愤怒 = 9,
        炽热之血 = 10,
        融甲术 = 11,
        count
    };

    public enum SkillDoType
    {
        oneWay = 0,
        twoWay = 1,
        unorder = 2,
        single = 3,
        norequire = 4,
        count
    };

    public enum BuffName
    {
        ATK下降 = 0,
        火焰路径 = 1,
        count
    }

    public enum ItemName
    {

    }

    public enum BuffType
    {
        pBuffMoveIn = 0,
        pBuffMoveOut = 1,
        pBuffSkill = 2,
        pBuffAttack = 3,
        pBuffDefence = 4,
        pBuffBroken = 5,

        lBuffPass = 6,
        lBuffDamage = 7,
        lBuffDefence = 8,

        sBuffStart = 9,
        sBuffTurn = 10,
        sBuffTurnEnd = 11,
        sBuffSkill = 12,
        sBuffDamage = 13,
        sBuffMove = 14,
   
        count

    }

    /// <summary>
    /// 单击鼠标左键时执行的操作类型
    /// </summary>
    public enum ClickFlag
    {
        normal = 0,                       //选择路径/选取技能
        defencer = 1,                     //选择防御节点
        target = 2,                       //选择施法对象
        upgrade = 4,                      //要升级节点的时候
        transToRed = 5,
        transToBlue = 6,
        transToYellow = 7,
        TransToWhite = 8,
        count
    };

    /// <summary>
    /// 技能类型
    /// </summary>
    public enum SkillType
    {
        singleE = 0,
        allE = 1,
        randomE = 2,
        selfP = 3,
        selfS = 4,
        self = 5,
        count
    }

    /// <summary>
    /// 节点对象的结构
    /// </summary>
    public class Point
    {
        public Point(int id,PointColor c,PointType t,int maxM,List<int> lineList)
        {
            locate = id;
            color = c;
            type = t;
            MaxMagic = maxM;
            magic = maxM;
            line = lineList;

            isUnpassable = false;
            isBroken = false;
            isActivity = false;
            isDefence = false;
        }

        //节点属性
        public int locate;            //节点位置编号
        public PointColor color;      //节点颜色
        public PointType type;        //节点类型
        public List<int> line;        //边列表
        public int MaxMagic;          //最大魔力值
        public int magic;             //魔力值

        //节点状态
        public bool isUnpassable;       //不能通过么
        public bool isBroken;           //坏了么
        public bool isActivity;         //位于激活状态么
        public bool isDefence;          //防御状态

        public List<BuffBasic> buff;
    };

    /// <summary>
    /// 路径对象的结构
    /// </summary>
    public class Line
    {
        public Line(int id,int i1, int i2)
        {
            roateID = id;
            p1 = i1;
            p2 = i2;

            isUnpassable = false;
            isPassed = false;
            def = 0;
        }

        public int roateID;
        public int p1, p2;

        //状态
        public bool isUnpassable;   //不能通过么
        public bool isPassed;     //这回合已经走过了么
        public int def;         //这个路径的护盾值

        public List<BuffBasic> buff;
    };

    /// <summary>
    /// 技能对象的结构
    /// </summary>
    public class Skill
    {
        public Skill(int i,SkillName sm,List<PointColor> lr,List<int> lrp,SkillType st,SkillDoType sdt,float p,float b)
        {
            id = i;
            name = sm;
            mRequire = lr;
            mRequireP = lrp;
            skillType = st;
            power = p;
            basic = b;
            skillDoType = sdt;

            useable = false;
            usedTime = 0;
            usedTimeTurn = 0;
        }

       
        public int id;                      //技能编号
        public SkillName name;              //技能名字
        public List<PointColor> mRequire;   //释放要求
        public List<int> mRequireP;         //释放要求对应的点数
        public SkillType skillType;         //技能类型
        public SkillDoType skillDoType;     //技能释放类型

        public float power;                 //伤害倍率
        public float basic;                 //基础伤害值

        public List<Move> subRoute;         //本次技能的执行路径

        public bool useable;                //可以使用么
        public int usedTime;                //本次战斗的使用次数
        public int usedTimeTurn;            //本回合的使用次数

        public SkillEvent beforeDo;         //释放前的操作
        public SkillEvent skillDo;           //释放技能时的效果
    }

    public class BuffBasic
    {
        public BuffType type;
        public int turn;
        public int count;
        public int maxCount;

        public NaiveEvent NE;
        public MoveEvent ME;
        public SkillEvent SE;
        public DamageEvent DE;
        public DefenceEvent DFE;
    }

    public class Buff:BuffBasic
    {

        public Buff(BuffName bn, int c, BuffType bt,int max)
        {
            name = bn;
            turn = c;
            type = bt;
            maxCount = max;
            count = 0;
        }

        public BuffName name;
      
    }

    public class ItemBuff:BuffBasic
    {
        public ItemBuff(ItemName it,BuffType bt,int max)
        {
            iName = it;
            type = bt;
            turn = -1;
            maxCount = max;
            count = 0;
        }

        public ItemName iName;
    }

    /// <summary>
    /// 移动事件
    /// </summary>
    public struct Move
    {
        public int pStart, pEnd;
        public int moveLine;
    };

    /// <summary>
    /// 释放技能事件
    /// </summary>
    public struct Magic
    {
        public List<int> magicRoute;  //释放法术时的执行路径
        public Skill skill;             //技能id
        public int target;            //法术的执行对象
    };

    /// <summary>
    /// 受到伤害事件
    /// </summary>
    public struct Damage
    {
        public int dam;                //单次攻击的伤害
        public int dRasour;            //伤害来源
    }

    public class EDamage
    {
        public int ID;
        public int damage;
        public List<int> sorce;
    }

   

    /// <summary>
    /// 移动时触发的事件
    /// </summary>
    /// <param name="move"></param>
    public delegate void MoveEvent(Move move);

    /// <summary>
    /// 受到伤害时会触发的事件
    /// </summary>
    /// <param name="damage"></param>
    public delegate void DamageEvent(Damage damage);

    /// <summary>
    /// 防御成功的事件
    /// </summary>
    /// <param name="SorceID"></param>
    /// <param name="Core"></param>
    public delegate void DefenceEvent(int SorceID,int p,int l);

    /// <summary>
    /// 释放技能的事件
    /// </summary>
    /// <param name="magic"></param>
    /// <param name="Core"></param>
    public delegate void SkillEvent(ref Magic magic);

    /// <summary>
    /// 不需要触发参数的事件
    /// </summary>
    public delegate void NaiveEvent();    
}