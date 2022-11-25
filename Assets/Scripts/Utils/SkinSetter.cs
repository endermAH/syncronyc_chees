using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SkinSetter : MonoBehaviour
{
    [SerializeField] private Material KnightBlackMaterial;
    [SerializeField] private Material KnightWhiteMaterial;

    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void SetSkinnedMaterial(int type, TeamColor team)
    {
        switch (type)
        {
            case 3:
                if (team == TeamColor.White)
                    skinnedMeshRenderer.material = KnightBlackMaterial;
                else
                    skinnedMeshRenderer.material = KnightBlackMaterial;
                break;
            default:
                Debug.Log($"No Material for {type} of {team}");
                break;
        }
        
    }
}
