using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RouteOfTheMagic
{
    [RequireComponent(typeof(Monster))]
    public class Minister : MonoBehaviour
    {
        Monster monster;
        int countNum;
        // Use this for initialization
        void Start()
        {
            monster = GetComponent<Monster>();
            countNum = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void attackTurn()
        {
            if(countNum %3 == 0)    //3回合使用一次全体缓回buff
            {
                SlowRecoveryBuff();
            }
            recoverySkill();
            monster.attackPlayer(Monster.AttackType.Random);
            countNum++;
            return;         //补写攻击返回攻击点与攻击力
        }

        public void recoverySkill()
        {
            //获取所有怪物的monster，查询血量最低的，然后回血
        }

        public void SlowRecoveryBuff()
        {
            //获取偶有怪物的monster,挂上2号缓回buff
        }
    }
}
