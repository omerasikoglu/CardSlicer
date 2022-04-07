using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

//TODO: optimisation
public class PlatformCreator : MonoBehaviour
{
    [SerializeField, BoxGroup("[Prefabs]")] private GameObject platform;
    [SerializeField, BoxGroup("[Prefabs]")] private List<GameObject> platformList;
    [SerializeField, BoxGroup("[Settings]")] private float distanceBetweenPlatforms = 10f;

    private Vector3 currentPos;

    private void OnValidate()
    {
        currentPos = new Vector3(transform.position.x, transform.position.y
            , platformList.Count * distanceBetweenPlatforms);
    }

    [Button]
    private void Create()
    {
        //GameObject platformm = new GameObject($"platform{platformList.Count}");

        GameObject platform = Instantiate(this.platform, new Vector3(
                transform.position.x, -0.5f, 1f * platformList.Count * distanceBetweenPlatforms),
            Quaternion.identity, transform);
        platformList.Add(platform);

        platform.name = $"visual{platformList.Count}";
        currentPos += Vector3.forward * distanceBetweenPlatforms;

    }

    [Button]
    private void RemoveLastOne()
    {
        if (platformList.Count <= 1 || transform.childCount <= 1) return;

        //world space'den siler
        DestroyImmediate(platformList[platformList.Count - 1]);

        //listeden çýkarýr
        platformList.RemoveAt(platformList.Count - 1);

        currentPos -= Vector3.forward * distanceBetweenPlatforms;


    }




}
