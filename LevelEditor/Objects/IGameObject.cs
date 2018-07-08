using System;

namespace LevelEditor.Objects
{
    interface IGameObject
    {
        String Name { get; set; }

        bool HasSoundEmitter { get; set; }

        string Save();
        bool Load();
    }
}
