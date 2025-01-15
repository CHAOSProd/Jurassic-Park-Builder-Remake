using UnityEngine;

public class DinoShadowAnim : MonoBehaviour
{
    [SerializeField] private Transform targetBone1;
     [SerializeField] private Transform targetBone2;
    [SerializeField] private GameObject shadowObject1;
    [SerializeField] private GameObject shadowObject2;
    [SerializeField] private Vector3 offset = new Vector3(0, 0.1f, 0);
    [SerializeField] private Vector3 shadowScale = new Vector3(1f, 1f, 1f);

    private void LateUpdate()
    {
        if (targetBone1 != null && shadowObject1 != null)
        {
            shadowObject1.transform.position = targetBone1.position + offset;
            shadowObject1.transform.rotation = Quaternion.identity;
            shadowObject1.transform.localScale = shadowScale;
        }

        if (targetBone2 != null && shadowObject2 != null)
        {
            shadowObject2.transform.position = targetBone2.position + offset;
            shadowObject2.transform.rotation = Quaternion.identity;
            shadowObject2.transform.localScale = shadowScale;
        }
    }
}