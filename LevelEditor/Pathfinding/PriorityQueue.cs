using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LevelEditor.Pathfinding
{
    public sealed class PriorityQueue
    {
        private readonly List<PriorityQueueItem> mItems;
        
        private readonly Func<Node, Node, bool> mCompare;

        private readonly HashSet<Node> mHashSet;

        public int Count => mItems.Count - 1;

        public PriorityQueue(Func<Node, Node, bool> compare)
        {
            mItems = new List<PriorityQueueItem> {new PriorityQueueItem(null, 0)};
            mCompare = compare;
            mHashSet = new HashSet<Node>();
        }

        public void Insert(Node value)
        {
            var item = new PriorityQueueItem(value, mItems.Count);
            mItems.Add(item);
            mHashSet.Add(value);
            RepairHeapUpwards(item.mHeapIndex);
        }

        public void InsertRange(IEnumerable<Node> values)
        {
            foreach (var value in values)
            {
                Insert(value);
            }
        }

        private void RepairHeapUpwards(int heapIndex)
        {
            while (heapIndex > 1)
            {
                var j = (int) heapIndex / 2;
                if (mCompare(mItems[heapIndex].mValue, mItems[j].mValue))
                {
                    Swap(heapIndex, j);
                }

                heapIndex = j;
            }
        }

        private void Swap(int i, int j)
        {
            var tmp = mItems[i];
            var tmpIndex = mItems[j].mHeapIndex;
            mItems[i] = mItems[j];
            mItems[i].mHeapIndex = tmp.mHeapIndex;
            mItems[j] = tmp;
            mItems[j].mHeapIndex = tmpIndex;
        }

        public Node Top()
        {
            return Count == 0 ? null : mItems[1].mValue;
        }

        public void Pop()
        {
            if (Count == 0)
            {
                return;
            }
            Swap(1, Count);
            mHashSet.Remove(mItems[Count].mValue);
            mItems.RemoveAt(Count);
        }

        private void RepairHeapDownWards(int heapIndex)
        {
            while (heapIndex * 2 < mItems.Count)
            {
                var j = heapIndex * 2;
                if (j + 1 < mItems.Count && mCompare(mItems[j + 1].mValue, mItems[j].mValue))
                {
                    j = heapIndex * 2 + 1;
                }

                if (mCompare(mItems[j].mValue, mItems[heapIndex].mValue))
                {
                    Swap(heapIndex, j);
                }
            }
        }

        public bool Empty()
        {
            return Count == 0;
        }

        public bool Contains(Node elem)
        {
            // This might not be ideal yet, but it works 
            return mItems.FindIndex(e => e.mValue.Equals(elem)) >= 0;
        }

        public void UpdateElement(Node element, float value, Action<Node, float> updateElement)
        {
            var tmpStorage = new List<Node>();
            while (!element.Equals(Top()))
            {
                if (Empty())
                {
                    InsertRange(tmpStorage);
                    return;
                }
                tmpStorage.Add(Top());
                Pop();
            }

            updateElement(Top(), value);
            InsertRange(tmpStorage);
        }

        public Node Find(Func<Node, Vertex, bool> compare, Vertex compareObj)
        {
            /*
            var value = mItems.Find(elem =>
            {
                if (elem.mValue == null)
                {
                    return false;
                }
                var cmp = compare(elem.mValue, compareObj);
                return cmp;
            });
            if (value != null)
            {
                var debug =  value;
                return debug.mValue;
            }

            return default(T);
            */
            
            for (var i = 1; i < mItems.Count; i++)
            {
                if (mItems[i].mValue == null)
                {
                    continue;
                }
                if (compare(mItems[i].mValue, compareObj))
                {
                    return mItems[i].mValue;
                }
            }

            return null;
            
        }

        private sealed class PriorityQueueItem
        {
            public readonly Node mValue;

            public int mHeapIndex;

            public PriorityQueueItem(Node value, int heapIndex)
            {
                mValue = value;
                mHeapIndex = heapIndex;
            }
        }
    }
}
