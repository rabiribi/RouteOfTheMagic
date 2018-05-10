using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RouteOfTheMagic
{
    [RequireComponent(typeof(Monster))]
    public class DoubleSwordMan : MonoBehaviour
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
            //攻击两次两个地方
            monster.attackPlayer(Monster.AttackType.Random);
            monster.attackPlayer(Monster.AttackType.Random);
            return;         //补写攻击返回攻击点与攻击力
        }


    }
}
