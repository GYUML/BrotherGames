using System;
using UnityEngine;

public class MakeMapLayoutTile : MonoBehaviour
{
    public KButton centerButton;

    public KButton upButton;
    public KButton downButton;
    public KButton leftButton;
    public KButton rightButton;

    int center;
    int up;
    int down;
    int left;
    int right;

    Action<int, int, int, int, int> onUpdate;

    private void Start()
    {
        
    }
}
