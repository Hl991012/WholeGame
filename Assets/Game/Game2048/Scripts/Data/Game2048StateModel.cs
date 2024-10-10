public class Game2048StateModel
{
    //特定位置特定值的磁贴->二维数组
    public int[,] tailValues = new int[4, 4];
    //修正撤销时的分数
    public int curScore;

    public int bestScore;
}