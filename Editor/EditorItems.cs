using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using IndieGabo.FSM;

namespace IndieGabo.Editor
{
    public class FSMLogEditor
    {
        [MenuItem("GameObject/GLogger/FSMLog")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject logger = new GameObject(typeof(FSMLog).Name);
            logger.AddComponent<FSMLog>();
            GameObjectUtility.SetParentAndAlign(logger, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(logger, "Create " + logger.name);
            Selection.activeObject = logger;
        }

    }

    public class CC2DLogEditor
    {

        [MenuItem("GameObject/GLogger/CC2DLog")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject logger = new GameObject(typeof(CC2DLog).Name);
            logger.AddComponent<CC2DLog>();
            GameObjectUtility.SetParentAndAlign(logger, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(logger, "Create " + logger.name);
            Selection.activeObject = logger;
        }

    }

    public class GLogEditor
    {

        [MenuItem("GameObject/GLogger/GLog")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject logger = new GameObject(typeof(GLog).Name);
            logger.AddComponent<GLog>();
            GameObjectUtility.SetParentAndAlign(logger, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(logger, "Create " + logger.name);
            Selection.activeObject = logger;
        }

    }

    public class FSMEditor
    {
        [MenuItem("GameObject/IndieGabo/FSM/State Machine")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject machineObject = new GameObject("StateMachine");
            machineObject.AddComponent<StateMachine>();
            GameObjectUtility.SetParentAndAlign(machineObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(machineObject, "Create " + machineObject.name);
            Selection.activeObject = machineObject;
        }
    }

}