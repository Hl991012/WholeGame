namespace PuzzleGame.Gameplay
{
    public interface IGameState<T> where T : GameStateBaseModel
    {
        T GameState { get; }
    }
}
