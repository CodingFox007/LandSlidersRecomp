using System;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
	[Serializable]
	public class ShaderForMaterials
	{
		public string name;

		public Shader shader;

		public List<Material> materials = new List<Material>();
	}

	public List<ShaderForMaterials> shaders = new List<ShaderForMaterials>();

	public ShaderForMaterials GetShaderForMaterialsForShader(Shader shader)
	{
		if (shader.name.StartsWith("Hidden") || shader.name.StartsWith("_"))
		{
			return null;
		}
		foreach (ShaderForMaterials shader2 in shaders)
		{
			if (shader2.shader == shader)
			{
				return shader2;
			}
		}
		ShaderForMaterials shaderForMaterials = new ShaderForMaterials();
		shaderForMaterials.shader = shader;
		shaderForMaterials.name = shader.name;
		shaders.Add(shaderForMaterials);
		shaders.Sort((ShaderForMaterials _shaderCompareA, ShaderForMaterials _shaderCompareB) => _shaderCompareA.name.CompareTo(_shaderCompareB.name));
		return shaderForMaterials;
	}

	public void AddMaterial(Material material)
	{
		ShaderForMaterials shaderForMaterialsForShader = GetShaderForMaterialsForShader(material.shader);
		if (shaderForMaterialsForShader != null)
		{
			shaderForMaterialsForShader.materials.Add(material);
		}
	}

	private void Start()
	{
	}

	public void ResetShadersToDefault()
	{
		foreach (ShaderForMaterials shader in shaders)
		{
			foreach (Material material in shader.materials)
			{
				if (material != null)
				{
					material.shader = shader.shader;
				}
			}
		}
	}

	private void Update()
	{
	}
}
