using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ComicController : MonoBehaviour
{
    [SerializeField] private GameObject comic_container;
    [SerializeField] private GameObject comic_start_position;
    [SerializeField] private GameObject comic_end_position;
    [SerializeField] private GameObject[] comic_slides;

    private int index = 0; // Начинаем с первого слайда (0-й индекс)

    private void Start()
    {
        // Убедимся, что только первый слайд активен при старте
        for (int i = 0; i < comic_slides.Length; i++)
        {
            Image img = comic_slides[i].GetComponent<Image>();
            img.DOFade(i == index ? 1f : 0f, 0f);
        }
    }

    public void ToNextPage()
    {
        if (index < comic_slides.Length - 1)
        {
            Image current = comic_slides[index].GetComponent<Image>();
            Image next = comic_slides[index + 1].GetComponent<Image>();

            current.DOFade(0f, 2f);
            next.DOFade(1f, 1f); // мгновенно делаем видимым перед анимацией

            index++;
            Debug.Log("Next Page: " + index);
        }
    }

    public void ToPastPage()
    {
        if (index > 0)
        {
            Image current = comic_slides[index].GetComponent<Image>();
            Image previous = comic_slides[index - 1].GetComponent<Image>();

            current.DOFade(0f, 1f);
            previous.DOFade(1f, 1f);

            index--;
            Debug.Log("Previous Page: " + index);
        }
    }

    public void SkipPage()
    {
        comic_container.transform.DOMoveY(comic_start_position.transform.position.y, 1f);
    }

    public void StartComic()
    {
        comic_container.transform.DOMoveY(comic_end_position.transform.position.y, 1f);
    }
}