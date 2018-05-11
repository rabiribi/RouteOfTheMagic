using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SkillState : MonoBehaviour {
    public GameObject panel;
    public GameObject child;
    string[] line;
    string str;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void show()
    {
        FileStream fs = new FileStream("..\\Skill.txt", FileMode.Open, FileAccess.Read);
        StreamReader sr = new StreamReader(fs,Encoding.Default);
        while ((str=sr.ReadLine())!=null)
        {
            line = str.Split(' ');
            if (line[0] == child.GetComponent<Text>().text)
            {
                panel.GetComponentInChildren<Text>().text = line[1];
                break;
            }
        }
    }
}
