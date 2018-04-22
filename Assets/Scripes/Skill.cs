using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using scriptStructs;

public class SkillTool {
    MagicCore magicCore;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    List<Skill> skillList;

    SkillTool()
    { 
        Skill s = new Skill(0, SkillName.魔力放射, new List<PointColor>(),new List<int>(), SkillType.singleE, 1, 0);
        skillList.Add(s);

        s = new Skill(1, SkillName.魔法飞弹, new List<PointColor> { PointColor.white, PointColor.white }, new List<int> { 1, 1 }, SkillType.singleE, 1.5f, 0);
        skillList.Add(s);

        s = new Skill(2, SkillName.火球术, new List<PointColor> { PointColor.red, PointColor.white }, new List<int> { 1, 1 }, SkillType.singleE, 2.5f, 0);
        skillList.Add(s);

        s = new Skill(3, SkillName.火焰风暴, new List<PointColor> { PointColor.red, PointColor.white,PointColor.yellow}, new List<int> { 2, 1, 1 }, SkillType.allE, 3.0f, 0);
        skillList.Add(s);

        s = new Skill(4, SkillName.爆裂魔弹, new List<PointColor> { PointColor.red, PointColor.red, PointColor.white , PointColor.red, PointColor.red }, new List<int> { 2, 1, 1, 1 ,2 }, SkillType.singleE, 6.0f, 0);
        skillList.Add(s);

        s = new Skill(5, SkillName.火焰路径, new List<PointColor> { PointColor.red }, new List<int> { 2, 1 }, SkillType.singleE, 5.0f, 0);
        s.skillDo += sS5;

        s = new Skill(6, SkillName.一触即发, new List<PointColor> { PointColor.red, PointColor.white}, new List<int> { 2, 1 }, SkillType.singleE, 5.0f, 0);
        s.skillDo += beS6;
        skillList.Add(s);

        s = new Skill(7, SkillName.超频, new List<PointColor> { PointColor.red, PointColor.white }, new List<int> { 1, 1 }, SkillType.singleE, 3.0f, 0); 
        //加事件
        skillList.Add(s);

        s = new Skill(8, SkillName.燃烧潜能, new List<PointColor> { PointColor.red, PointColor.red,PointColor.red }, new List<int> { 1, 1,1 }, SkillType.self, 0.0f, 0);
        //加事件
        skillList.Add(s);

        s = new Skill(9, SkillName.愤怒, new List<PointColor> { PointColor.red, PointColor.red }, new List<int> { 1, 1 }, SkillType.singleE, 1.0f, 0);
        //加事件
        skillList.Add(s);

        s = new Skill(10, SkillName.炽热之血, new List<PointColor> { PointColor.red, PointColor.white,PointColor.blue }, new List<int> { 1, 1, 1 }, SkillType.self, 0.0f, 0);
        //加事件
        skillList.Add(s);

        s = new Skill(11, SkillName.融甲术, new List<PointColor> { PointColor.red, PointColor.white, PointColor.red }, new List<int> { 1, 1, 1 }, SkillType.singleE, 0.0f, 0);
        //加事件
        skillList.Add(s);
    }

    Skill getSkill(SkillName sk)
    {
        int id = (int)sk;
        return skillList[id];
    }

    void sS5( ref Magic magic)
    {
        List<Move> route = magicCore.getRoute();
        List<int> magicRoute = magic.magicRoute;
        List<Point> point = magicCore.getPoint();
        //计算防御值
        float basic = 0;
        for (int i = 0; i < magicRoute.Count; i++)
        {
            basic += point[magicRoute[i]].magic;
        }
        //添加防御路径
        for (int i = 0; i < route.Count; i++)
        {
            magicCore.addLineDefence(route[i].moveLine, basic * 3.0f);
        }

        //添加火焰反伤状态
        Buff buff = new Buff(BuffName.火焰路径, 1, BuffType.lBuffDefence,-1);
        buff.DFE += BuffTool.FireBramble;
        magicCore.addBuff(buff, -1);
    }

    void beS6(ref Magic magic)
    {
        //获取当前战斗盘对象
        int t = magicCore.getTurn();
        //如果当前回合数不为1，则将power变为1.5;
        if (t != 1)
        {
            magic.skill.power = 1.5f;
        }
    }
}

class BuffTool
{
    public static void FireBramble(int SorceID, int p, int l)
    {
        //给予伤害来源伤害
        //如果伤害来源HP死了,就回复生命值
    }
}