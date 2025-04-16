using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelTarget : MonoBehaviour
{

    public Sprite image;
    public CellData.Cell_Type type;
    public int count;

    public GameObject template;
    private GameObject objective;

    public LevelTarget(CellData.Cell_Type type, int count, Sprite image)
    {
        this.type = type;
        this.image = image;
        this.count = count;
    }

    public GameObject InstantiateTarget(GameObject parent)
    {
        template.GetComponent<Image>().sprite = image;
        template.GetComponentInChildren<TMP_Text>().text = count.ToString();

        objective = Instantiate(template, parent.transform);
        return objective;
    }

    public void UpdateCount(int number){
        count = number;
        objective.GetComponentInChildren<TMP_Text>().text = count >= 0 ? count.ToString() : 0.ToString();
    }

    public bool isCompleted()
    {
        return count <= 0;
    }
}