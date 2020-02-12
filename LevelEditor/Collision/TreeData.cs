using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace LevelEditor.Collision
{
    [Serializable]
    public class TreeData<T>: ISerializable
    {

        public T mT;
        public Rectangle mAxisAlignedRectangle;

        public TreeData(T t, Rectangle rectangle)
        {
            mT = t;
            mAxisAlignedRectangle = rectangle;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mT", mT);
            info.AddValue("mAxisAlignedRectangle", mAxisAlignedRectangle);
        }

        public TreeData(SerializationInfo info, StreamingContext context)
        {
            mT = (T)info.GetValue("mT", typeof(T));
            mAxisAlignedRectangle = (Rectangle)info.GetValue("mAxisAlignedRectangle", typeof(Rectangle));
        }
        public TreeData() { }
    }
}
