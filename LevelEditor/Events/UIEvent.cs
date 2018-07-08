using System;
using System.Collections.Generic;

namespace LevelEditor.Events
{
    class UiEvent
    {
        private static List<Func<string, string>> sMember = new List<Func<string, string>>();

        public static void Remove(Func<string, string> f)
        {
            sMember.Remove(f);
        }

        public static void Add(Func<string, string> f)
        {
            sMember.Add(f);
        }

        public static void Call(string a)
        {
            foreach(var f in sMember)
            {
                f(a);
            }
        }
    }
}
