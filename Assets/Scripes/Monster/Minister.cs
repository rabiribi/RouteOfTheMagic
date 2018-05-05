using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RouteOfTheMagic
{
    [RequireComponent(typeof(Monster))]
    public class Minister : MonoBehaviour
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
            recoverySkill();
            monster.attackPlayer(Monster.AttackType.Random);
            return;         //补写攻击返回攻击点与攻击力
        }

        public void recoverySkill()
        {
            //获取所有怪物的monster，然后回血
        }
    }
}
