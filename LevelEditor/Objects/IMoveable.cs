﻿using Microsoft.Xna.Framework;

namespace LevelEditor.Objects
{
    interface IMoveable
    {

        bool IsMoving { get; set; }

        bool IsCrazy { get; set; }
        void Move();

        void SetTarget(Vector3 target);
    }
}
