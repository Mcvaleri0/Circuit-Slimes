using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;



public class AnalyticsController : MonoBehaviour
{
    #region /* Custom Events */

    public const string RESTART_CLICKED = "RestartButtonClicked";
    public const string PIECE_DELETED   = "PieceDeleted";
    public const string PIECE_LOCATION  = "PieceLocation";
    
    #endregion


    #region === General Info Methods ===

    public void PrintInformation()
    {
        Debug.Log("configUrl: " + Analytics.configUrl);
        Debug.Log("deviceStatsEnabled: " + Analytics.deviceStatsEnabled);
        Debug.Log("enabled: " + Analytics.enabled);
        Debug.Log("eventUrl: " + Analytics.eventUrl);
        Debug.Log("initializeOnStartup: " + Analytics.initializeOnStartup);
        Debug.Log("limitUserTracking: " + Analytics.limitUserTracking);
        Debug.Log("playerOptedOut: " + Analytics.playerOptedOut);
    }

    #endregion


    #region === Game Info Methods ===

    public void GameStart()
    {
        AnalyticsEvent.GameStart();
    }


    public void GameOver()
    {
        AnalyticsEvent.GameOver();
    }

    #endregion


    #region === Level Info Methods ===

    public void LevelStart(string name)
    {
        AnalyticsEvent.LevelStart(name);
    }


    public void LevelComplete(string name)
    {
        AnalyticsEvent.LevelComplete(name);
    }


    public void LevelQuit(string name)
    {
        AnalyticsEvent.LevelQuit(name);
    }


    public void LevelSkip(string name)
    {
        AnalyticsEvent.LevelSkip(name);
    }

    #endregion


    #region === Tutorial Info Methods ===
    
    public void TutoralStart(string name)
    {
        AnalyticsEvent.TutorialStart(name);
    }


    public void TutoralComplete(string name)
    {
        AnalyticsEvent.TutorialComplete(name);
    }


    public void TutoralSkip(string name)
    {
        AnalyticsEvent.TutorialSkip(name);
    }

    #endregion


    #region === Custom Events Info Methods ===
    
    public void Custom(string name)
    {
        AnalyticsEvent.Custom(name);
    }
    
    #endregion
}
