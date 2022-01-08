using UnityEngine;

public class BoxheadModel : MonoBehaviour
{
    //这个值会影响ScaleModel方法的缩放比例, 这个值应该根据具体建模比例设置
    public static Vector3 GlobalScaler = new Vector3(4, 4, 4);

    [ContextMenu(nameof(SetBoxheadModel))]
    private void SetBoxheadModel()
    {
        transform.localScale = GlobalScaler;
    }
}