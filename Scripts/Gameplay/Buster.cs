using UnityEngine;

public class Buster : MonoBehaviour
{
    public enum BusterType
    {
        UltraBomb = 1,
        Bomb = 2,
        Splitter = 3,
    }

    public int id;
    public BusterType type;
    public int count;
}
