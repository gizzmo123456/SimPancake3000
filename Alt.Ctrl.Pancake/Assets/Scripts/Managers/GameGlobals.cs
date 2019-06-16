using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Containes all data that is used globaly
/// and maybe the odd shortcut.
/// </summary>
public static class GameGlobals
{

	// 
	public static string name = "Alt.Ctrl.Pancake";
	public static int buildId = 0;

	// static store
	public static InputHandler inputs;

	// Constants
	public const int fryingpanCount = 3;	// aparently in c# if a var is a const it is also static, i did not know that :)

}
