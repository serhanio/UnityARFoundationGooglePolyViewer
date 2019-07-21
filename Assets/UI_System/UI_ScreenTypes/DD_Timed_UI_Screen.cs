using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DigitalDreams.UI;

public class DD_Timed_UI_Screen : DD_UI_Screen
{
    #region variables
    [Header("Timed Screen Properties")]
    public float m_ScreenTime;
    public UnityEvent onTimeCompleted = new UnityEvent();

    private float startTime;
    #endregion

    #region Helper Methods
    public override void StartScreen()
    {
        base.StartScreen();

        startTime = Time.time;
        StartCoroutine(WairForTime());
    }

    IEnumerator WairForTime()
    {
        yield return new WaitForSeconds(m_ScreenTime);

        if(onTimeCompleted != null)
             onTimeCompleted.Invoke();
    }
    #endregion
}
