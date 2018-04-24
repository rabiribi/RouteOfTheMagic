
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RouteOfTheMagic
{
    /// <summary>
    /// 场景类型，我的节点跳转到什么类型的场景
    /// </summary>
    public enum NodeType
    {
        fight = 0,
    }
    /// <summary>
    /// 地图上的节点结构体
    /// </summary>
    public class MapNode
    {
        /// <summary>
        /// 上层节点通过后才可以点击触发
        /// </summary>
        public bool fatherIsPass = false;
        /// <summary>
        /// 场景类型
        /// </summary>
        public NodeType nodeType = NodeType.fight;
        /// <summary>
        /// 自身的层数
        /// </summary>
        public int layer = 0;
        /// <summary>
        /// 该节点连接的子节点，队列形式
        /// </summary>
        public List<int> child = new List<int>();
    }

    /// <summary>
    /// 主地图生成绘制类
    /// </summary>
    public class MapMain : MonoBehaviour
    {
        //public List<List<MapNode>> map = new List<List<MapNode>>();
        public List<List<MapNode>> map = new List<List<MapNode>>();
        public UiRender render;
        public int layerCount = 5;
        // Use this for initialization
        void Start()
        {
            Init();
            int length = 500 / layerCount;
            for (int i = 0; i < map.Count; i++)
            {
                Debug.Log("层");
                for (int j = 0; j < map[i].Count; j++)
                {
                    Debug.Log("节点");
                    int layerXNum = map[i].Count*-50+50;
                    int layerYNum = 0;
                    if (i == map.Count - 1)
                        layerYNum = 0;
                    else
                        layerYNum = map[i+1].Count *-50+50;

                    for (int m = 0; m < map[i][j].child.Count; m++)
                    {
                        Debug.Log(map[i][j].child[m]);
                        int num = map[i][j].child[m];
                        mapLine li = new mapLine();
                        li.x = new Vector2(layerXNum + 100*j, -250+ length*i);
                        //if(i==map.Count-1)
                        //li.y = new Vector2(0, -260 + length * (i+1));
                        //else
                        li.y = new Vector2(layerYNum + 100 * num, -260 + length * (i + 1));
                        render.addLine(li);

                    }
                }
            }
            
        }
        void Init()
        {
            List<List<bool>> mark = new List<List<bool>>();
            for (int i = 0; i < layerCount; i++)
            {

                List<MapNode> floor = new List<MapNode>();
                List<bool> floorMark = new List<bool>();
                int num = Random.Range(2, 5);
                for (int j = 0; j < num; j++)
                {
                    MapNode node = new MapNode();
                    node.layer = i;
                    floor.Add(node);
                    floorMark.Add(false);
                }
                map.Add(floor);
                mark.Add(floorMark);
            }
            for (int i = 0; i < layerCount - 1; i++)
            {
                List<MapNode> floor = map[i];
                int childNum = map[i + 1].Count;
                List<bool> floorMark = mark[i + 1];
                float stand = childNum / (floor.Count + 0.0f);
                for (int j = 0; j < floor.Count; j++)
                {
                    MapNode node = new MapNode();
                    if (j == 0)
                    {
                        floor[j].child.Add(0);
                        floorMark[0] = true;
                        if (Random.Range(0.0f, 1.0f) >= 0.5)
                        {
                            floor[j].child.Add(1);
                            floorMark[1] = true;
                        }
                    }
                    else
                    {
                        if(j< childNum-1)
                        {
                            if (Random.Range(0.0f, 1.0f) >= 0.5)
                            {
                                floor[j].child.Add(j);
                                floorMark[j] = true;
                            }
                            if (Random.Range(0.0f, 1.0f) >= 0.5&& j < childNum - 2)
                            {
                                floor[j].child.Add(j + 1);
                                floorMark[j + 1] = true;
                            }
                            if (!floorMark[j])
                            {
                                if (floor[j].child.Count == 0)
                                {
                                    floor[j].child.Add(j);
                                    floorMark[j] = true;
                                }
                                else if (Random.Range(0.0f, 1.0f) >= 0.5)
                                {
                                    floor[j].child.Add(j - 1);
                                    floorMark[j - 1] = true;
                                }
                            }
                            else
                            {
                                if (floor[j].child.Count == 0)
                                {

                                    floor[j].child.Add(j);
                                    floorMark[j] = true;
                                }
                            }
                        }
                        else if(floorMark[childNum - 1])
                        {
                            floor[j].child.Add(childNum - 1);
                            floorMark[childNum - 1] = true;
                        }
                        else
                        {
                            if (Random.Range(0.0f, 1.0f) >= 0.5)
                            {
                                floor[j].child.Add(childNum - 2);
                                floorMark[childNum - 2] = true;
                                if (Random.Range(0.0f, 1.0f) >= 0.5)
                                {
                                    floor[j].child.Add(childNum - 1);
                                    floorMark[childNum - 1] = true;
                                }
                            }
                            else
                            {
                                floor[j].child.Add(childNum - 1);
                                floorMark[childNum - 1] = true;
                            }
                            if(j== floor.Count - 1)
                            {
                                floor[floor.Count - 1].child.Add(childNum - 1);
                                floorMark[childNum - 1] = true;
                            }
                                

                        }
                        for (int m = 0; m < childNum; m++)
                        {
                            if (!floorMark[m])
                            {
                                if (m >= floor.Count)
                                    floor[floor.Count - 1].child.Add(m);
                                else
                                    floor[m].child.Add(m);
                            }
                        }
                    }


                }
            }
            //指向BOSS
            List<MapNode> floorTop = map[layerCount - 1];
            for (int i = 0; i < floorTop.Count; i++)
            {
                floorTop[i].child.Add(0);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }


}
