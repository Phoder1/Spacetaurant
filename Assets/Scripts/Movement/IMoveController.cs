using UnityEngine;
using Sirenix.Serialization;
using System;

public interface IMoveController
{
    void Move(Vector2 direction, float distance);
}