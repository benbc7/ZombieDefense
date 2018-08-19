/************************************************
Created By:		Ben Cutler
Company:		Tetricom Studios
Product:
Date:
*************************************************/

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using UnityEditorInternal;
using UnityEditor.ProjectWindowCallback;

public class HotkeyEditor : EditorWindow {

	private enum WindowType {
		None, CSWindow
	}

	private static WindowType windowType = WindowType.None;

	private static string className = "";
	private static string selectedPath = "";

	private static string[] defaultCS;

	private static HotkeyEditor window;

	private static bool showNameError;

	[MenuItem ("Editor Tools/New C# Script %#c")]
	private static void NewCSMenuItem () {

		if (window != null) {
			window.Close ();
			window = null;
		}

		windowType = WindowType.CSWindow;
		string path;
		UnityEngine.Object obj = Selection.activeObject;
		if (obj == null) {
			path = "";
		} else {
			path = AssetDatabase.GetAssetPath (obj.GetInstanceID ());
		}

		if (path.Length > 0) {
			if (Directory.Exists (path)) {
				CSDialogeWindow (path);
			} else {
				CSDialogeWindow (Directory.GetParent (path).ToString ());
			}
		}
	}

	private static void CSDialogeWindow (string path) {
		selectedPath = path;
		window = CreateInstance<HotkeyEditor> ();
		window.position = new Rect (Screen.width / 2, Screen.height / 2, 250, 150);
		window.ShowPopup ();
		window.Focus ();
	}

	private static void CreatCSFile () {
		string newFilePath = selectedPath + "/" + className.Replace (" ", "_") + ".cs";
		if (!File.Exists (newFilePath)) {
			using (StreamWriter outFile = new StreamWriter (newFilePath)) {
				foreach (string line in defaultCS) {
					if (line.Contains ("#SCRIPTNAME#")) {
						outFile.WriteLine (line.Replace ("#SCRIPTNAME#", className.Replace (" ", "_")));
					} else {
						outFile.WriteLine (line);
					}
				}
			}
			AssetDatabase.Refresh ();
		}
	}

	private void SCFileGUI (Event guiEvent) {
		GUILayout.Label ("Class Name");
		className = GUILayout.TextField (className);
		if (GUILayout.Button ("Create Script (Return)") || guiEvent.isKey && guiEvent.keyCode == KeyCode.Return) {
			if (className.Length > 0) {
				className = char.ToUpper (className [0]) + className.Substring (1);
				CreatCSFile ();
				ResetSCWindow ();
			} else {
				showNameError = true;
			}
		}

		if (GUILayout.Button ("Cancel (ESC)") || guiEvent.isKey && guiEvent.keyCode == KeyCode.Escape) {
			ResetSCWindow ();
		}

		if (showNameError) {
			EditorGUILayout.HelpBox ("Class name cannot be blank", MessageType.Warning);
		}
	}

	private void ResetSCWindow () {
		showNameError = false;
		selectedPath = "";
		className = "";
		windowType = WindowType.None;
		Close ();
		window = null;
	}

	private void OnGUI () {
		Event guiEvent = Event.current;

		switch (windowType) {
			case WindowType.CSWindow: {
					SCFileGUI (guiEvent);
				}
				break;
		}
	}

	private void OnEnable () {
		string defaultCSPath = EditorApplication.applicationContentsPath + "/Resources/ScriptTemplates/81-C# Script-NewBehaviourScript.cs.txt";
		if (File.Exists (defaultCSPath)) {
			defaultCS = File.ReadAllLines (defaultCSPath);
		}
	}
}