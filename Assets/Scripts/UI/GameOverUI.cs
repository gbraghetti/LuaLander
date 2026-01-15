using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
  [SerializeField] private Button mainMenuButton;
  [SerializeField] private TextMeshProUGUI finalScoreTextMesh;

  private void Awake() {
    mainMenuButton.onClick.AddListener(() => {
      SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
    });
  }

  private void Start() {
    finalScoreTextMesh.text = "FINAL SCORE: " + GameManager.Instance.GetTotalScore().ToString();

    mainMenuButton.Select();
  }
}
