using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;

namespace LevelEditor.Collision
{
    class QuadTree<T>
    {

        private TreeNode<T> mRootNode;

        public QuadTree(Rectangle boundary, int capacity, int maxDepth)
        {

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

    }
}
