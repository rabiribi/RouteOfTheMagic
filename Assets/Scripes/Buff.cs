using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RouteOfTheMagic;

public class CharactorBuffTool {
    List<Buff> buffs;
    public MagicCore magic;

    public CharactorBuffTool()
    {
        buffs = new List<Buff>();

        Buff buff = new Buff(BuffName.ATK下降,1,BuffType.sBuffTurn,-1,false);
        buff.NE += doATKDown;
        buffs.Add(buff);

        buff = new Buff(BuffName.火焰路径, 1, BuffType.sBuffDefence, -1,false);
        buff.DFE += doFireRoad;
        buffs.Add(buff);

        buff = new Buff(BuffName.燃烧潜能, 100, BuffType.sBuffTurn, -1,false);
        buff.NE += doPotency;
        buffs.Add(buff);

        buff = new Buff(BuffName.炽热之血, 1, BuffType.sBuffAttack, -1,true);
        buff.SE += hotBlod;
        buffs.Add(buff);

        buff = new Buff(BuffName.碎冰甲D, 1, BuffType.sBuffDefence, -1, false);
        buff.DFE += brokenIceArmorD;
        buffs.Add(buff);

        buff = new Buff(BuffName.碎冰甲S, 1, BuffType.sBuffDamage, -1, false);
        buff.DE += brokenIceArmorS;
        buffs.Add(buff);

        buff = new Buff(BuffName.恢复, 3, BuffType.sBuffTurn, -1, false);
        buff.NE += Recover;
        buffs.Add(buff);
        
    }

    public Buff getBuff(BuffName bName)
    {
        return buffs[(int)bName]; 
    }

    /// <summary>
    /// 回合开始时，ATK减少一点
    /// </summary>
    void doATKDown()
    {
        magic.setATK(magic.getATK() - 1);
    }

    /// <summary>
    /// 边格挡发生时，给与怪物防御值的伤害，如果怪物死亡，回复全部魔力
    /// </summary>
    /// <param name="sorce"></param>
    void doFireRoad(Defen defen)
    {
        //防御怪物攻击时，给与节点魔力5倍的伤害，如果怪物死亡，回复全部魔力
        if (defen.plID > 0 && defen.plID < magic.getPoint().Count)
        {
            magic.doAttackToMonster(defen.sorce, 1, 5 * magic.getPoint(defen.plID).MaxMagic);
            if (!magic.isMonsterLive(defen.sorce))
            {
                magic.getPoint(defen.plID).magic = magic.getPoint(defen.plID).MaxMagic;
            }
        }
    }

    /// <summary>
    /// 回合开始时，ATK + 3，HP - 3,永久发动
    /// </summary>
    void doPotency()
    {
        magic.setATK(magic.getATK() + 3);
        magic.setHP(magic.getHP() - 3);
    }

    /// <summary>
    /// 如果伤害值导致怪物死亡，回复伤害数值的HP
    /// </summary>
    void hotBlod(ref Magic m)
    {
        //技能伤害导致怪物死亡,回复生命值
        if (!magic.isMonsterLive(m.target))
        {
            magic.setHP(magic.getHP() + m.Damage);
        }
    }

    /// <summary>
    /// 碎冰甲的防御被动：消去敌人一条攻击力
    /// </summary>
    /// <param name="d"></param>
    void brokenIceArmorD(Defen d)
    {
        magic.delectMonsterATK(d.sorce);
    }

    /// <summary>
    /// 碎冰甲被击中时的被动：消去敌人一条攻击力
    /// </summary>
    /// <param name="d"></param>
    void brokenIceArmorS(Damage d)
    {
        magic.delectMonsterATK(d.dRasour);
    }

    void Recover()
    {
        magic.setHP(magic.getHP() + 5);
        if (magic.getHP() > magic.getMaxHP())
        {
            magic.setHP(magic.getMaxHP());
        }
    }
}
