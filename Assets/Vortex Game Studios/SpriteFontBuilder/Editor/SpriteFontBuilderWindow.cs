using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

[System.Serializable]
public class SpriteFontBuilderWindow : EditorWindow {
	private GUIContent _iconMinus;

	private TextureImporter _spriteFontImporter;
	private Vector2 _scrollView = Vector2.zero;

	private SpriteFontComponent _toBeRemovedEntry = null;
	private string _path = "Assets/Vortex Game Studios/SpriteFontBuilder/";

	// Space Lander ==> !"#%'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
	// NEHE ==>         !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRTSUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
	// private string _fontOrder = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRTSUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

	private GameObject _setup;

	[MenuItem( "Window/Vortex Game Studios/Sprite Font Builder", false, 0 )]
	public static void ShowWindow() {
		EditorWindow spriteFontEditorWindow = EditorWindow.GetWindow( typeof( SpriteFontBuilderWindow ) );

		spriteFontEditorWindow.titleContent.text = "Sprite Font";
		spriteFontEditorWindow.titleContent.image = AssetDatabase.LoadAssetAtPath<Texture>( "Assets/Vortex Game Studios/SpriteFontBuilder/icon.png" );

		/*
		EditorWindow spriteFontEditorWindow = EditorWindow.GetWindow( typeof( SpriteFontBuilderWindow ) );
		PropertyInfo p = typeof( EditorWindow ).GetProperty( "cachedTitleContent", BindingFlags.Instance | BindingFlags.NonPublic );
		GUIContent spriteFontEditorContent = p.GetValue( spriteFontEditorWindow, null ) as GUIContent;

		spriteFontEditorContent.text = "Sprite Font";
		spriteFontEditorContent.image = AssetDatabase.LoadAssetAtPath<Texture>( "Assets/Vortex Game Studios/SpriteFontBuilder/icon.png" );
		spriteFontEditorContent.tooltip = "Sprite Font Builder";
		*/
	}

	// Use this for initialization
	void OnGUI() {
		GetPrefab();

		// Draw the scroll bar
		_scrollView = GUILayout.BeginScrollView( _scrollView, false, false );

		this.BeginWindows();

		this.EndWindows();

		// Get the minus button size
		foreach ( SpriteFontComponent font in _setup.GetComponents<SpriteFontComponent>() ) {
			// Draw the font component header
			//GUILayout.BeginVertical( "AS TextArea", GUILayout.Height( 5 ) );
			
			GUILayout.BeginVertical( EditorStyles.helpBox, GUILayout.Height( 5 ) );
			

			GUILayout.Space( 2 );
			GUILayout.BeginHorizontal();

			// Draw the foldout button with the font name
			font.enabled = EditorGUILayout.Foldout( font.enabled, font.name );

			// Draw the minus button
			if ( GUILayout.Button( _iconMinus, GUIStyle.none, GUILayout.Width( 20 ) ) ) {
				//Debug.Log( "destroy" );
				_toBeRemovedEntry = font;
			}
			GUILayout.EndHorizontal();

			// Draw the font component properties
			if ( font.enabled ) {
				// Font name
				font.name = EditorGUILayout.TextField( "Font Name", font.name );

				// Load the new Custom Font asset
				//font.font = EditorGUILayout.ObjectField( "Custom Font", font.font, typeof( Font ), false ) as Font;

				// Load the Sprite asset
				font.sprite = EditorGUILayout.ObjectField( "Sprite Font", font.sprite, typeof( Texture2D ), false ) as Texture2D;

				// Set the character order inside spritesheet
				EditorStyles.textField.wordWrap = true;
				GUILayout.Label( "Character Font Order" );
				font.characters = EditorGUILayout.TextArea( font.characters, GUILayout.Height( 60.0f ) );

				// Just show this button when the CSV file exist in the same sprite path
				if ( font.sprite != null ) {
					string path = GetPath( font.sprite, '.' );
					//Debug.Log( path );
					if ( AssetDatabase.LoadAssetAtPath<TextAsset>( path + "txt" ) ||
						 AssetDatabase.LoadAssetAtPath<TextAsset>( path + "fnt" ) ) {
						EditorGUILayout.HelpBox( "ShoeBox Created Sprite Sheet Found.", MessageType.Info );
					} else {
						EditorGUILayout.HelpBox( "Using User Created Sprite Sheet.", MessageType.Warning );
					}
				} else {
					EditorGUILayout.HelpBox( "No Sprite Selected.", MessageType.Error );
				}
			}

			// Draw the font component footer
			GUILayout.Space( 2 );
			GUILayout.EndVertical();
			GUILayout.Space( 2 );
		}
		
		// Add new font
		if ( GUILayout.Button( "Add New Sprite Font" ) ) {
			AddEntry();
		}

		// Build the font button
		GUI.backgroundColor = Color.green;
		if ( GUILayout.Button( "Build the Sprite Font...", GUILayout.Height( 50 ) ) ) {
			BuildFont();
		}

		GUI.backgroundColor = Color.white;

		GUILayout.Label( "Sprite Font Builder v.1.2.0", EditorStyles.miniBoldLabel );

		GUILayout.EndScrollView();

		if ( _toBeRemovedEntry != null ) {
			RemoveEntry();
		}
	}

	private void AddEntry() {
		SpriteFontComponent newFont = _setup.AddComponent<SpriteFontComponent>();
		newFont.name = "New Sprite Font";
	}

	private void RemoveEntry() {
		//MessageBox.show();

		GameObject.DestroyImmediate( _toBeRemovedEntry, true );
		_toBeRemovedEntry = null;
		this.Repaint();
	}

	private void SetPrefab() {
		
	}

	private void GetPrefab() {
		if ( _setup == null ) {
			_setup = AssetDatabase.LoadAssetAtPath<GameObject>( _path + "SpriteFontSetup.prefab" );

			if ( _setup == null ) {
				_setup = new GameObject( "SpriteFontSetup" );
				PrefabUtility.CreatePrefab( _path + "SpriteFontSetup.prefab", _setup );
				GameObject.DestroyImmediate( _setup );
				GetPrefab();
			}

			_iconMinus = new GUIContent( EditorGUIUtility.IconContent( "Toolbar Minus" ) );
		}
	}

	private string GetPath( Texture2D sprite, char separator = '/' ) {
		string path = "";
		string[] pathSplit = AssetDatabase.GetAssetPath( sprite ).Split( separator );
		for ( int x = 0; x < pathSplit.Length - 1; x++ ) {
			path += pathSplit[x] + separator.ToString();
		}

		return path;
	}

	private void BuildSprite( SpriteFontComponent font ) {
		string path = GetPath( font.sprite, '.' );
		string fnt;
		TextAsset fntIn = null;

		if ( AssetDatabase.LoadAssetAtPath<TextAsset>( path + "txt" ) ) {
			fntIn = AssetDatabase.LoadAssetAtPath<TextAsset>( path + "txt" );
		} else if ( AssetDatabase.LoadAssetAtPath<TextAsset>( path + "fnt" ) ) {
			fntIn = AssetDatabase.LoadAssetAtPath<TextAsset>( path + "fnt" );
		}

		if ( !fntIn ) {
			Debug.LogWarning( "Sprite Font Builder: " + font.name + " built using user created sprite sheet." );
			return;
		}

		fnt = fntIn.text;

		// Get the total of characters
		int totalChars = int.Parse( new Regex( "((chars count=)([0-9]*))" ).Match( fnt ).Value.Replace( "chars count=", "" ) );
		font.fontBase = int.Parse( new Regex( "((base=)([0-9]*))" ).Match( fnt ).Value.Replace( "base=", "" ) );
		font.lineSpacing = int.Parse( new Regex( "((lineHeight=)([0-9]*))" ).Match( fnt ).Value.Replace( "lineHeight=", "" ) );

		_spriteFontImporter = TextureImporter.GetAtPath( AssetDatabase.GetAssetPath( font.sprite ) ) as TextureImporter;

		// Reset the current sprite suff
		_spriteFontImporter.spriteImportMode = SpriteImportMode.Single;
		_spriteFontImporter.spritesheet = null;
		_spriteFontImporter.SaveAndReimport();

		// And create a new one!
		_spriteFontImporter.textureType = TextureImporterType.Sprite;
		_spriteFontImporter.spriteImportMode = SpriteImportMode.Multiple;
		SpriteMetaData[] tSheet = new SpriteMetaData[totalChars];
		font.vector = new SpriteFontVector[totalChars];
		Match chars = new Regex( "((char )(.*))" ).Match( fnt );

		font.characters = "";

		int x = 0;
		while ( chars.Success && x < totalChars ) {
			//string l = new Regex( "((letter=\")(.*)(\"))" ).Match( chars.Value ).Value.Replace( "letter=\"", "" );
			int charId = int.Parse( new Regex( "((id=)([0-9]*))" ).Match( chars.Value ).Value.Replace( "id=", "" ) );
			string l = System.Convert.ToChar( charId ).ToString();
			//l = l.Substring( 0, l.Length - 1 );
			font.characters += l;

			font.vector[x] = new SpriteFontVector();
			Rect rect = new Rect();

			rect.x = float.Parse( new Regex( "((x=)([0-9]*))" ).Match( chars.Value ).Value.Replace( "x=", "" ) );
			rect.y = float.Parse( new Regex( "((y=)([0-9]*))" ).Match( chars.Value ).Value.Replace( "y=", "" ) );
			rect.width = float.Parse( new Regex( "((width=)([0-9]*))" ).Match( chars.Value ).Value.Replace( "width=", "" ) );
			rect.height = float.Parse( new Regex( "((height=)([0-9]*))" ).Match( chars.Value ).Value.Replace( "height=", "" ) );

			rect.y = font.sprite.height - rect.y - rect.height;

			font.vector[x].x = (int)(rect.x);
			font.vector[x].y = (int)(rect.y);
			font.vector[x].width = (int)(rect.width);
			font.vector[x].height = (int)(rect.height);
			font.vector[x].xOffset = int.Parse( new Regex( "((xoffset=)(-?[0-9]*))" ).Match( chars.Value ).Value.Replace( "xoffset=", "" ) );
			font.vector[x].yOffset = int.Parse( new Regex( "((yoffset=)(-?[0-9]*))" ).Match( chars.Value ).Value.Replace( "yoffset=", "" ) );
			font.vector[x].xAdvance = int.Parse( new Regex( "((xadvance=)(-?[0-9]*))" ).Match( chars.Value ).Value.Replace( "xadvance=", "" ) );

			SpriteMetaData tSpriteData = new SpriteMetaData();
			tSpriteData.name = font.name + "_" + x;
			tSpriteData.pivot = Vector2.zero;

			tSpriteData.rect = rect;
			
			tSheet[x] = tSpriteData;

			chars = chars.NextMatch();
			x++;
		}

		//	kernings
		/*
		font.kernings = new KerningFontVector[ int.Parse( new Regex( "((kernings count=)([0-9]*))" ).Match( fnt ).Value.Replace( "kernings count=", "" ) ) ];
		Match kernings = new Regex( "((kerning )(.*))" ).Match( fnt );
		x = 0;
		while ( kernings.Success && x < font.kernings.Length ) {
			font.kernings[ x ] = new KerningFontVector();
			font.kernings[ x ].first = int.Parse( new Regex( "((first=)(-?[0-9]*))" ).Match( kernings.Value ).Value.Replace( "first=", "" ) );
			font.kernings[ x ].second = int.Parse( new Regex( "((second=)(-?[0-9]*))" ).Match( kernings.Value ).Value.Replace( "second=", "" ) );
			font.kernings[ x ].amount = int.Parse( new Regex( "((amount=)(-?[0-9]*))" ).Match( kernings.Value ).Value.Replace( "amount=", "" ) );

			kernings.NextMatch();
			x++;
		}
		*/

		// Update and save the new sprite stuff
		_spriteFontImporter.spritesheet = tSheet;
		_spriteFontImporter.SaveAndReimport();
	}

	private void BuildFont() {
		foreach ( SpriteFontComponent font in _setup.GetComponents<SpriteFontComponent>() ) {
			// Check if we have a sprite
			if ( font.sprite == null ) {
				Debug.LogError( "Sprite Font Builder: " + font.name + " didn't have a sprite set." );
				continue;
			}

			// Get the the spritesheet path
			string path = GetPath( font.sprite );

			// Create the new font when need
			if ( font.font == null ) {							
				// Create the font
				font.font = new Font();
				AssetDatabase.CreateAsset( font.font, path + font.name + ".fontsettings" );
			}

			// Create the font material
			if ( font.font.material == null ) {
				font.font.material = new Material( Shader.Find( "UI/Default" ) );
				font.font.material.SetTexture( "_MainTex", font.sprite );
				AssetDatabase.CreateAsset( font.font.material, path + font.name + ".mat" );
			}

			// Try to build the spritesheet using a external file from the ShoeBox
			BuildSprite( font );

			// Get every sprite frames
			_spriteFontImporter = TextureImporter.GetAtPath( AssetDatabase.GetAssetPath( font.sprite ) ) as TextureImporter;

			// Basic font informations
			SerializedObject mFont = new SerializedObject( font.font );
			mFont.FindProperty( "m_CharacterPadding" ).intValue = 1;
			mFont.FindProperty( "m_FontSize" ).floatValue = font.fontSize;
			mFont.FindProperty( "m_LineSpacing" ).floatValue = font.lineSpacing;

			// Clear font character data
			mFont.FindProperty( "m_CharacterRects" ).ClearArray();

			// Build ou update the font
			for ( int x = 0; x < font.characters.Length; x++ ) {
				SpriteMetaData fontSprite = _spriteFontImporter.spritesheet[ x ];

				mFont.FindProperty( "m_CharacterRects" ).InsertArrayElementAtIndex( x );
				mFont.FindProperty( "m_CharacterRects" ).GetArrayElementAtIndex( x ).FindPropertyRelative( "index" ).intValue = (int)( font.characters[ x ] );
				mFont.FindProperty( "m_CharacterRects" ).GetArrayElementAtIndex( x ).FindPropertyRelative( "advance" ).floatValue = fontSprite.rect.width;

				if ( font.vector == null || font.vector.Length == 0 || font.vector[ x ] == null ) {
					mFont.FindProperty( "m_CharacterRects" ).GetArrayElementAtIndex( x ).FindPropertyRelative( "vert" )
						.rectValue = new Rect( 0, 
											   fontSprite.rect.height / 2.0f,
											   fontSprite.rect.width, 
											   -fontSprite.rect.height );
				} else {
					mFont.FindProperty( "m_CharacterRects" ).GetArrayElementAtIndex( x ).FindPropertyRelative( "advance" ).floatValue = font.vector[ x ].xAdvance + font.vector[ x ].xOffset;
					mFont.FindProperty( "m_CharacterRects" ).GetArrayElementAtIndex( x ).FindPropertyRelative( "vert" )
						.rectValue = new Rect( 0, 
											   font.fontBase - font.vector[ x ].yOffset,
											   font.vector[ x ].width, 
											   -font.vector[ x ].height );
				}

				mFont.FindProperty( "m_CharacterRects" ).GetArrayElementAtIndex( x ).FindPropertyRelative( "uv" )
					.rectValue = new Rect( fontSprite.rect.x / font.sprite.width,
										   fontSprite.rect.y / font.sprite.height,
										   fontSprite.rect.width / font.sprite.width,
										   fontSprite.rect.height / font.sprite.height );
			}

			/*
			if ( font.kernings != null ) {
				mFont.FindProperty( "m_KerningValues" ).ClearArray();

				for ( int x = 0; x < font.kernings.Length; x++ ) {
					mFont.FindProperty( "m_KerningValues" ).InsertArrayElementAtIndex( x );
					mFont.FindProperty( "m_KerningValues" ).GetArrayElementAtIndex( x ).FindPropertyRelative( "first" )
					mFont.FindProperty( "m_KerningValues" ).GetArrayElementAtIndex( x ).FindPropertyRelative( "second" )
					mFont.FindProperty( "m_KerningValues" ).GetArrayElementAtIndex( x ).FindPropertyRelative( "amount" )
				}
			}
			*/

			mFont.ApplyModifiedProperties();
			mFont.Update();

			EditorUtility.SetDirty( font );
			EditorUtility.SetDirty( font.sprite );
			EditorUtility.SetDirty( font.font );
			EditorUtility.SetDirty( font.font.material );

			_spriteFontImporter.SaveAndReimport();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
		}
	}
}