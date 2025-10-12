using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChangeToPlatformer : MonoBehaviour
{
    // Start is called before the first frame update
    public int ClickGoal = 15;
    // public GameObject sceneSwitcher;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Slider>().value = Clickable.Clicks;
        gameObject.GetComponent<Slider>().maxValue = ClickGoal;
        if (Clickable.Clicks >= ClickGoal)
        {
            print('o');
            gameObject.GetComponent<UI_SwitchScene>().SwitchSceneTo("03_Platformer");
        }
    }
}
