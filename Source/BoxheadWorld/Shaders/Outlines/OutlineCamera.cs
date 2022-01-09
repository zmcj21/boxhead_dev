using UnityEngine;

public class OutlineCamera : MonoBehaviour
{
    public Material[] EffectMaterials;

    public int CurrentMaterialIndex = 0;

    public bool UseDebugMode = true;

    private Material effectMaterial = null;

    private void SetEffectMaterial(int index)
    {
        if (index < 0 || index >= EffectMaterials.Length)
        {
            return;
        }
        effectMaterial = EffectMaterials[index];
        CurrentMaterialIndex = index;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (effectMaterial != null)
        {
            Graphics.Blit(src, dest, effectMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    private void Start()
    {
        SetEffectMaterial(CurrentMaterialIndex);
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
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                effectMaterial = null;
                CurrentMaterialIndex = -1;
            }
        }
    }
}