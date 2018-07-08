using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace LevelEditor.Collision
{
    class TreeData<T>
    {

        public T mT;
        public Rectangle mAxisAlignedRectangle;

        public TreeData(T t, Rectangle rectangle)
        {
            mT = t;
            mAxisAlignedRectangle = rectangle;
        }

    }
}
