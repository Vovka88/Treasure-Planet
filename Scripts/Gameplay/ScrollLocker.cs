using UnityEngine;
using UnityEngine.UI;

public class ScrollLocker : MonoBehaviour
{
    public ScrollRect scrollRect;
    private RectTransform contentRect;
    private float minY, maxY;

    void Start() {
        contentRect = scrollRect.content;
        UpdateBounds();
    }

    void LateUpdate() {
        Vector2 pos = contentRect.anchoredPosition;

        // Ограничиваем движение контента
        if (pos.y > minY) {
            pos.y = minY;
            scrollRect.velocity = Vector2.zero; // Останавливаем инерцию
        }
        if (pos.y < maxY) {
            pos.y = maxY;
            scrollRect.velocity = Vector2.zero;
        }

        contentRect.anchoredPosition = pos;
    }

    // Автоматически вычисляем границы
    void UpdateBounds() {
        RectTransform viewport = scrollRect.viewport;

        float contentHeight = contentRect.rect.height;
        float viewportHeight = viewport.rect.height;

        if (contentHeight <= viewportHeight) {
            scrollRect.vertical = false; // Отключаем скролл, если контент меньше экрана
        } else {
            scrollRect.vertical = true;
            minY = 0f; // Верхняя граница (изображение полностью наверху)
            maxY = viewportHeight - contentHeight; // Нижняя граница (изображение полностью внизу)
        }
    }


}
