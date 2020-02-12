using System;

namespace LevelEditor.Objects
{
    interface IGameObject : ISaver, ILoader
    {
        String Name { get; set; }

        bool HasSoundEmitter { get; set; }
    }
}
