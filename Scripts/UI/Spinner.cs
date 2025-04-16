using DG.Tweening;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    void Start()
    {
        // Вращаем бесконечно (по оси Z)
        transform.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1); // -1 = бесконечно
    }
}