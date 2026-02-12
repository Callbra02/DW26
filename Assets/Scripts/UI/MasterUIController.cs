using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterUIController : MonoBehaviour
{
    public GameObject timerUIWidgetPrefab;
    public Transform timerUIHolder;
    public List<GameObject> timerWidgets { get; private set; }
    
    
    void Start()
    {
        timerWidgets = new List<GameObject>();
        CreateWidget();
    }

    void Update()
    {
        UpdateWidgets();
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
