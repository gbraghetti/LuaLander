using UnityEngine;

public class CargoRopeVisual : MonoBehaviour {

  [SerializeField] private SpriteRenderer iconSpriteRenderer;

  private void Start() {
    iconSpriteRenderer.sprite = Lander.Instance.GetCargoSO().sprite;
  }
}
