using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeController : MonoBehaviour
{
    [SerializeField] private InputAction touch_position = new InputAction();
    [SerializeField] private InputAction touch_press = new InputAction();
    private Vector2 pos_swipe_start;
    private Vector2 pos_swipe_current => touch_position.ReadValue<Vector2>();
    [SerializeField] float swipe_distance = 50f;

    public event Action<Point, Point> OnSwipe;

    private void OnEnable() {
        touch_position.Enable();
        touch_press.Enable();

        touch_press.started += context => pos_swipe_start = pos_swipe_current;
        touch_press.canceled += context => DetectSwipe();
    }

    private void OnDisable() {
        touch_position.Disable();
        touch_press.Disable();
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

        if (direction.x != 0 || direction.y != 0) {
            Point start = GetBoardPointFromScreenPosition(pos_swipe_start);


            // Debug.Log($"Start position: X: {pos_swipe_start.x}; Y: {pos_swipe_start.y}");
            // Debug.Log($"Start point: X: {start.x}; Y: {start.y}");

            OnSwipe?.Invoke(start, new Point((int)direction.x, (int)direction.y));
        }
    }

    private Point GetBoardPointFromScreenPosition(Vector2 screenPos) {
        return new Point(Mathf.RoundToInt(screenPos.x / Config.cell_size) - 1,
        Mathf.RoundToInt(Config.maximal_table_vertical - (screenPos.y / Config.cell_size)) + 2);
    }

    public static Vector2 GetBoardPositionFromPoint(Point point) => new Vector2(
        Config.cell_size / 2  + Config.cell_size * point.x, -Config.cell_size / 2  + -Config.cell_size * point.y);

}
