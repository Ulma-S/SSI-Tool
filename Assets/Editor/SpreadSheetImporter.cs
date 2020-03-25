using System;
using System.Net;
using UnityEditor;
using UnityEngine;
using LitJson;
using Object = System.Object;

namespace SsiEditor {
	public class SpreadSheetImporter : EditorWindow {
		private string m_sheetUrl;
		private string m_apiKey;
		private string m_sheetName;
		private string m_sheetId;
		private string m_row;
		private string m_colF;
		private string m_colT;
		private string m_urlBase = "https://sheets.googleapis.com/v4/spreadsheets/";
		private ScriptableObject m_script;
		private SsiSettings m_settings;

		[MenuItem("Window/SSI Tool")]
		private static void Init() {
			GetWindow<SpreadSheetImporter>("SSI Tool");
		}

		private void InitField() {
			m_settings = SsiSettings.LoadInstance();
			m_sheetId = m_settings.SheetId;
			m_sheetName = m_settings.SheetName;
			m_apiKey = m_settings.ApiKey;
		}

		private void Awake() {
			InitField();
		}

		private void OnGUI() {
			EditorGUILayout.Space();
			m_row = EditorGUILayout.TextField("Row (1, 2, 3, …)", m_row);
			EditorGUILayout.Space();
			m_colF = EditorGUILayout.TextField("From (A, B, C …)", m_colF);
			EditorGUILayout.Space();
			m_colT = EditorGUILayout.TextField("To (A, B, C …)", m_colT);
			EditorGUILayout.Space();
			m_script = (ScriptableObject)EditorGUILayout.ObjectField("Scriptable Object", m_script, typeof(ScriptableObject), true);
			EditorGUILayout.Space();
			if (GUILayout.Button("Create", GUILayout.Width(50))) {
				//if (m_sheetId != null && m_sheetName != null && m_apiKey != null) {
				m_settings = SsiSettings.LoadInstance();
				m_sheetId = m_settings.SheetId;
				m_sheetName = m_settings.SheetName;
				m_apiKey = m_settings.ApiKey;
				DownloadJsonData();
				//}
			}
		}

		private void DownloadJsonData() {
			WebClient client = new WebClient();
			try {
				m_sheetUrl = m_urlBase + m_sheetId + "/values/" + m_sheetName + "!" + m_colF + m_row + ":" + m_colT + m_row;
				client.DownloadStringCompleted += OnCompleteDownload;
				client.DownloadStringAsync(new Uri(m_sheetUrl + "?key=" + m_apiKey));
			}
			catch (WebException e) {
				Console.WriteLine(e);
				throw;
			}
		}

		private void OnCompleteDownload(Object sender, DownloadStringCompletedEventArgs e) {
			var scriptType = m_script.GetType();
			var properties = scriptType.GetProperties();
			JsonData jsonData = JsonMapper.ToObject(e.Result);

			int count = 0;
			foreach (var property in properties) {
				//不要なプロパティへの代入を防ぐ
				if(jsonData["values"][0].Count-1 < count) break;
				
				var propertyType = property.PropertyType;
				
				//型変換・代入
				if (propertyType == typeof(string)) {
					property.SetValue(m_script,(string)jsonData["values"][0][count]);
				}else if (propertyType == typeof(float)) {
					string __tmp = (string)jsonData["values"][0][count];
					property.SetValue(m_script,double.Parse(__tmp));
				}else if (propertyType == typeof(int)) {
					string __tmp = (string)jsonData["values"][0][count];
					property.SetValue(m_script,int.Parse(__tmp));
				}
				count++;
			}

			Debug.Log("Update" + " "+ m_script.name + " ScriptableObject!");
		}
	}
}