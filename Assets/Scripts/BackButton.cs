using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : Button {

    MainButton openChapter;
    public Vector3 standartPosion;
    public Vector3 openPosition;
    public bool isActive = true;

    // Use this for initialization
    void Start () {
		
	}

    public void RemoteClick()
    {
        if (isActive)
        {
            switch (MainMenu.state)
            {
                case 1: { BackState1(); break; }
                case 2: { BackState2(); break; }
            }
        }
    }

    public override void Click()
    {
        if (isActive)
        {
            switch (MainMenu.state)
            {
                case 1: { BackState1(); break; }
                case 2: { BackState2(); break; }
            }
            Sfx.Click();
        }
    }

    public void BackState1()
    {
        foreach (MainButton select in MainMenu.GetInstance().m_mainButtons)
        {
            if (select.select) select.State1Close();
        }
        MainMenu.GetInstance().SendMessage("UpdateTimer");
    }

    public void BackState2()
    {
        foreach (MainButton select in MainMenu.GetInstance().m_mainButtons)
        {
            if (select.isOpen) select.State2Close();
        }
        MainMenu.GetInstance().SendMessage("UpdateTimer");
    }


}
