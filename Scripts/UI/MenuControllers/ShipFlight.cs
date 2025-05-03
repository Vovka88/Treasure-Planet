using UnityEngine;
using DG.Tweening;

public class ShipFlight : MonoBehaviour
{
    [Header("Настройки пути")]
    public Transform[] waypoints;
    public float moveDuration = 2f;
    public float pauseBetweenPoints = 1f;

    [Header("Визуальные компоненты")]
    public GameObject shipSprite;

    private int currentPoint = 0;
    private int targetPoint = 0;
    private int direction = 1; // +1 или -1
    private bool isFlying = false;

    /// <summary>
    /// Летит к указанному уровню (вперёд и назад)
    /// </summary>
    public void FlyToLevel(int levelId)
    {
        if (isFlying || waypoints.Length == 0) return;

        targetPoint = Mathf.Clamp(levelId, 0, waypoints.Length - 1);
        currentPoint = Mathf.Clamp(DataManager.Instance.current_level, 0, waypoints.Length - 1);

        // Телепортируемся в текущую позицию (если нужно)
        transform.position = waypoints[currentPoint].position;

        // Определяем направление движения
        direction = (targetPoint > currentPoint) ? 1 : -1;

        // Если уже на месте — ничего не делаем
        if (currentPoint == targetPoint) return;

        isFlying = true;
        currentPoint += direction;

        FlyStep();
    }

    private void FlyStep()
    {
        if ((direction > 0 && currentPoint > targetPoint) ||
            (direction < 0 && currentPoint < targetPoint))
        {
            isFlying = false;
            DataManager.Instance.current_level = targetPoint;
            return;
        }

        Vector3 target = waypoints[currentPoint].position;
        RotateTowards(target);

        transform.DOMove(target, moveDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                DataManager.Instance.current_level = currentPoint;
                currentPoint += direction;
                Invoke(nameof(FlyStep), pauseBetweenPoints);
            });
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        shipSprite.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}