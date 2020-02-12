using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace LevelEditor.Collision
{
    [Serializable()]
    public class QuadTree<T>: ISerializable
    {

        public TreeNode<T> mRootNode;
        public Rectangle mBoundary;
        public int mCapacity;
        public int mMaxDepth;

        public QuadTree(Rectangle boundary, int capacity, int maxDepth)
        {
            mBoundary = boundary;
            mCapacity = capacity;
            mMaxDepth = maxDepth;
            mRootNode = new TreeNode<T>(boundary, capacity, maxDepth);

        }

        public bool Insert(T t, Rectangle rectangle)
        {

            var data = new TreeData<T>(t, rectangle);
            return mRootNode.Insert(data);

        }

        public void Remove(T t, Rectangle rectangle)
        {

            var data = new TreeData<T>(t, rectangle);
            mRootNode.Remove(data);

        }

        public List<T> QueryRectangle(Rectangle rectangle)
        {

            return mRootNode.QueryRectangle(rectangle);

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mRootNode", mRootNode);
        }

        public QuadTree(SerializationInfo info, StreamingContext context)
        {
            mRootNode = (TreeNode<T>)info.GetValue("mRootNode", typeof(TreeNode<T>));
        }

        public QuadTree()
        {
            mRootNode = new TreeNode<T>(mBoundary, mCapacity, mMaxDepth);
        }
    }
}
