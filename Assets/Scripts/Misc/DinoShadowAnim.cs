using UnityEngine;

public class DinoShadowAnim : MonoBehaviour
{
    [SerializeField] private Transform targetBone;
    [SerializeField] private Vector3 offset = new Vector3(0, 0.1f, 0);
    [SerializeField] private Vector3 shadowScale = new Vector3(1f, 1f, 1f);

    private void LateUpdate()
    {
        if (targetBone != null)
        {
            transform.position = targetBone.position + offset;
            transform.rotation = Quaternion.identity;
            transform.localScale = shadowScale;
        }
    }
}