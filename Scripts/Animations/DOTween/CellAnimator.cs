using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Data;

public class CellAnimator : MonoBehaviour
{
    public Sprite[] bomb_sprites; // не нужно
    public Sprite[] splitter_sprites; // не нужно
    public Sprite[] ultrabomb_sprites; // не нужно

    public GameObject bomb_anim_prefab;

    void Start()
    {
        // Invoke(nameof(ExecuteAnimationBomb), 0.2f);
        ExecuteAnimationUltraBomb(0, new GameObject[] { p1, p2 });
        // Invoke(nameof(ExecuteAnimationSplitter), 0.2f);
        ExecuteAnimationSplitter(true);
    }

    public void ExecuteAnimationBomb(int id)
    {
        var anim = Instantiate(bomb_anim_prefab, transform);
        anim.GetComponent<Image>().sprite = bomb_sprites[id];
        Debug.Log("Before fade: " + anim.GetComponent<Image>().color.a);
        anim.GetComponent<Image>().DOFade(1f, 0.5f).OnComplete(() =>
        {
            Debug.Log("After fade: " + anim.GetComponent<Image>().color.a);
        });
        // anim.GetComponent<Image>().DOFade(1f, 0.5f);
        anim.GetComponent<Image>().rectTransform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 2f);
        anim.GetComponent<Image>().rectTransform.DORotate(new Vector3(0, 0, 0), 1f, RotateMode.FastBeyond360);
        this.GetComponent<Image>().rectTransform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).OnComplete(() =>
        {
            anim.GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(() => DestroyImmediate(anim));
        });
    }

    // false = Горізонтальна 
    // true  = Вертикальна

    public RectTransform boardRect;
    public void ExecuteAnimationSplitter(bool mode = false)
    {
        Vector3 from = transform.position;

        // Правая мировая граница

        Vector3 to = new Vector3();

        if (!mode)
        {
            Vector3 worldRight = boardRect.TransformPoint(new Vector3(boardRect.rect.xMax, 0, 0));
            to = new Vector3(worldRight.x, from.y, from.z);
        }
        else
        {
            Vector3 worldUp = boardRect.TransformPoint(new Vector3(0, boardRect.rect.yMax, 0));
            to = new Vector3(from.x, worldUp.y, from.z);
        }

        CreateBeam(from, to);
    }



    public GameObject beamPrefab;
    public GameObject canvasTransform;


    public GameObject p1; // не нужно
    public GameObject p2; // не нужно

    public void ExecuteAnimationUltraBomb(int id, GameObject[] targets)
    {
        transform.DOScale(1.3f, 2f);
        transform.DOShakeRotation(2f, strength: 45, randomnessMode: ShakeRandomnessMode.Harmonic);
        foreach (var target in targets)
        {
            CreateAnimatedBeam(gameObject, target);
        }
    }

    private void CreateAnimatedBeam(GameObject source, GameObject target)
    {
        Vector3 start = source.transform.position;
        Vector3 end = target.transform.position;
        CreateBeam(start, end);
    }

    private void CreateBeam(Vector3 from, Vector3 to)
    {
        var dir = to - from;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var beam = Instantiate(beamPrefab, from, Quaternion.identity, canvasTransform.transform);
        var rect = beam.GetComponent<RectTransform>();

        rect.sizeDelta = new Vector2(distance * 100, rect.sizeDelta.y);
        rect.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(beam, 2f);
    }


}
