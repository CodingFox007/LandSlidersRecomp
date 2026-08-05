using System;
using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISerializationCallbackReceiver, IEventSystemHandler
{
	[Serializable]
	public class TilesetOverride
	{
		public string m_ignoreMe;

		public LevelTileSetGroup m_toReplace;

		public LevelTileSetGroup m_replacement;

		public float m_chance;
	}

	[SerializeField]
	private float m_minSpeedBeforeTorque = 15f;

	[SerializeField]
	private float m_minTorque = 100f;

	[SerializeField]
	private float m_maxTorque = 1000f;

	[SerializeField]
	private float m_maxSpeedForTorque = 50f;

	[SerializeField]
	private float m_driveMaxVel = 10f;

	[SerializeField]
	private float m_driveMinForce = 5f;

	[SerializeField]
	private float m_driveMaxForce = 500f;

	[SerializeField]
	private bool m_reverse;

	[SerializeField]
	private float m_maxRotAngle = 60f;

	[SerializeField]
	private float m_maxSpeedForRot = 2f;

	[SerializeField]
	private float m_rotSmoothTime = 0.5f;

	private float m_targetRotVel;

	[Dictionary("m_replacementsValues")]
	public List<GameObject> m_replacements;

	[HideInInspector]
	public List<GameObject> m_replacementsValues;

	private Dictionary<GameObject, GameObject> m_replacementObjects;

	[Dictionary("m_materialReplacementsValues")]
	public List<GameObject> m_materialReplacements;

	[HideInInspector]
	public List<Material> m_materialReplacementsValues;

	private Dictionary<GameObject, Material> m_materialReplacementObjects;

	[Dictionary("m_hexMaterialReplacementsValues")]
	public List<Material> m_hexMaterialReplacements;

	[HideInInspector]
	public List<Material> m_hexMaterialReplacementsValues;

	private Dictionary<Material, Material> m_hexMaterialReplacementObjects;

	[Dictionary("m_setPieceReplacementsValues")]
	public List<Level> m_setPieceReplacements;

	[HideInInspector]
	public List<Level> m_setPieceReplacementsValues;

	private Dictionary<Level, Level> m_setPieceReplacementObjects;

	public TilesetOverride[] m_tilesetOverrides;

	private Dictionary<LevelTileSetGroup, TilesetOverride> m_tilesetOverridesDictionary;

	public GameObject m_customUIPrefab;

	private GameObject m_customUIInstance;

	[Dictionary("m_additionsValues")]
	public List<GameObject> m_additions;

	[HideInInspector]
	public List<GameObject> m_additionsValues;

	private Dictionary<GameObject, GameObject> m_additionsObjects;

	[Dictionary("m_colorizeValues")]
	public List<Material> m_colorize;

	[HideInInspector]
	public List<Color> m_colorizeValues;

	private Dictionary<Material, Color> m_colorizeList;

	private Dictionary<Material, Color> m_defaultColorizeList = new Dictionary<Material, Color>();

	[SerializeField]
	public string m_customShaderKeyword = string.Empty;

	[SerializeField]
	public Color m_monochromeBlack = Color.black;

	[SerializeField]
	public Color m_monochromeWhite = Color.white;

	[SerializeField]
	public float m_monochromeScale = 1f;

	[SerializeField]
	public Texture2D m_skyboxTexture;

	[SerializeField]
	private Material m_goldMaterial;

	[SerializeField]
	private UnityEvent m_onLoad = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onUnload = new UnityEvent();

	private ConfigurableJointMotion m_preFreezeMotion;

	private bool m_loaded;

	private ConstantForce m_constantForce;

	private Rigidbody m_parentRigidBody;

	private ConfigurableJoint m_joint;

	private Dictionary<Renderer, Material[]> m_defaultMaterials = new Dictionary<Renderer, Material[]>();

	private float minFrozenRotation = 1f;

	private float maxFrozenRotation = 4f;

	private float freezeRotationSpeed;

	private Dictionary<Rigidbody, float> m_Motors = new Dictionary<Rigidbody, float>();

	public Material GoldMaterial
	{
		get
		{
			if (m_goldMaterial == null)
			{
				return Singleton<Game>.Instance.Player.GoldMaterial;
			}
			return m_goldMaterial;
		}
	}

	public LevelTileSetGroup OverrideTileset(LevelTileSetGroup toReplace)
	{
		if (m_tilesetOverridesDictionary == null)
		{
			m_tilesetOverridesDictionary = new Dictionary<LevelTileSetGroup, TilesetOverride>();
			TilesetOverride[] tilesetOverrides = m_tilesetOverrides;
			foreach (TilesetOverride tilesetOverride in tilesetOverrides)
			{
				m_tilesetOverridesDictionary.Add(tilesetOverride.m_toReplace, tilesetOverride);
			}
		}
		if (!m_tilesetOverridesDictionary.ContainsKey(toReplace))
		{
			return toReplace;
		}
		if (UnityEngine.Random.Range(0f, 1f) <= m_tilesetOverridesDictionary[toReplace].m_chance)
		{
			return m_tilesetOverridesDictionary[toReplace].m_replacement;
		}
		return toReplace;
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_replacements, m_replacementsValues, out m_replacementObjects, "replacementObjects");
		Utils.InitDictionary(m_materialReplacements, m_materialReplacementsValues, out m_materialReplacementObjects, "materialReplacementObjects");
		Utils.InitDictionary(m_hexMaterialReplacements, m_hexMaterialReplacementsValues, out m_hexMaterialReplacementObjects, "hexMaterialReplacementObjects");
		Utils.InitDictionary(m_setPieceReplacements, m_setPieceReplacementsValues, out m_setPieceReplacementObjects, "setPieceReplacementObjects");
		Utils.InitDictionary(m_additions, m_additionsValues, out m_additionsObjects, "additionsObjects");
		Utils.InitDictionary(m_colorize, m_colorizeValues, out m_colorizeList, "colorizeList");
	}

	public GameObject GetReplacement(GameObject obj)
	{
		if (m_replacementObjects == null || obj == null)
		{
			return obj;
		}
		if (m_replacementObjects.ContainsKey(obj))
		{
			return m_replacementObjects[obj];
		}
		return obj;
	}

	public Material GetMaterialReplacement(GameObject obj)
	{
		if (m_materialReplacementObjects == null || obj == null)
		{
			return null;
		}
		if (m_materialReplacementObjects.ContainsKey(obj))
		{
			return m_materialReplacementObjects[obj];
		}
		return null;
	}

	public bool GetHexMaterialReplacement(ref Material obj)
	{
		if (m_hexMaterialReplacementObjects == null || obj == null)
		{
			return false;
		}
		string text = obj.ToString();
		text = text.Replace(" (Instance)", string.Empty);
		foreach (KeyValuePair<Material, Material> hexMaterialReplacementObject in m_hexMaterialReplacementObjects)
		{
			if (hexMaterialReplacementObject.Key.ToString() == text)
			{
				obj = hexMaterialReplacementObject.Value;
				return true;
			}
		}
		return false;
	}

	public Level GetSetPieceReplacement(Level obj)
	{
		if (m_setPieceReplacementObjects == null || obj == null)
		{
			return obj;
		}
		if (m_setPieceReplacementObjects.ContainsKey(obj))
		{
			return m_setPieceReplacementObjects[obj];
		}
		return obj;
	}

	public GameObject GetAdditions(GameObject obj)
	{
		if (m_additionsObjects == null || obj == null)
		{
			return null;
		}
		if (m_additionsObjects.ContainsKey(obj))
		{
			return m_additionsObjects[obj];
		}
		return null;
	}

	public void Load()
	{
		m_loaded = true;
		m_defaultColorizeList.Clear();
		foreach (KeyValuePair<Material, Color> colorize in m_colorizeList)
		{
			m_defaultColorizeList.Add(colorize.Key, colorize.Key.GetColor("_Colorize"));
			colorize.Key.SetColor("_Colorize", colorize.Value);
		}
		if (m_customUIPrefab != null)
		{
			m_customUIInstance = UnityEngine.Object.Instantiate(m_customUIPrefab);
			m_customUIInstance.transform.SetParent(Singleton<Game>.Instance.UIController.transform, false);
		}
		m_onLoad.Invoke();
	}

	public void Unload()
	{
		if (!m_loaded)
		{
			return;
		}
		if (m_customUIInstance != null)
		{
			UnityEngine.Object.Destroy(m_customUIInstance);
		}
		foreach (KeyValuePair<Material, Color> defaultColorize in m_defaultColorizeList)
		{
			defaultColorize.Key.SetColor("_Colorize", defaultColorize.Value);
		}
		m_onUnload.Invoke();
	}

	private void OnDestroy()
	{
		Unload();
	}

	public void SetupPhysics(Rigidbody parentRigidBody)
	{
		m_parentRigidBody = parentRigidBody;
		m_joint = GetComponent<ConfigurableJoint>();
		m_joint.connectedBody = m_parentRigidBody;
		GetComponent<SpringJoint>().connectedBody = m_parentRigidBody;
		m_constantForce = GetComponent<ConstantForce>();
		m_defaultMaterials.Clear();
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			m_defaultMaterials.Add(renderer, renderer.sharedMaterials);
		}
		m_preFreezeMotion = m_joint.angularYMotion;
		m_Motors.Clear();
		HingeJoint[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<HingeJoint>();
		foreach (HingeJoint hingeJoint in componentsInChildren2)
		{
			Rigidbody component = hingeJoint.gameObject.GetComponent<Rigidbody>();
			if (hingeJoint.useMotor && component != null)
			{
				m_Motors.Add(component, hingeJoint.motor.targetVelocity);
			}
		}
	}

	public void Freeze()
	{
		freezeRotationSpeed = UnityEngine.Random.Range(minFrozenRotation, maxFrozenRotation);
		if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
		{
			freezeRotationSpeed = 0f - freezeRotationSpeed;
		}
		foreach (KeyValuePair<Rigidbody, float> motor in m_Motors)
		{
			Rigidbody key = motor.Key;
			HingeJoint component = key.gameObject.GetComponent<HingeJoint>();
			key.angularVelocity = Vector3.zero;
			component.motor = new JointMotor
			{
				force = component.motor.force,
				freeSpin = component.motor.freeSpin,
				targetVelocity = 0f
			};
		}
	}

	public void Unfreeze()
	{
		UnsetMaterials();
		foreach (KeyValuePair<Rigidbody, float> motor in m_Motors)
		{
			Rigidbody key = motor.Key;
			float value = motor.Value;
			HingeJoint component = key.gameObject.GetComponent<HingeJoint>();
			component.motor = new JointMotor
			{
				force = component.motor.force,
				freeSpin = component.motor.freeSpin,
				targetVelocity = value
			};
		}
	}

	private void FixedUpdate()
	{
		if (m_parentRigidBody == null || m_joint == null)
		{
			return;
		}
		Vector3 velocity = m_parentRigidBody.velocity;
		float magnitude = velocity.magnitude;
		if (magnitude > m_minSpeedBeforeTorque)
		{
			Vector3 torque = m_constantForce.torque;
			torque = velocity;
			torque.y = 0f;
			float x = torque.x;
			torque.x = 0f - torque.z;
			torque.z = x;
			float t = 0f;
			if (m_maxSpeedForTorque != 0f)
			{
				t = magnitude / m_maxSpeedForTorque;
			}
			torque = torque.normalized * Mathf.Lerp(m_minTorque, m_maxTorque, t);
			if (m_reverse)
			{
				m_constantForce.torque = -torque;
			}
			else
			{
				m_constantForce.torque = torque;
			}
		}
		else
		{
			m_constantForce.torque = Vector3.zero;
		}
		JointDrive slerpDrive = m_joint.slerpDrive;
		if (magnitude > m_driveMaxVel)
		{
			slerpDrive.maximumForce = m_driveMinForce;
		}
		else
		{
			float t2 = 0f;
			if (m_driveMaxVel != 0f)
			{
				t2 = magnitude / m_driveMaxVel;
			}
			slerpDrive.maximumForce = Mathf.Lerp(m_driveMaxForce, m_driveMinForce, t2);
		}
		m_joint.slerpDrive = slerpDrive;
		Vector3 eulerAngles = m_joint.targetRotation.eulerAngles;
		float target = Mathf.Sign(velocity.x) * Mathf.Lerp(0f, m_maxRotAngle, Mathf.Abs(velocity.x / m_maxSpeedForRot));
		if (Singleton<Game>.Instance.Player.CharacterInfo.m_faceTargetDirection)
		{
			if (velocity.magnitude > 0.05f)
			{
				target = Mathf.Atan2(velocity.z, velocity.x) * 57.29578f + 90f;
				eulerAngles.y = Mathf.SmoothDampAngle(eulerAngles.y, target, ref m_targetRotVel, m_rotSmoothTime);
			}
		}
		else
		{
			eulerAngles.y = Mathf.SmoothDampAngle(eulerAngles.y, target, ref m_targetRotVel, (!Singleton<Game>.Instance.Player.IsFrozen) ? m_rotSmoothTime : 1f);
		}
		if (Singleton<Game>.Instance.Player.IsFrozen)
		{
			m_joint.angularYMotion = ConfigurableJointMotion.Free;
			eulerAngles.y += freezeRotationSpeed;
		}
		else if (m_preFreezeMotion == ConfigurableJointMotion.Locked)
		{
			if (Mathf.Abs(eulerAngles.y) < 0.05f)
			{
				m_joint.angularYMotion = m_preFreezeMotion;
			}
			else
			{
				eulerAngles.y = Mathf.MoveTowardsAngle(eulerAngles.y, 0f, 180f * Time.deltaTime);
			}
		}
		else if (m_preFreezeMotion == ConfigurableJointMotion.Limited)
		{
			m_joint.angularYMotion = m_preFreezeMotion;
		}
		m_joint.targetRotation = Quaternion.Euler(eulerAngles);
	}

	public void SetMaterials(Material mat)
	{
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			Material[] array2 = new Material[renderer.sharedMaterials.Length];
			for (int j = 0; j < renderer.sharedMaterials.Length; j++)
			{
				if (renderer.GetComponent<ParticleSystem>() == null)
				{
					array2[j] = mat;
				}
				else
				{
					array2[j] = renderer.sharedMaterials[j];
				}
			}
			renderer.sharedMaterials = array2;
		}
	}

	public void UnsetMaterials()
	{
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (m_defaultMaterials.ContainsKey(renderer))
			{
				renderer.sharedMaterials = m_defaultMaterials[renderer];
			}
		}
	}
}
