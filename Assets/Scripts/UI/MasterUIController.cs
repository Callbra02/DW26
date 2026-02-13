using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterUIController : MonoBehaviour
{
    public GameObject timerUIWidgetPrefab;
    public Transform timerUIHolder;
    public List<GameObject> timerWidgets { get; private set; }

    public List<GameObject> objectives;

    private int objectivesCompleted;
    
    void Start()
    {
        timerWidgets = new List<GameObject>();
        CreateWidget();

        for (int i = 0; i < objectives.Count; i++)
        {
            Image objImage = objectives[i].GetComponent<Image>();
            Color objColor = objImage.color;
            
            objImage.color = new Color(objColor.r, objColor.g, objColor.b, 0.25f);
        }

        objectivesCompleted = GameManager.Instance.objectivesCompleted;
    }

    void Update()
    {
        UpdateWidgets();
        
        objectivesCompleted = GameManager.Instance.objectivesCompleted;
        for (int i = 0; i < objectives.Count; i++)
        {
            if (i < objectivesCompleted)
            {
                Image objImage = objectives[i].GetComponent<Image>();
                Color objColor = objImage.color;
            
                objImage.color = new Color(objColor.r, objColor.g, objColor.b, 0.85f);
            }
        }
    }


    void CreateWidget()
    {
        GameObject newWidget = Instantiate(timerUIWidgetPrefab);
        newWidget.transform.position = timerUIHolder.position;
        newWidget.transform.SetParent(timerUIHolder);
        timerWidgets.Add(newWidget);
    }
    
    void UpdateWidgets()
    {
        if (timerWidgets.Count == 0)
            return;
        
        for (int i = 0; i < timerWidgets.Count; i++)
        {
            Slider widgetSlider =  timerWidgets[i].GetComponentInChildren<Slider>();
            
            widgetSlider.value =
                bUtils.NormalizeSliderValue(GameManager.Instance.gameTimer, 0, GameManager.Instance.maxGameTime);
        }
    }
}
