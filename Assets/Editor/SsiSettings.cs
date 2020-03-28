using UnityEngine;
using UnityEditor;

namespace SsiEditor{
	public class SsiSettings : ScriptableObject {
		[SerializeField] private string m_sheetURL;
		[SerializeField] private string m_sheetName;
		[SerializeField] private string m_APIKey;

		public string SheetId {
			get => m_sheetURL;
			set => m_sheetURL = value;
		}

		public string SheetName {
			get => m_sheetName;
			set => m_sheetName = value;
		}

		public string ApiKey {
			get => m_APIKey;
			set => m_APIKey = value;
		}

		public static SsiSettings LoadInstance() {
			string path = "Assets/Editor/SsiSettings.asset";
			SsiSettings settings = AssetDatabase.LoadAssetAtPath<SsiSettings>(path);

			if (settings != null) return settings;

			settings = CreateInstance<SsiSettings>();
			AssetDatabase.CreateAsset(settings, path);
			AssetDatabase.SaveAssets();

			return settings;
		}
		
		public static SerializedObject GetSerializedObject() {
			return new SerializedObject(LoadInstance());
		}
	}

}