using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace LevelEditor.Collision
{
    [Serializable()]
    public class TreeNode<T>: ISerializable
    {

        public Rectangle mBoundary;
        public int mCapacity;
        public int mMaxDepth;
        public List<TreeData<T>> mTreeData;

        private bool mIsSubDivided;

        private TreeNode<T> mNorthWest;
        private TreeNode<T> mNorthEast;
        private TreeNode<T> mSouthWest;
        private TreeNode<T> mSouthEast;

        public TreeNode(Rectangle boundary, int capacity, int maxDepth)
        {

            mBoundary = boundary;
            mCapacity = capacity;
            mMaxDepth = maxDepth;

            mTreeData = new List<TreeData<T>>();

            mIsSubDivided = false;

        }

        public bool Insert(TreeData<T> data)
        {

            if (!data.mAxisAlignedRectangle.Intersects(mBoundary))
            {
                return false;
            }

            if (mTreeData.Count < mCapacity)
            {
                mTreeData.Add(data);
                return true;
            }

            if (!mIsSubDivided && mMaxDepth > 0)
            {
                SubDivide();
                mIsSubDivided = true;
            }

            if (mMaxDepth == 0)
            {
                return false;
            }

            var intersects = mNorthWest.Insert(data);
            intersects = mNorthEast.Insert(data) || intersects;
            intersects = mSouthWest.Insert(data) || intersects;
            intersects = mSouthEast.Insert(data) || intersects;

            return intersects;

        }

        public void Remove(TreeData<T> data)
        {

            if (!data.mAxisAlignedRectangle.Intersects(mBoundary))
            {
                return;
            }

            var index = mTreeData.FindIndex(ele => ele.mT.Equals(data.mT));

            if (index >= 0 && index < mTreeData.Count)
            {
                mTreeData.RemoveAt(index);
                return;
            }

            if (!mIsSubDivided)
            {
                return;
            }

            mNorthWest.Remove(data);
            mNorthEast.Remove(data);
            mSouthWest.Remove(data);
            mSouthEast.Remove(data);

            if (mNorthWest.mTreeData.Count == 0 && !mNorthWest.mIsSubDivided &&
                mNorthEast.mTreeData.Count == 0 && !mNorthEast.mIsSubDivided &&
                mSouthWest.mTreeData.Count == 0 && !mSouthWest.mIsSubDivided &&
                mSouthEast.mTreeData.Count == 0 && !mSouthEast.mIsSubDivided)
            {
                mNorthWest = null;
                mNorthEast = null;
                mSouthWest = null;
                mSouthEast = null;
                mIsSubDivided = false;
            }

        }

        public List<T> QueryRectangle(Rectangle rectangle)
        {

            var intersections = new List<T>();

            if (!rectangle.Intersects(mBoundary))
            {
                return intersections;
            }

            foreach (var data in mTreeData)
            {
                if (data.mAxisAlignedRectangle.Intersects(rectangle))
                {
                    intersections.Add(data.mT);
                }
            }

            if (mIsSubDivided)
            {
                intersections.AddRange(mNorthWest.QueryRectangle(rectangle));
                intersections.AddRange(mNorthEast.QueryRectangle(rectangle));
                intersections.AddRange(mSouthWest.QueryRectangle(rectangle));
                intersections.AddRange(mSouthEast.QueryRectangle(rectangle));
            }

            return intersections;

        }

        private void SubDivide()
        {

            var halfWidth = mBoundary.Width / 2;
            var halfHeight = mBoundary.Height / 2;

            var boundNorthWest = new Rectangle(mBoundary.X, mBoundary.Y, halfWidth, halfHeight);
            var boundNorthEast = new Rectangle(mBoundary.X + halfWidth, mBoundary.Y, halfWidth, halfHeight);
            var boundSouthWest = new Rectangle(mBoundary.X, mBoundary.Y + halfHeight, halfWidth, halfHeight);
            var boundSouthEast = new Rectangle(mBoundary.X + halfWidth, mBoundary.Y + halfHeight, halfWidth, halfHeight);

            mNorthWest = new TreeNode<T>(boundNorthWest, mCapacity, mMaxDepth - 1);
            mNorthEast = new TreeNode<T>(boundNorthEast, mCapacity, mMaxDepth - 1);
            mSouthWest = new TreeNode<T>(boundSouthWest, mCapacity, mMaxDepth - 1);
            mSouthEast = new TreeNode<T>(boundSouthEast, mCapacity, mMaxDepth - 1);

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("mBoundary", mBoundary);
            info.AddValue("mCapacity", mCapacity);
            info.AddValue("mMaxDepth", mMaxDepth);
            info.AddValue("mTreeData", mTreeData);
            info.AddValue("mIsSubDivided", mIsSubDivided);
            info.AddValue("mNorthWest", mNorthWest);
            info.AddValue("mNorthEast", mNorthEast);
            info.AddValue("mSouthWest", mSouthWest);
            info.AddValue("mSouthEast", mSouthEast);
        }

        public TreeNode(SerializationInfo info, StreamingContext context)
        {
            mBoundary = (Rectangle)info.GetValue("mBoundary", typeof(Rectangle));
            mCapacity = (int)info.GetValue("mCapacity", typeof(int));
            mMaxDepth = (int)info.GetValue("mMaxDepth", typeof(int));
            mTreeData = (List<TreeData<T>>)info.GetValue("mTreeData", typeof(List<TreeData<T>>));
            mIsSubDivided = (bool)info.GetValue("mIsSubDivided", typeof(bool));
            mNorthWest = (TreeNode<T>)info.GetValue("mNorthWest", typeof(TreeNode<T>));
            mNorthEast = (TreeNode<T>)info.GetValue("mNorthEast", typeof(TreeNode<T>));
            mSouthWest = (TreeNode<T>)info.GetValue("mSouthWest", typeof(TreeNode<T>));
            mSouthEast = (TreeNode<T>)info.GetValue("mSouthEast", typeof(TreeNode<T>));
        }

        public TreeNode()
        {
            mTreeData = new List<TreeData<T>>();

            mIsSubDivided = false;
        }
    }

}
