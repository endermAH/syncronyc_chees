using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(SkinnedMeshRenderer))]
public class MaterialSetter : MonoBehaviour
{
	[SerializeField] private MeshRenderer _meshRenderer;
	[SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
	private MeshRenderer meshRenderer
	{
		get
		{
			if (_meshRenderer == null)
				_meshRenderer = GetComponent<MeshRenderer>();
			return _meshRenderer;
		}
	}
	private SkinnedMeshRenderer skinnedMeshRenderer
	{
		get
		{
			if (_skinnedMeshRenderer == null)
				_skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
			return _skinnedMeshRenderer;
		}
	}

	public void SetSkinnedMaterial(Material material)
    {
		skinnedMeshRenderer.material = material;
		skinnedMeshRenderer.material.shader = Shader.Find("UnityChan/Eye");
	}

	public void SetSingleMaterial(Material material)
	{
		meshRenderer.material = material;
	}
}
