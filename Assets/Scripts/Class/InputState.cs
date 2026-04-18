using UnityEngine;
using EmotivUnityPlugin;
using System.Collections.Generic;


public class InputState
{
    public string Action = "";
    public float Power = 0f;
    public Vector2 HeadMove = Vector2.zero;
    public bool IsJumpTriggered = false;
    public bool IsUsingHeadMovement = false;
}