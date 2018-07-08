namespace LevelEditor.Objects
{
    interface IAttacker
    {
        void Attack();

        void Attacked(int damage);

        bool IsApe { get; set; }
    }
}
