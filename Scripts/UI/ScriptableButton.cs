using UnityEngine.UI;
using UnityEngine;

public class ScriptableButton : MonoBehaviour
{
    public int level_id;

    private Image button_image;
    [SerializeField] private Transform[] star_positions;
    [SerializeField] private Image starFilled;
    [SerializeField] private Image starEmpty;

    private void Awake() {
        button_image = GetComponent<Image>();
    }

    public void OffButton(){
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.interactable = false;

        // Color color = button_image.color;
        // color.a = 0.35f;
        // button_image.color = color;
    }

    public void InstantiateStars(int count){
        for (int i = 0; i < count; i++)
        {
            var new_star = Instantiate(starFilled, star_positions[i]);
            new_star.gameObject.transform.localPosition = Vector3.zero;
        }
        for (int i = count; i < star_positions.Length; i++)
        {
            var new_star = Instantiate(starEmpty, star_positions[i]);
            new_star.gameObject.transform.localPosition = Vector3.zero;
        }
    }
}
