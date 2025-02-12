using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeController : MonoBehaviour
{
    [SerializeField] private InputAction touch_position = new InputAction();
    [SerializeField] private InputAction touch_press = new InputAction();
    private Vector2 pos_swipe_start;
    private Vector2 pos_swipe_current => touch_position.ReadValue<Vector2>();
    [SerializeField] float swipe_distance = 50f;

    private void Update() {
        touch_position.Enable();
        touch_press.Enable();

        touch_press.performed += context => {pos_swipe_start = pos_swipe_current;};
        touch_press.canceled += context => DetectSwipe();
        // Debug.Log(pos_swipe_current);
    }

    private void DetectSwipe(){
        
        Vector2 delta = pos_swipe_current - pos_swipe_start;
        Vector2 direction = Vector2.zero;

        if(Mathf.Abs(delta.x) > swipe_distance){

            direction.x = Mathf.Clamp(delta.x, -1, 1);
            direction.y = 0f;

            Debug.Log("Swipe Horizontal Detected : " + direction.x);
        }
        else if(Mathf.Abs(delta.y) > swipe_distance){

            direction.y = Mathf.Clamp(delta.y, -1, 1);
            direction.x = 0f;

            Debug.Log("Swipe Vertical Detected : " + direction.y);
        }
    }

}
