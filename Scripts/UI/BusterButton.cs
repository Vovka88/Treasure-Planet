using UnityEngine;

public class BusterButton : MonoBehaviour
{
    public Buster.BusterType btn_type;

    public void AddBusterToLevel()
    {
        var bs = DataManager.Instance.players_busters.Find((b) => b.type == btn_type);
        if (bs != null)
        {
            DataManager.Instance.busters_on_level.Add(bs);
            gameObject.SetActive(false);
        }
    }
}
