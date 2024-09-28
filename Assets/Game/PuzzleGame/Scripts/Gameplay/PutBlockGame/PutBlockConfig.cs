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
            6, 10, 8, 12, 10, 8, 7, 7, 8, 8, 8, 8, 2, 16, 7,
        };
    }
}
