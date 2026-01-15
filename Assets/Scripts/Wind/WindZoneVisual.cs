
using System.Collections.Generic;
using UnityEngine;

public class WindZoneVisual : MonoBehaviour {

  [SerializeField] private List<Transform> arrowTransformList;

  private WindZone windZone;

  private void Awake() {
    windZone = GetComponent<WindZone>();
  }

  private void Start() {
    foreach (Transform arrowTransform in arrowTransformList) {
      arrowTransform.right = windZone.GetWindForce();
    }
  }
}
