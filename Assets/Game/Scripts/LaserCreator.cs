using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using DG.Tweening;

public class LaserCreator : MonoBehaviour {

    [SerializeField] private BoxCollider cardCollider;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private GameObject instantiatedObject;
    [SerializeField] private Transform LightoEdge;
    private void Update() {
        Lighto2();
    }

    private void Lighto2() {
        RaycastHit[] hits;
        hits = Physics.BoxCastAll(LightoEdge.transform.position, Vector3.forward, Vector3.forward * 100);

        for (int i = 0; i < hits.Length; i++) {
            RaycastHit hit = hits[i];
            Renderer rend = hit.transform.GetComponent<Renderer>();
            if (rend) {
                Color tempColor = rend.material.color;
                tempColor.r = 0.0F;
                rend.material.color = tempColor;
                Debug.Log("1");
            }

        }


        //if (Physics.RaycastAll(
        //        origin: transform.position,
        //        direction: Vector3.forward,
        //        hitInfo: out hits,
        //        maxDistance: 10f,
        //        layerMask: targetLayerMask)) return;


    }

    private void Lighto() {
        Bounds bounds = cardCollider.bounds;
        float rayLength = 5f;

        bool isHitCollectible = Physics.BoxCast(
            center: new Vector3(bounds.center.x, bounds.center.y, bounds.max.z),
            halfExtents: bounds.extents,
            direction: Vector3.forward,
            orientation: Quaternion.identity,
            maxDistance: rayLength,
            layerMask: targetLayerMask
        );

        Color rayColor = isHitCollectible ? Color.green : Color.red;

        DrawSquareRay(cardCollider.bounds, Vector3.forward, rayLength, rayColor);


    }

    private void DrawSquareRay(Bounds bounds, Vector3 dir, float rayLength, Color rayColor) {
        Debug.DrawRay( //top right
            start: bounds.max,
            dir: dir * rayLength,
            color: rayColor
        );
        Debug.DrawRay( //top left
            start: new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            dir: dir * rayLength,
            color: rayColor
        );
        Debug.DrawRay( //bottom right
            start: new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            dir: dir * rayLength,
            color: rayColor
        );
        Debug.DrawRay( //bottom left
            start: new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            dir: dir * rayLength,
            color: rayColor
        );
    }



}
