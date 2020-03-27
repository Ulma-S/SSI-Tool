using System;
using System.Net;
using UnityEditor;
using UnityEngine;
using LitJson;
using Object = System.Object;

namespace SsiEditor {
	public class SpreadSheetImporter : EditorWindow {
		private string m_apiKey;
		private string m_sheetName;
		private string m_sheetUrl;
		private string m_sheetId;
		private string m_row;
		private string m_colF;
		private string m_colT;
		private string m_urlBase = "https://sheets.googleapis.com/v4/spreadsheets/";
		private string m_commonUrl = "https://docs.google.com/spreadsheets/d/";
		private bool m_showLog = true;
		private ScriptableObject m_script;
		private SsiSettings m_settings;

		[MenuItem("Window/SSI Tool")]
		private static void Init() {
			GetWindow<SpreadSheetImporter>("SSI Tool");
		}

		private void InitField() {
			m_settings = SsiSettings.LoadInstance();
			m_sheetUrl = m_settings.SheetId;
			m_sheetName = m_settings.SheetName;
			m_apiKey = m_settings.ApiKey;
		}

		private void Awake() {
			InitField();
		}

		private void OnGUI() {
			GUIStyle centerBold = new GUIStyle()
			{
				alignment = TextAnchor.MiddleCenter,
				fontStyle = FontStyle.Bold,
			};
			
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				EditorGUILayout.LabelField("Settings", centerBold);
				EditorGUILayout.Space();
				m_row = EditorGUILayout.TextField("Row (1, 2, 3, …)", m_row);
				EditorGUILayout.Space();
				m_colF = EditorGUILayout.TextField("From (A, B, C …)", m_colF);
				EditorGUILayout.Space();
				m_colT = EditorGUILayout.TextField("To (A, B, C …)", m_colT);
				EditorGUILayout.Space();
				m_script = (ScriptableObject) EditorGUILayout.ObjectField("Scriptable Object", m_script,
					typeof(ScriptableObject), true);
				EditorGUILayout.Space();
				m_showLog = EditorGUILayout.Toggle("Show Log", m_showLog);
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.Space();
				if (GUILayout.Button("Download", GUILayout.Width(100), GUILayout.Height(30))) {
					//if (m_sheetId != null && m_sheetName != null && m_apiKey != null) {
					InitField();
					m_settings = SsiSettings.LoadInstance();
					m_sheetUrl = m_settings.SheetId;
					m_sheetName = m_settings.SheetName;
					m_apiKey = m_settings.ApiKey;
					DownloadJsonData();
					//}
				}
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.Space();
				if (GUILayout.Button("Upload", GUILayout.Width(100), GUILayout.Height(30))) {
				}
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DownloadJsonData() {
			WebClient client = new WebClient();
			ExtractSheetId();
			var targetUrl = m_urlBase + m_sheetId + "/values/" + m_sheetName + "!" + m_colF + m_row + ":" + m_colT + m_row;
			try {
				client.DownloadStringCompleted += OnCompleteDownload;
				client.DownloadStringAsync(new Uri(targetUrl + "?key=" + m_apiKey));
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

			int __count = 0;
			foreach (var property in properties) {
				//不要なプロパティへの代入を防ぐ
				if(jsonData["values"][0].Count-1 < __count) break;
				
				var propertyType = property.PropertyType;
				
				//型変換・代入
				if (propertyType == typeof(string)) {
					property.SetValue(m_script,(string)jsonData["values"][0][__count]);
				}else if (propertyType == typeof(float)) {
					string __tmp = (string)jsonData["values"][0][__count];
					property.SetValue(m_script,float.Parse(__tmp));
				}else if (propertyType == typeof(int)) {
					string __tmp = (string)jsonData["values"][0][__count];
					property.SetValue(m_script,int.Parse(__tmp));
				}
				__count++;
			}

			if(m_showLog) Debug.Log("Update" + " "+ m_script.name + " ScriptableObject!");
		}

		private void ExtractSheetId() {
			var __tmp = m_sheetUrl.Replace(m_commonUrl, "");
			int __count = 0;
			while (true) {
				if(__tmp.Substring(__count, 1) == "/") break;
				__count++;
			}
			m_sheetId = __tmp.Remove(__count);
		}
	}
}