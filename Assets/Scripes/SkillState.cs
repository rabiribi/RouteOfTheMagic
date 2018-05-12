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
    public TextAsset Skilltext;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void show()
    {
        string[] lines = Skilltext.text.Split("\n"[0]);

        for (int i = 0; i < lines.Length; ++i)
        {
            string[] parts = lines[i].Split(" "[0]);
            if (parts[0] == child.GetComponent<Text>().text)
            {
                panel.GetComponentInChildren<Text>().text = parts[1];
                break;
            }
        }
    }
}
