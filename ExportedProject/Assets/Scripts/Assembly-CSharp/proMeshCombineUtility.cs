using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proMeshCombineUtility : MonoBehaviour
{
	private bool generateTriangleStrips = true;

	private int optLevel;

	[SerializeField]
	public bool AddMeshColliders = true;

	[SerializeField]
	public bool RemoveLeftOvers;

	[SerializeField]
	public bool PrepareForLightmaps;

	[HideInInspector]
	public int currentOptimizationLevelIndex;

	[SerializeField]
	public bool CombineAtStart;

	[SerializeField]
	public bool CombineOnlySelected;

	[SerializeField]
	public bool CombineDynamicSmooth = true;

	[SerializeField]
	public bool CombineOnlyStatic;

	[SerializeField]
	public bool MarkStaticAuto;

	private bool RemoveStatic;

	[SerializeField]
	[HideInInspector]
	public string[] mapOptimizationLevel;

	[HideInInspector]
	public List<MeshObjectList> list = new List<MeshObjectList>();

	public bool _MarkStaticAuto
	{
		get
		{
			return MarkStaticAuto;
		}
		set
		{
			if (MarkStaticAuto != value)
			{
				MarkStaticAuto = value;
			}
		}
	}

	public bool _CombineOnlyStatic
	{
		get
		{
			return CombineOnlyStatic;
		}
		set
		{
			if (CombineOnlyStatic != value)
			{
				CombineOnlyStatic = value;
			}
		}
	}

	public bool _CombineOnlySelected
	{
		get
		{
			return CombineOnlySelected;
		}
		set
		{
			if (CombineOnlySelected != value)
			{
				CombineOnlySelected = value;
			}
		}
	}

	public bool _CombineAtStart
	{
		get
		{
			return CombineAtStart;
		}
		set
		{
			if (CombineAtStart != value)
			{
				CombineAtStart = value;
			}
		}
	}

	public int OptimizationIndexVal
	{
		get
		{
			return currentOptimizationLevelIndex;
		}
		set
		{
			if (currentOptimizationLevelIndex != value)
			{
				currentOptimizationLevelIndex = value;
			}
		}
	}

	public bool _PrepareForLightmaps
	{
		get
		{
			return PrepareForLightmaps;
		}
		set
		{
			if (PrepareForLightmaps != value)
			{
				PrepareForLightmaps = value;
			}
		}
	}

	public bool _RemoveLeftOvers
	{
		get
		{
			return RemoveLeftOvers;
		}
		set
		{
			if (RemoveLeftOvers != value)
			{
				RemoveLeftOvers = value;
			}
		}
	}

	public bool _AddMeshColliders
	{
		get
		{
			return AddMeshColliders;
		}
		set
		{
			if (AddMeshColliders != value)
			{
				AddMeshColliders = value;
			}
		}
	}

	public void CombineAllSelectedMeshRuntime()
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < this.list.Count; i++)
		{
			if (this.list[i].doCombine)
			{
				list.Add(this.list[i].mainObject);
			}
		}
		BatchPrepare(list, AddMeshColliders, RemoveLeftOvers, PrepareForLightmaps, mapOptimizationLevel[currentOptimizationLevelIndex]);
	}

	public void CombineAllMeshRuntime()
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < this.list.Count; i++)
		{
			list.Add(this.list[i].mainObject);
		}
		BatchPrepare(list, AddMeshColliders, RemoveLeftOvers, PrepareForLightmaps, mapOptimizationLevel[currentOptimizationLevelIndex]);
	}

	public void UnCombineAllSelectedMeshRuntime()
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].doCombine)
			{
				UnBatch(list[i].mainObject);
			}
		}
	}

	public void UnCombineAllMeshRuntime()
	{
		for (int i = 0; i < list.Count; i++)
		{
			UnBatch(list[i].mainObject);
		}
	}

	public void CombineCustomRuntime(GameObject go, bool l_AddMeshColliders, bool l_RemoveLeftOvers)
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(go);
		string optimizationLevel = mapOptimizationLevel[currentOptimizationLevelIndex];
		BatchPrepare(list, l_AddMeshColliders, l_RemoveLeftOvers, false, optimizationLevel);
	}

	public void UnBatch(GameObject go)
	{
		Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>();
		if (componentsInChildren.Length > 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].gameObject.name == "COMBINED_MESH_DATA")
				{
					UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
					continue;
				}
				if (componentsInChildren[i].gameObject.name.Contains("Batch_"))
				{
					Debug.Log("It seems that RemoveLeftOvers was set to true when combining. When RemoveLeftOvers is checked recover batched data is not possible.");
				}
				MeshRenderer[] componentsInChildren2 = componentsInChildren[i].gameObject.GetComponentsInChildren<MeshRenderer>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].enabled = true;
				}
			}
		}
		else
		{
			Debug.Log("No Batch information was found. If you want to UnCombine Mesh make sure RemoveLeftOvers is false");
		}
	}

	public void Start()
	{
		if (CombineAtStart)
		{
			RemoveStatic = true;
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < this.list.Count; i++)
			{
				list.Add(this.list[i].mainObject);
			}
			BatchPrepare(list, AddMeshColliders, RemoveLeftOvers, PrepareForLightmaps, mapOptimizationLevel[currentOptimizationLevelIndex]);
		}
	}

	public void BatchPrepare(List<GameObject> gos, bool AddMeshColliders, bool RemoveLeftOvers, bool isPrepareForLightmapping, string optimizationLevel)
	{
		StartCoroutine(Batch(gos, AddMeshColliders, RemoveLeftOvers, isPrepareForLightmapping, optimizationLevel));
	}

	public IEnumerator Batch(List<GameObject> gos, bool AddMeshColliders, bool RemoveLeftOvers, bool isPrepareForLightmapping, string optimizationLevel)
	{
		generateTriangleStrips = true;
		generateTriangleStrips = false;
		if (optimizationLevel.Equals("low"))
		{
			optLevel = 32000;
		}
		else if (optimizationLevel.Equals("medium"))
		{
			optLevel = 63000;
		}
		else if (optimizationLevel.Equals("high"))
		{
			optLevel = 81000;
		}
		for (int i = 0; i < gos.Count; i++)
		{
			yield return StartCoroutine(_Batch(gos[i], AddMeshColliders, RemoveLeftOvers, isPrepareForLightmapping));
		}
		yield return 0;
	}

	public IEnumerator _Batch(GameObject _go, bool AddMeshColliders = false, bool RemoveLeftOvers = false, bool isPrepareForLightmapping = false)
	{
		Component[] filters = _go.GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 myTransform = _go.transform.worldToLocalMatrix;
		List<Hashtable> materialToMesh = new List<Hashtable>();
		int vertexCalc = 0;
		int hasIterations = 0;
		materialToMesh.Add(new Hashtable());
		for (int i = 0; i < filters.Length; i++)
		{
			MeshFilter meshFilter = (MeshFilter)filters[i];
			if (CombineOnlyStatic && !RemoveStatic)
			{
				if (!meshFilter.gameObject.isStatic)
				{
					if (!MarkStaticAuto)
					{
						continue;
					}
					meshFilter.gameObject.isStatic = true;
				}
			}
			else if (RemoveStatic && meshFilter.gameObject.isStatic)
			{
				Debug.Log("[proMeshCombineUtility] Objects must be non-static in the scene in order to combine them at Runtime.");
				meshFilter.gameObject.isStatic = false;
			}
			Renderer component = filters[i].GetComponent<Renderer>();
			MeshCombineUtility.MeshInstance meshInstance = new MeshCombineUtility.MeshInstance
			{
				mesh = meshFilter.sharedMesh
			};
			if (!meshInstance.mesh)
			{
				continue;
			}
			vertexCalc += meshInstance.mesh.vertexCount;
			if (!(component != null) || !component.enabled || !(meshInstance.mesh != null))
			{
				continue;
			}
			meshInstance.transform = myTransform * meshFilter.transform.localToWorldMatrix;
			Material[] sharedMaterials = component.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				meshInstance.subMeshIndex = Math.Min(j, meshInstance.mesh.subMeshCount - 1);
				ArrayList arrayList = (ArrayList)materialToMesh[hasIterations][sharedMaterials[j]];
				if (arrayList != null)
				{
					arrayList.Add(meshInstance);
				}
				else
				{
					arrayList = new ArrayList();
					arrayList.Add(meshInstance);
					materialToMesh[hasIterations].Add(sharedMaterials[j], arrayList);
				}
				if (vertexCalc > optLevel)
				{
					vertexCalc = 0;
					hasIterations++;
					materialToMesh.Add(new Hashtable());
				}
			}
			if (!RemoveLeftOvers)
			{
				component.enabled = false;
			}
		}
		for (int k = 0; k < hasIterations + 1; k++)
		{
			IDictionaryEnumerator enumerator = materialToMesh[k].GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry de = (DictionaryEntry)enumerator.Current;
					if (CombineDynamicSmooth)
					{
						yield return 0;
					}
					ArrayList elements = (ArrayList)de.Value;
					MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));
					GameObject go = new GameObject("COMBINED_MESH_DATA");
					go.transform.parent = _go.transform;
					go.transform.localScale = Vector3.one;
					go.transform.localRotation = Quaternion.identity;
					go.transform.localPosition = Vector3.zero;
					go.AddComponent(typeof(MeshFilter));
					go.AddComponent<MeshRenderer>();
					go.isStatic = true;
					go.GetComponent<Renderer>().material = (Material)de.Key;
					MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
					filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
					if (isPrepareForLightmapping)
					{
					}
					if (AddMeshColliders)
					{
						if (CombineDynamicSmooth)
						{
							yield return 0;
						}
						go.AddComponent<MeshCollider>();
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				IDisposable disposable = (disposable2 = enumerator as IDisposable);
				if (disposable2 != null)
				{
					disposable.Dispose();
				}
			}
		}
		if (RemoveLeftOvers)
		{
			List<GameObject> list = new List<GameObject>();
			int num = 0;
			MeshRenderer[] componentsInChildren = _go.GetComponentsInChildren<MeshRenderer>();
			for (int l = 0; l < componentsInChildren.Length; l++)
			{
				if ((bool)componentsInChildren[l])
				{
					list.Add(componentsInChildren[l].gameObject);
				}
			}
			for (int m = 0; m < list.Count; m++)
			{
				if (list[m].name != "COMBINED_MESH_DATA")
				{
					UnityEngine.Object.DestroyImmediate(list[m]);
				}
				else
				{
					list[m].name = "Batch_" + num++;
				}
			}
		}
		yield return 0;
	}

	public IEnumerator BatchCollision(List<GameObject> gos, bool RemoveLeftOvers, string optimizationLevel)
	{
		generateTriangleStrips = true;
		generateTriangleStrips = false;
		if (optimizationLevel.Equals("low"))
		{
			optLevel = 32000;
		}
		else if (optimizationLevel.Equals("medium"))
		{
			optLevel = 63000;
		}
		else if (optimizationLevel.Equals("high"))
		{
			optLevel = 81000;
		}
		for (int i = 0; i < gos.Count; i++)
		{
			yield return StartCoroutine(_BatchCollision(gos[i], RemoveLeftOvers));
		}
		yield return 0;
	}

	public IEnumerator _BatchCollision(GameObject _go, bool RemoveLeftOvers = false)
	{
		Component[] colliders = _go.GetComponentsInChildren(typeof(MeshCollider));
		Matrix4x4 myTransform = _go.transform.worldToLocalMatrix;
		List<Hashtable> materialToMesh = new List<Hashtable>();
		int vertexCalc = 0;
		int hasIterations = 0;
		materialToMesh.Add(new Hashtable());
		for (int i = 0; i < colliders.Length; i++)
		{
			MeshCollider meshCollider = (MeshCollider)colliders[i];
			if (CombineOnlyStatic && !RemoveStatic)
			{
				if (!meshCollider.gameObject.isStatic)
				{
					if (!MarkStaticAuto)
					{
						continue;
					}
					meshCollider.gameObject.isStatic = true;
				}
			}
			else if (RemoveStatic && meshCollider.gameObject.isStatic)
			{
				Debug.Log("[proMeshCombineUtility] Objects must be non-static in the scene in order to combine them at Runtime.");
				meshCollider.gameObject.isStatic = false;
			}
			MeshCombineUtility.MeshInstance meshInstance = new MeshCombineUtility.MeshInstance
			{
				mesh = meshCollider.sharedMesh
			};
			if (!meshInstance.mesh)
			{
				continue;
			}
			vertexCalc += meshInstance.mesh.vertexCount;
			if (!(meshCollider != null) || !meshCollider.enabled || !(meshInstance.mesh != null))
			{
				continue;
			}
			meshInstance.transform = myTransform * meshCollider.transform.localToWorldMatrix;
			PhysicMaterial sharedMaterial = meshCollider.sharedMaterial;
			if (sharedMaterial != null)
			{
				meshInstance.subMeshIndex = 0;
				ArrayList arrayList = (ArrayList)materialToMesh[hasIterations][sharedMaterial];
				if (arrayList != null)
				{
					arrayList.Add(meshInstance);
				}
				else
				{
					arrayList = new ArrayList();
					arrayList.Add(meshInstance);
					materialToMesh[hasIterations].Add(sharedMaterial, arrayList);
				}
				if (vertexCalc > optLevel)
				{
					vertexCalc = 0;
					hasIterations++;
					materialToMesh.Add(new Hashtable());
				}
			}
			if (!RemoveLeftOvers)
			{
				meshCollider.enabled = false;
			}
		}
		for (int j = 0; j < hasIterations + 1; j++)
		{
			foreach (DictionaryEntry item in materialToMesh[j])
			{
				if (CombineDynamicSmooth)
				{
				}
				ArrayList arrayList2 = (ArrayList)item.Value;
				MeshCombineUtility.MeshInstance[] combines = (MeshCombineUtility.MeshInstance[])arrayList2.ToArray(typeof(MeshCombineUtility.MeshInstance));
				GameObject gameObject = new GameObject("COMBINED_MESH_DATA");
				gameObject.transform.parent = _go.transform;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.AddComponent<MeshCollider>();
				gameObject.isStatic = true;
				gameObject.GetComponent<MeshCollider>().material = (PhysicMaterial)item.Key;
				MeshCollider meshCollider2 = (MeshCollider)gameObject.GetComponent(typeof(MeshCollider));
				meshCollider2.sharedMesh = MeshCombineUtility.Combine(combines, generateTriangleStrips);
			}
		}
		if (RemoveLeftOvers)
		{
			List<GameObject> list = new List<GameObject>();
			int num = 0;
			MeshCollider[] componentsInChildren = _go.GetComponentsInChildren<MeshCollider>();
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				if ((bool)componentsInChildren[k])
				{
					list.Add(componentsInChildren[k].gameObject);
				}
			}
			for (int l = 0; l < list.Count; l++)
			{
				if (list[l].name != "COMBINED_MESH_DATA")
				{
					UnityEngine.Object.DestroyImmediate(list[l]);
				}
				else
				{
					list[l].name = "Batch_" + num++;
				}
			}
		}
		yield return 0;
	}
}
