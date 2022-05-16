using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using DG.Tweening;

public class LaserCreator : MonoBehaviour {

    [SerializeField] private BoxCollider cardCollider;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private GameObject instantiatedObject;
    [SerializeField] private Transform lightoEdge;
    [SerializeField] private Transform laserRotation;
    private void Update() {
        Lighto2();
    }

    private void Lighto2() {

        #region BoxCast

        RaycastHit hit3;
        Bounds bounds = cardCollider.bounds;
        float rayLength = 5f;

        bool isHitCollectible = Physics.BoxCast(
            center: new Vector3(bounds.center.x, bounds.center.y, bounds.max.z),
            halfExtents: bounds.extents,
            direction: Vector3.forward,
            hitInfo: out hit3,
            orientation: Quaternion.identity,
            maxDistance: rayLength,
            layerMask: targetLayerMask
        );

        if (isHitCollectible) {
            
                instantiatedObject.SetActive(true);
                instantiatedObject.transform.SetPositionAndRotation(new Vector3(
                    lightoEdge.transform.position.x, lightoEdge.transform.position.y, hit3.point.z), 
                    laserRotation.rotation);

        }
        else {
            instantiatedObject.SetActive(false);
        }

        Color rayColor = isHitCollectible ? Color.green : Color.red;
        DrawSquareRay(cardCollider.bounds, Vector3.forward, rayLength, rayColor);

        #endregion

        #region BoxCastAll
        //RaycastHit[] hits;
        //hits = Physics.BoxCastAll(lightoEdge.transform.position, Vector3.forward, Vector3.forward * 100);

        //for (int i = 0; i < hits.Length; i++) {
        //    RaycastHit hit = hits[i];
        //    Renderer rend = hit.transform.GetComponent<Renderer>();
        //    if (rend) {
        //        Color tempColor = rend.material.color;
        //        tempColor.r = 0.0F;
        //        rend.material.color = tempColor;
        //        Debug.Log("1");
        //    }

        //} 
        #endregion

        #region Raycast
        //RaycastHit hit2;
        //bool isHitCollectible = Physics.Raycast(
        //    origin: lightoEdge.transform.position,
        //    direction: Vector3.forward,
        //    hitInfo: out hit2,
        //    maxDistance: 10f,
        //    layerMask: targetLayerMask);
        //if (isHitCollectible) {

        //    MeshRenderer rend = hit2.transform.GetComponent<MeshRenderer>();
        //    if (rend) {
        //        instantiatedObject.gameObject.SetActive(true);
        //        instantiatedObject.transform.position = hit2.point;
        //        instantiatedObject.transform.rotation = laserRotation.rotation;
        //        Color tempColor = rend.material.color;
        //        tempColor.r = 0.0F;
        //        rend.material.color = tempColor;
        //    }
        //}
        //else {
        //    instantiatedObject.gameObject.SetActive(false);
        //}

        //Color rayColor = isHitCollectible ? Color.green : Color.red;
        //DrawSquareRay(cardCollider.bounds, Vector3.forward, 5f, rayColor);
        #endregion

        #region RaycastAll
        //RaycastHit[] hits;
        //hits = Physics.RaycastAll(
        //    lightoEdge.transform.position,
        //    Vector3.forward,
        //    100f,
        //    targetLayerMask);

        //foreach (var hit in hits) {
        //    Debug.Log("0");
        //    MeshRenderer rend = hit.transform.GetComponent<MeshRenderer>();
        //    if (rend) {
        //        Color tempColor = rend.material.color;
        //        tempColor.r = 0.0F;
        //        rend.material.color = tempColor;

        //    }
        //} 
        #endregion


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
