using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RouteOfTheMagic;

public class SkillTool {

    public MagicCore magicCore;
    public CharactorBuffTool buffTool;

    List<Skill> skillList;

    public SkillTool()
    {

        buffTool = new CharactorBuffTool();

        skillList = new List<Skill>();

        Skill s = new Skill(0, SkillName.魔法飞弹, new List<PointColor> { PointColor.white, PointColor.white }, new List<int> { 0, 0 }, SkillType.singleE, SkillDoType.oneWay, 1.5f, 0,1);
        skillList.Add(s);

        s = new Skill(1, SkillName.火球术, new List<PointColor> { PointColor.red, PointColor.white }, new List<int> { 0, 0 }, SkillType.singleE, SkillDoType.twoWay, 2.5f, 0,1);
        skillList.Add(s);

        s = new Skill(2, SkillName.火焰风暴, new List<PointColor> { PointColor.red, PointColor.white, PointColor.yellow }, new List<int> { 0, 0, 0 }, SkillType.allE, SkillDoType.twoWay, 1.0f, 0, 3);
        skillList.Add(s);

        s = new Skill(3, SkillName.火焰缠绕, new List<PointColor> { PointColor.red }, new List<int> { 1 }, SkillType.self, SkillDoType.single, 0.0f, 0,0);
        s.skillDo += sS5;
        skillList.Add(s); 

        s = new Skill(4, SkillName.一触即发, new List<PointColor> { PointColor.red, PointColor.white }, new List<int> { 0 , 0 }, SkillType.singleE, SkillDoType.oneWay, 5.0f, 0,1);
        s.skillDo += beS6;
        skillList.Add(s);

        s = new Skill(5, SkillName.超频, new List<PointColor> { PointColor.red, PointColor.white }, new List<int> { 0, 0 }, SkillType.singleE, SkillDoType.twoWay, 4.0f, 0,1);
        s.skillDo += sS7;
        skillList.Add(s);

        s = new Skill(6, SkillName.燃烧潜能, new List<PointColor> { PointColor.red, PointColor.white, PointColor.red }, new List<int> { 0, 1, 0 }, SkillType.self, SkillDoType.oneWay, 0.0f, 0,0);
        s.skillDo += sS8;
        skillList.Add(s);

        s = new Skill(7, SkillName.愤怒, new List<PointColor> { PointColor.red, PointColor.red }, new List<int> { 0, 0 }, SkillType.singleE, SkillDoType.oneWay, 1.0f, 0,1);
        s.beforeDo += beS9;
        skillList.Add(s);

        s = new Skill(8, SkillName.炽热之血, new List<PointColor> { PointColor.red, PointColor.white, PointColor.blue }, new List<int> { 0, 0, 0 }, SkillType.self, SkillDoType.unorder, 0.0f, 0,0);
        s.skillDo += sS10;//加事件
        skillList.Add(s);

        s = new Skill(9, SkillName.融甲术, new List<PointColor> { PointColor.red, PointColor.white, PointColor.red }, new List<int> { 1, 1, 1 }, SkillType.singleE, SkillDoType.oneWay, 0.0f, 0,1);
        //加事件
        skillList.Add(s);

        s = new Skill(10, SkillName.电光火石, new List<PointColor> { PointColor.red, PointColor.yellow }, new List<int> { 0, 0 }, SkillType.singleE, SkillDoType.twoWay, 2.0f, 0,1);
        s.skillDo += quickAttack;
        skillList.Add(s);

        foreach (Skill skill in skillList)
        {
            skill.skillDo += doDamage;
            skill.beforeDo += doNull;
        }
    }

    Skill getSkill(SkillName sk)
    {
        int id = (int)sk;
        return skillList[id];
    }

    /// <summary>
    /// 火焰缠绕：附加三回合的反伤buff，如果怪物死亡，回复节点魔力
    /// </summary>
    /// <param name="magic"></param>
    void sS5(ref Magic magic)
    {
        List<Move> mainRoute = magicCore.getRoute();
        List<int> magicRoute = magic.magicRoute;
        List<Point> point = magicCore.getPoint();
        //计算防御值
        float basic = 0;
        for (int i = magicRoute[0]; i <= magicRoute[1]; ++i)
        {
            basic += point[mainRoute[i].pEnd].magic;
        }
        //添加防御路径
        magicCore.addBuff(buffTool.getBuff(BuffName.火焰路径), -1);
    }

    /// <summary>
    /// 一触即发释放前：当前回合数不为1时，倍率变为1.5倍
    /// </summary>
    /// <param name="magic"></param>
    void beS6(ref Magic magic)
    {
        //获取当前战斗盘对象
        int t = magicCore.getTurn();
        //如果当前回合数不为0，则将power变为1.5;
        if (t == 1)
        {
            magic.skill.power = 5.0f;
        }
        else
        {
            magic.skill.power = 1.0f;
        }
    }

    /// <summary>
    /// 超频：下回合开始时攻击力减1
    /// </summary>
    /// <param name="magic"></param>
    void sS7(ref Magic magic)//超频：添加buff：下回合开始时攻击力-1
    {
        magicCore.addBuff(buffTool.getBuff(BuffName.ATK下降), -1);
    }

    /// <summary>
    /// 燃烧潜能：回合开始时，hp-3.atk+3
    /// </summary>
    /// <param name="magic"></param>
    void sS8(ref Magic magic)
    {
        magicCore.addBuff(buffTool.getBuff(BuffName.燃烧潜能), -1);
    }

    /// <summary>
    /// 愤怒:伤害倍率随着生命值降低升高
    /// </summary>
    /// <param name="magic"></param>
    void beS9(ref Magic magic)
    {
        magic.skill.count = (int)((float)magicCore.getHP() / (float)magicCore.getMaxHP() * 10.0f);
    }

    /// <summary>
    /// 炽热之血：下次技能如果击杀敌人，回复伤害数值
    /// </summary>
    /// <param name="magic"></param>
    void sS10(ref Magic magic)
    {
        magicCore.addBuff(buffTool.getBuff(BuffName.炽热之血), -1);
    }

    void quickAttack(ref Magic magic)
    {
        magicCore.setATK(magicCore.getATK() + 2);
    }

    public List<Skill> getInitSkills()
    {
        List<Skill> skill = new List<Skill>();
        skill.Add(skillList[8]); //加一个魔法飞弹
        skill.Add(skillList[10]);
        return skill;
    }

    public Skill getSkill(int id)
    {
        return skillList[id];
    }

    void doDamage(ref Magic magic)
    {
        List<Point> point = magicCore.getPoint();
        List<Move> route = magicCore.getRoute();
        //统计伤害值
        int pStart = magic.magicRoute[0];
        int pEnd = magic.magicRoute[1];

        int atk = 0;
        for (int i = pStart; i <= pEnd; ++i)
        {
            atk += point[route[i].pEnd].MaxMagic;
        }

        atk = (int)(atk * magic.skill.power) + (int)magic.skill.basic;

        magic.Damage = atk;

        if (magic.skill.skillType == SkillType.singleE)
            magicCore.doAttackToMonster(magic.target, magic.skill.count, (int)atk);
        else if (magic.skill.skillType == SkillType.allE)
        {
            magicCore.doAOEToMonster(magic.skill.count, (int)atk);
        }
        else if (magic.skill.skillType == SkillType.randomE)
        {
            magicCore.doRandomToMonster(magic.skill.count, (int)atk);
        }

    }

    void doNull(ref Magic m)
    {

    }
}