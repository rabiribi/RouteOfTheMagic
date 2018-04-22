using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using scriptStructs;

public class mouseevent : MonoBehaviour {
    // Use this for initialization
    MagicCore magic;
    List<Point> pList;
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        magic = GameObject.Find("EventSystem").GetComponent<Clickcontrol>().magic;
        pList = magic.getPoint();

        //判断状态
        if (magic.getPoint(int.Parse(this.tag)).MaxMagic == 0)
        {
            this.GetComponent<SpriteRenderer>().sprite=null;
        }
        else
        {
            this.GetComponentInChildren<TextMesh>().text =
            magic.getPoint(int.Parse(this.tag)).magic + "/" + magic.getPoint(int.Parse(this.tag)).MaxMagic;
        }

        if (pList[int.Parse(this.tag)].isActivity)
            this.GetComponent<SpriteRenderer>().color = Color.red;
        else
            this.GetComponent<SpriteRenderer>().color = Color.white;
       

        if (Input.GetMouseButton(1)&&!Clickcontrol.isDrag)
            magic.RclickP(-1);
    }
    void OnMouseOver()
    {
        if (Clickcontrol.isDrag)
            magic.drag(int.Parse(this.tag));
        if (Input.GetMouseButton(1))
            magic.RclickP(int.Parse(this.tag));
    }
    void OnMouseDown()
    {
        if (magic.getCurrentPos() == int.Parse(this.tag))
            Clickcontrol.isDrag = true;
    }
    void OnMouseUp()
    {
        Clickcontrol.isDrag = false;
        magic.dragLoose();
    }
    public  MagicCore getCore()
    {
        return magic;
    }
}
