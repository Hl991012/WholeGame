namespace PuzzleGame.Gameplay
{
    public interface IGameState<T> where T : GameStateModel
    {
        T GameState { get; }
    }
}
