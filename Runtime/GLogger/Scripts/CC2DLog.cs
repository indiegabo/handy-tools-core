using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The FSM GLogger. 
/// </summary>
public class CC2DLog : GLogger
{
    private static CC2DLog _log;

    /// <summary>
    /// Instantiate a new GLog if it does not already exists.
    /// Returns the GLog instance to be accessed statically.
    /// </summary>    
    public static CC2DLog I
    {
        get
        {
            if (!m_logger) { FindOrInstantiate<CC2DLog>(); }

            _log = m_logger.GetComponent<CC2DLog>();
            if (!_log) { m_logger.AddComponent<CC2DLog>(); }
            return _log;
        }
    }

    protected virtual void Awake()
    {
        // If the GLog is beeing instantiated manually, 
        // Defining itself as the instance. Kinda like a Singleton.
        _log = this;
    }

    /// <summary>
    /// The GLoggers default Log message
    /// </summary>
    /// <param name="message"> The message to be logged </param>
    /// <param name="sender"> Optional: The object the log message is comming from </param>
    public static void Log(string message, Object sender = null) => I.DoLog("CharacterController2D: " + message, I.whiteHEX, sender: sender);

    /// <summary>
    /// The success colored Log message
    /// </summary>
    /// <param name="message"> The message to be logged </param>
    /// <param name="sender"> Optional: The object the log message is comming from </param>
    public static void Success(string message, Object sender = null) => I.DoLog("CharacterController2D: " + message, I.successHEX, sender);

    /// <summary>
    /// The warning colored Log message
    /// </summary>
    /// <param name="message"> The message to be logged </param>
    /// <param name="sender"> Optional: The object the log message is comming from </param>
    public static void Warning(string message, Object sender = null) => I.DoLogWarning("CharacterController2D: " + message, I.warningHEX, sender);

    /// <summary>
    /// The danger colored Log message
    /// </summary>
    /// <param name="message"> The message to be logged </param>
    /// <param name="sender"> Optional: The object the log message is comming from </param>
    public static void Danger(string message, Object sender = null) => I.DoLogError("CharacterController2D: " + message, I.dangerHEX, sender);


    public static void Configure(bool shouldLog, Color success, Color warning, Color danger)
    {
        Configure(shouldLog);
        Configure(success, warning, danger);
    }

    public static void Configure()
    {
        I.m_shouldLog = true;
        I.m_successColor = Color.green;
        I.m_warningColor = Color.yellow;
        I.m_dangerColor = Color.red;
    }

    public static void Configure(bool shouldLog)
    {
        I.m_shouldLog = shouldLog;
    }

    public static void Configure(Color success, Color warning, Color danger)
    {
        if (success != null) I.m_successColor = success;
        if (warning != null) I.m_warningColor = warning;
        if (danger != null) I.m_dangerColor = danger;
    }
}
