using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RouteOfTheMagic;


public class NodeControl : MonoBehaviour {

    MagicCore magic;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //magic = GameObject.Find("EventSystem").GetComponent<Control>().magic;
        //直接使用单例获取
        magic = MagicCore.Instance;
        //判断状态,确定节点是否显示，以及其魔力值等
        if (magic.getPoint(int.Parse(this.tag)).MaxMagic == 0)
        {
            this.GetComponent<SpriteRenderer>().sprite = null;
        }
        else if (!magic.getPointBroked(int.Parse(this.tag)))
        {
            this.GetComponentInChildren<TextMesh>().text =magic.getPoint(int.Parse(this.tag)).MaxMagic.ToString();
        }
        //节点颜色初始化
        this.GetComponent<SpriteRenderer>().color = toPointColor(magic.getPointColor(int.Parse(this.tag)));    
    }

    public Color toPointColor(PointColor pointC)
    {
        Color color = new Color();
        switch (pointC)
        {
            case PointColor.black:
                color = Color.black;
                break;
            case PointColor.blue:
                color = Color.blue;
                break;
            case PointColor.red:
                color = Color.red;
                break;
            case PointColor.white:
                color = Color.white;
                break;
            case PointColor.yellow:
                color = Color.yellow;
                break;
        }
        return color;
    }
    void OnMouseDown()
    {
       
    }
}
