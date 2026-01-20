using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreatorUIHandler : MonoBehaviour {
    [SerializeField] private PlayerCharacterCustomizer customizer;
    public Button hairBtn, topBtn, bottomBtn, skinBtn, confirmBtn;

    private void Start() {
        hairBtn.onClick.AddListener(() => customizer.ChangeBodyPart(PlayerCharacterCustomizer.BodyPartType.Hair));
        topBtn.onClick.AddListener(() => customizer.ChangeBodyPart(PlayerCharacterCustomizer.BodyPartType.ClothesTop));
        bottomBtn.onClick.AddListener(() => customizer.ChangeBodyPart(PlayerCharacterCustomizer.BodyPartType.ClothesBottom));
        skinBtn.onClick.AddListener(() => customizer.ChangeBodyPart(PlayerCharacterCustomizer.BodyPartType.SkinColor));

        confirmBtn.onClick.AddListener(() => {
            customizer.SaveCharacter(); // Save data first
            SceneManager.LoadScene("02_GameScene"); // Then go to game
        });
    }
}