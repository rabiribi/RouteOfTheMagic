using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RouteOfTheMagic
{
    [RequireComponent(typeof(Monster))]
    public class Boss_TurnMan : MonoBehaviour
    {
        Monster monster;
        // Use this for initialization
        void Start()
        {
            monster = GetComponent<Monster>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void attackTurn()
        {
            
            monster.attackPlayer(Monster.AttackType.Circle);
            turnCore();

            return;         //补写攻击返回攻击点与攻击力
        }

        /// <summary>
        /// Turns the core 当回合结束时调用
        /// </summary>
        void turnCore()
        {
            //call 魔法盘控制，封锁珠子
        }
    }
}
