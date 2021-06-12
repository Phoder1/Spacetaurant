using CMF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public class PlayerInput : CharacterInput
    {
        private Vector2 _moveDirection;
        public void Move(Vector2 moveDirection) => _moveDirection = moveDirection;
        public override float GetHorizontalMovementInput() => _moveDirection.x;

        public override float GetVerticalMovementInput() => _moveDirection.y;

        public override bool IsJumpKeyPressed() => false;
    }
}
