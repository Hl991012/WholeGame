namespace PuzzleGame.Gameplay.Puzzle1010
{
    public class PutBlockConfig
    {
        public static readonly int[][,] Figures =
        {
            new[,]
            {
                {1}
            },
            new [,]
            {
                {1, 1}
            },
            new [,]
            {
                {1, 1, 1}
            },
            new[,]
            {
                {1, 1, 1, 1}
            },
            new[,]
            {
                {1, 0},
                {1, 1}
            },
            new[,]
            {
                {1, 1},
                {1, 1}
            },
            new[,]
            {
                {1, 0, 0},
                {1, 1, 1}
            },
            new[,]
            {
                {0, 0, 1},
                {1, 1, 1}
            },
            new[,]
            {
                {0, 1, 1},
                {1, 1, 0}
            },
            new[,]
            {
                {1, 1, 0},
                {0, 1, 1}
            },
            new[,]
            {
                {0, 1, 0},
                {1, 1, 1}
            },
            new[,]
            {
                {1, 1, 1},
                {0, 1, 0}
            },
            new[,]
            {
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1}
            },
            new[,]
            {
                {1, 0},
                {0, 1},
            },
            new[,]
            {
                {1, 0, 0},
                {0, 1, 0},
                {0, 0, 1},
            }
        };

        public static readonly int[] FiguresProbability = 
        {
            6, 16, 10, 15, 5, 5, 5, 5, 8, 8, 5, 5, 1, 10, 3,
        };
    }
}
