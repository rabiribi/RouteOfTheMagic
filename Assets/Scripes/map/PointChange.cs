using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RouteOfTheMagic;
public class PointChange : MonoBehaviour {
    public PointColor pc;
    SpriteRenderer spriteRenderer;
    NodeControl nc;
    private void Start()
    {
        nc = this.GetComponentInParent<NodeControl>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        spriteRenderer.color = toPointColor(pc);
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
    //鼠标抬起事件(基于碰撞体)
    void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            nc.PointChange(pc);
            Debug.Log(this.name);
        }
    }
}
