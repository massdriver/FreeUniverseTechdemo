using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FreeUniverse.Core
{
    public class IndexArray<T> : IEnumerable where T : class
    {
        private T[] arrayObj;
        private Stack<int> freeIds { get; set; }
        public int Capacity { get; set; }

        public IndexArray(int size)
        {
            IteratorIndex = -1;

            freeIds = new Stack<int>();
            Capacity = size;

            for (int i = 0; i < Capacity; i++)
                freeIds.Push(i);

            arrayObj = new T[size];
            Count = 0;
        }

        public T this[int id]
        {
            get { return arrayObj[id]; }
            set { arrayObj[id] = value; }
        }

        public int Count { get; private set; }

        public const int INVALID_ID = -1;

        public int Insert(T obj)
        {
            if (obj == null) throw new Exception("inserting a null object is not valid");

            if (freeIds.Count == 0) return INVALID_ID;

            int id = freeIds.Pop();

            if (arrayObj[id] != null) throw new Exception("trying to insert item into used slot");

            arrayObj[id] = obj;
            Count++;
            return id;
        }

        public T Remove(int id)
        {
            if (id < 0 || id >= Capacity) throw new Exception("trying to remove invalid id from collection");

            if (arrayObj[id] == null) return null;

            T obj = arrayObj[id];
            freeIds.Push(id);
            arrayObj[id] = null;
            Count--;
            return obj;
        }

        public int IteratorIndex { get; set; }

        public T RemoveAtIterator()
        {
            if (IteratorIndex == -1) return null;

            return Remove(IteratorIndex);
        }

        public void Remove(T obj)
        {
            int id = 0;
            foreach (T e in arrayObj)
            {
                if (e == obj)
                {
                    arrayObj[id] = null;
                    Count -= 1;
                    freeIds.Push(id);
                    return;
                }
                id++;
            }
        }

        public int freeSlot
        {
            get
            {
                return freeIds.Peek();
            }
        }

        public int Length { get { return arrayObj.Length; } }

        public void Clear()
        {
            for (int i = 0; i < arrayObj.Length; i++)
                arrayObj[i] = null;

            Count = 0;
        }

        private class IndexEnumerator : IEnumerator
        {
            IndexArray<T> idx;
            int iteratorIndex;

            public IndexEnumerator(IndexArray<T> idx)
            {
                this.idx = idx;
                iteratorIndex = -1;
                idx.IteratorIndex = iteratorIndex;
            }

            #region IEnumerator Members

            public object Current
            {
                get { return idx[iteratorIndex]; }
            }

            public bool MoveNext()
            {
                while (true)
                {
                    iteratorIndex++;
                    idx.IteratorIndex = iteratorIndex;

                    if (iteratorIndex >= idx.Capacity)
                        break;

                    if (idx[iteratorIndex] == null)
                        continue;

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                idx.IteratorIndex = iteratorIndex;
                iteratorIndex = -1;
            }

            #endregion
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new IndexEnumerator(this);
        }

        #endregion
    }
}
