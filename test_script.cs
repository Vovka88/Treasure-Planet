using UnityEngine;

public class test_script : MonoBehaviour
{
    public static test_script Instance { get; private set; }
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
