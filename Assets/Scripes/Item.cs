using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RouteOfTheMagic;


public class ItemTool  {

    public MagicCore magiccore;
    public Magic magic;
    Monster monster;
    List<ItemBuff> itemList;

    public ItemTool()
    {
        itemList = new List<ItemBuff>();
        magiccore = MagicCore.Instance;
        ItemBuff ib = new ItemBuff(ItemName.Alchemy, BuffType.sBuffMove, 5);
        ib.ME += alchemyE;//添加事件
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.Universalnode, BuffType.sBuffStart, 3);
        ib.NE += TheUniversalnode;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.HotGem, BuffType.sBuffMove, 3);
        ib.ME += HotGem;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.Pocketwatches, BuffType.sBuffStart, 3);
        ib.NE += Pocketwatches;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.Avalon, BuffType.sBuffMove, 1);
        ib.ME += Avalon;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.Shadowchains, BuffType.sBuffMove, 1);
        ib.ME += ShadowChains;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.Fourimagearray, BuffType.sBuffStart, -1);
        ib.NE += Fourimagearray;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.SageStone, BuffType.sBuffStart, -1);
        ib.NE += SageStone1;
        ib.ME += SageStone2;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.DoubleedgedStaff, BuffType.sBuffStart, -1);
        ib.NE += DoubleedgedStaff;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.FlameHeart, BuffType.sBuffMove, 3);
        ib.ME += FlameHeart;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.iceHeart, BuffType.sBuffMove, 3);
        ib.ME += IceHeart;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.ThunderHeart, BuffType.sBuffMove, 3);
        ib.ME += ThunderHeart;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.DeathEnd, BuffType.sBuffDamage, 1);
        ib.DE += DeathEnd1;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.DeathEnd2, BuffType.sBuffStart, 1);        
        ib.NE += DeathEnd2;
        itemList.Add(ib);

        ib = new ItemBuff(ItemName.BatterStaff, BuffType.sBuffSkill, 1);
        ib.NE += BatterStaff;
        itemList.Add(ib);

    }

    public ItemBuff getItem(ItemName iName)
    {
        return itemList[(int)iName];
    }

    public void alchemyE(Move m)
    {
        
        int dam;
        dam = magiccore.getUsedPCount()*3;
        monster.getDamage(dam);
        //造成节点数*3的伤害
    }//炼金阵

    public void TheUniversalnode() //万用节点
    {
        int pos;
        Point p;
        pos = magiccore.getPos();
        p = magiccore.getPoint(pos);
        p.magic++;
        magiccore.getPoint(pos).magic = p.magic;

    }

    public void HotGem(Move m)
    {
        magic.skill.power *= 2;
    }  //炽热宝石

    public void Pocketwatches()
    {
        int atk = magiccore.getATK();
        atk++;
        magiccore.setATK(atk);
    } //旅人的怀表

    public void Avalon(Move m)
    {
        int pos;
        Point p;
        pos = magiccore.getPos();
        p = magiccore.getPoint(pos);
        if (p.color == PointColor.blue)
        {
            int hp = magiccore.getHP();
            hp += 2;
            magiccore.setHP(hp);

        }

    }  //阿瓦隆

    public void ShadowChains(Move m) //暗影锁链
    {
        Point p;
        p = magiccore.getPoint(m.pEnd);
        if (p.color == PointColor.black)
        {
            int atk = magiccore.getATK();
            atk++;
            magiccore.setATK(atk);

        }
    }

    public void Fourimagearray()
    {
        int atk = magiccore.getATK();
        atk++;
        magiccore.setATK(atk);
        //释放两个技能后回合强制结束
    } //四圣阵

    public void SageStone1()
    {
        int atk = magiccore.getATK();
        atk++;
        magiccore.setATK(atk);
    }
    public void SageStone2(Move m)
    {
        int hp = magiccore.getHP();
        if (m.pStart != -1)
        {
            hp--;
            magiccore.setHP(hp);
        }
    }//贤者之石

    public void DoubleedgedStaff()
    {
        int atk = magiccore.getATK();
        atk++;
        magiccore.setATK(atk);
        int def = magiccore.getDEF();
        def--;
        magiccore.setDEF(def);
        
    } //双刃杖

    public void FlameHeart(Move m)
    {
        magic.skill.power *= 2;
        //下次技能伤害1.2倍化
    }//烈焰之心

    public void IceHeart(Move m)
    {
        magiccore.setDEF(magiccore.getDEF()+1);
    }//寒霜之心

    public void ThunderHeart(Move m)
    {
        magiccore.setATK(magiccore.getATK()+1);
    }//雷霆之心

    public void DeathEnd1(Damage dam)
    {
        int hp;
        int damage;
        int Maxhp;
        hp = magiccore.getHP();
        damage = dam.dam;

        Maxhp = magiccore.getMaxHP();
        if (hp-damage <= 0)
        {
            hp = (int)0.2 * Maxhp;
            magiccore.setHP(hp);
        }
        
    }
    public void DeathEnd2()
    {
        int atk = magiccore.getATK();
        atk+=5;
        magiccore.setATK(atk);
    } //即死领悟

    public void BatterStaff()
    {
        int rd;
        Point p;
        
        rd = Random.Range(0, 31);
        p = magiccore.getPoint(rd);
        p.magic++;
        
    }//连击法杖
}
