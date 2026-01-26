using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreatorUIHandler : MonoBehaviour
{
    [SerializeField] private PlayerCharacterCustomizer customizer;

    [Header("Feature Buttons")]
    public Button hairBtn;
    public Button topBtn;
    public Button bottomBtn;
    public Button skinBtn;
    public Button confirmBtn;

    private void Start() {
        // We cast the Enum to (int) to match the customizer's function
        hairBtn.onClick.AddListener(() => customizer.ChangeBodyPart((int)PlayerCharacterCustomizer.BodyPartType.Hair));
        topBtn.onClick.AddListener(() => customizer.ChangeBodyPart((int)PlayerCharacterCustomizer.BodyPartType.ClothesTop));
        bottomBtn.onClick.AddListener(() => customizer.ChangeBodyPart((int)PlayerCharacterCustomizer.BodyPartType.ClothesBottom));
        skinBtn.onClick.AddListener(() => customizer.ChangeBodyPart((int)PlayerCharacterCustomizer.BodyPartType.SkinColor));

        confirmBtn.onClick.AddListener(() => {
            customizer.SaveCharacter();
            SceneManager.LoadScene("02_GameScene"); 
        });
    }
}