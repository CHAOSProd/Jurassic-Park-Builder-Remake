using UnityEngine;
using UnityEditor;
using System.Collections;

public class VortexMenu : EditorWindow {
	[MenuItem( "Window/Vortex Game Studios/Our Assets", false, 1000 )]
	public static void ShowAssetStore() {
		Application.OpenURL( "https://goo.gl/E5JTf5" );
	}
	[MenuItem( "Window/Vortex Game Studios/Visit our Site", false, 1000 )]
	public static void ShowWebsite() {
		Application.OpenURL( "http://vortexstudios.com" );
	}
}
