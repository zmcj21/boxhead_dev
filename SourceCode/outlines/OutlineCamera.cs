using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OutlineCamera : MonoBehaviour
{
    //这里依赖Outlines提供的Material, 需要手动赋值
    public Material[] EffectMaterials;

    public bool UseDebugMode = true;

    private Material effectMaterial = null;

    private int materialIndex = 0;

    private void SetEffectMaterial(int index)
    {
        if (index < 0 || index >= EffectMaterials.Length)
        {
            return;
        }
        effectMaterial = EffectMaterials[index];
        materialIndex = index;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (effectMaterial != null)
            Graphics.Blit(src, dest, effectMaterial);
        else
            dest = src;
    }

    private void Start()
    {
        SetEffectMaterial(0);
    }

    private void Update()
    {
        if (UseDebugMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetEffectMaterial(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetEffectMaterial(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetEffectMaterial(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetEffectMaterial(3);
            }
        }
    }
}