namespace GameFrame
{
    public interface IRecyclable
    {
        RecyclableType RecyclableType { get; }
        void OnGet();
        void OnReturn();
    }

    public enum RecyclableType
    {
        NormalBullet,
        NormalSubBullet,
        Enemy,
    }
}