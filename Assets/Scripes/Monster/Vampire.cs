using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RouteOfTheMagic
{
    [RequireComponent(typeof(Monster))]
    public class Vampire : MonoBehaviour
    {
        Monster monster;
        bool StageOne;
        bool StageTwo;
        // Use this for initialization
        void Start()
        {
            monster = GetComponent<Monster>();
            StageOne = true;
            StageTwo = true;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void attackTurn()
        {
            
            if (monster.monsterHP <= 0.7f * monster.maxMonsterHP && monster.monsterHP > 0.3f * monster.maxMonsterHP && StageOne)
            {
                //加buff特效
                monster.addBuff(4, 0, 0, 1, 10, 2);
                StageOne = false;
            }
            if (monster.monsterHP <= 0.3f * monster.maxMonsterHP && StageTwo)
            {
                //加buff特效
                monster.addBuff(4, 0, 0, 1, 10, 2);
                StageTwo = false;
            }
            //攻击特效
            monster.attackPlayer(Monster.AttackType.Random);
            //回血特效
            monster.restoreMonsterHP(monster.attackValue);          //***********获取最终伤害值并以一定比例回复
        }

    }
}
