using UnityEngine.UI;
using UnityEngine;

public class ScriptableButton : MonoBehaviour
{
    public int level_id;

    private Image button_image;
    [SerializeField] private Transform[] star_positions;
    [SerializeField] private Image star;

    private void Awake() {
        button_image = GetComponent<Image>();
    }

    public void OffButton(){
        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();

        Color color = button_image.color;
        color.a = 0.65f;
        button_image.color = color;
    }

    public void InstantiateStars(int count){
        for (int i = 0; i < count; i++)
        {
            var new_star = Instantiate(star, star_positions[i]);
            new_star.gameObject.transform.localPosition = Vector3.zero;
        }
    }
}
