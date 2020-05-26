using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;



public class AnalyticsController : MonoBehaviour
{
    #region /* Custom Events */

    public const string RESTART       = "RestartLevel";
    public const string ATTEMP_SOLVED = "AttempLevelSolved";
    public const string PIECE_DELETED = "PieceDeleted";
    public const string PIECE_MOVED   = "PieceMoved";
    public const string PIECE_PLACED  = "PiecePlaced";

    #endregion


    #region /* Auxliar Names */

    private const string LEVEL_KEY = "Level";
    private const string PIECE_KEY = "Piece";
    
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

    
    public void LevelRestart(string name)
    {
        List<string> keys = new List<string> { LEVEL_KEY };
        List<object> info = new List<object> { name };

        this.Custum(RESTART, keys, info);
    }


    public void AttemptLevelSolved(string name)
    {
        List<string> keys = new List<string> { LEVEL_KEY };
        List<object> info = new List<object> { name };

        this.Custum(ATTEMP_SOLVED, keys, info);
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


    #region === Piece Info Methods ===

    public void DeletePiece(string level, string name)
    {
        List<string> keys = new List<string> { LEVEL_KEY, PIECE_KEY };
        List<object> info = new List<object> { level, name };

        this.Custum(RESTART, keys, info);
    }


    public void PieceMoved(string level, string name)
    {
        List<string> keys = new List<string> { LEVEL_KEY, PIECE_KEY };
        List<object> info = new List<object> { level, name };

        this.Custum(PIECE_MOVED, keys, info);
    }


    public void PiecePlaced(string level, string name)
    {
        List<string> keys = new List<string> { LEVEL_KEY, PIECE_KEY };
        List<object> info = new List<object> { level, name };

        this.Custum(PIECE_PLACED, keys, info);
    }

    #endregion


    #region === Custam Info Methods ===

    private void Custum(string Event, List<string> keys, List<object> info)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        for (int i = 0; i < keys.Count; i++)
        {
            data.Add(keys[i], info[i]);
        }

        Analytics.CustomEvent(Event, data);
    }

    #endregion
}
