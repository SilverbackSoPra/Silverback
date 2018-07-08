namespace LevelEditor.Objects
{
    interface IEscape
    {
        void Escape();

        int HP { get; set; }
        bool IsAlive { get; set; }
    }
}
