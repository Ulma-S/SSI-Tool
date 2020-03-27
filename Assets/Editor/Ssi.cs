using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SsiEditor {
	public class Ssi : SettingsProvider {
		private SerializedObject m_settings;

		private Ssi(string path, SettingsScope scope) 
			: base(path, scope) { }

		public override void OnActivate (string searchContext, VisualElement rootElement) {
			m_settings = SsiSettings.GetSerializedObject();
		}

		public override void OnGUI(string searchContext) {
			EditorGUILayout.PropertyField(m_settings.FindProperty("m_sheetURL"));
			EditorGUILayout.PropertyField(m_settings.FindProperty("m_sheetName"));
			EditorGUILayout.PropertyField(m_settings.FindProperty("m_APIKey"));
			m_settings.ApplyModifiedProperties();
		}

		[SettingsProvider]
		private static SettingsProvider Create() {
			string path = "Project/Spreadsheet Importer";
			Ssi provider = new Ssi(path, SettingsScope.Project);
			SerializedObject settings = SsiSettings.GetSerializedObject();
			provider.keywords = GetSearchKeywordsFromSerializedObject(settings);

			return provider;
		}
	}
}