using System;

namespace LevelEditor.Engine.Helper
{
    internal sealed class EngineInvalidParameterException : Exception
    {

        public EngineInvalidParameterException(string message) : base(message)
        {

        } 

    }
}
