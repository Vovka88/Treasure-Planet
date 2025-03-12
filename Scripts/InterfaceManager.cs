using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField] private Text turns;
    [SerializeField] private TableController tableController;

    private void Start()
    {
        tableController.onValueChanged += UpdateValue;
    }

    private void UpdateValue(int value){
        turns.text = $"Turns count: {value}";
    }
    
}
