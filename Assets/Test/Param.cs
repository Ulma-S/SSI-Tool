using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Params"), System.Serializable]
public class Param : ScriptableObject {
	[SerializeField] private string m_type;
	[SerializeField] private int m_hp;
	[SerializeField] private float m_attack;
	[SerializeField] private int m_defence;
	[SerializeField] private string m_name;

	public string Type {
		get => m_type;
		set => m_type = value;
	}

	public int Hp {
		get => m_hp;
		set => m_hp = value;
	}

	public float Attack {
		get => m_attack;
		set => m_attack = value;
	}

	public int Defence {
		get => m_defence;
		set => m_defence = value;
	}

	public string Name {
		get => m_name;
		set => m_name = value;
	}
}
