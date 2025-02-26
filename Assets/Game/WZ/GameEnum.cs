public enum Direction
{
    None,
    Left,
    Right,
    Up,
    Down,
}

public enum BoosterType
{
    None = -1,
    Undo = 0,
    ClearBrick = 1,
    ClearNumber = 2,
    Explosion = 3,
    RemoveFigure = 4,
    ClearHorizontalLine = 5,
    ClearVerticalLine = 6,
    
    Refresh = 10,
    Help = 11,
    RemoveHorizontal = 12,
    RemoveVertical = 13,
}

