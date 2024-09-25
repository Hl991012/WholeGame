using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringOrgan : MonoBehaviour
{
    public List<Direction> MoveDirections;

    public Direction GetMoveDir(Direction dir)
    {
        foreach (var item in MoveDirections)
        {
            switch (dir)
            {
                case Direction.Up:
                    if(item == Direction.Down)
                        continue;
                    break;
                case Direction.Down:
                    if(item == Direction.Up)
                        continue;
                    break;
                case Direction.Right:
                    if(item == Direction.Left)
                        continue;
                    break;
                case Direction.Left:
                    if(item == Direction.Right)
                        continue;
                    break;
            }

            return item;
        }

        return Direction.None;
    }
}
