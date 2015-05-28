using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FreeUniverse.Core
{
    public class ListEx<T> : IEnumerable
        where T : class
    {
        class Element
        {
            public T obj;
            public Element next;
        }

        Element _head;

        public void Clear()
        {
            T e = Iterate();
            while (e != null)
            {
                Remove();
                e = Next();
            }

            _head = null;
            _count = 0;
            _iterPrev = null;
            _iterCurrent = null;
            _iterNext = null;
        }

        public void Add(T obj)
        {
            if (_head == null)
            {
                _head = new Element();
                _head.obj = obj;
                _head.next = null;
            }
            else
            {
                Element newHead = new ListEx<T>.Element();
                newHead.next = _head;
                newHead.obj = obj;
                _head = newHead;
            }

            _count++;
        }

        int _count;
        public int count { get { return _count; } }

        Element _iterPrev;
        Element _iterCurrent;
        Element _iterNext;

        public T Iterate()
        {
            if (_head == null)
                return null;

            _iterPrev = null;
            _iterCurrent = _head;
            _iterNext = _iterCurrent.next;

            return _iterCurrent.obj;
        }

        public T Next()
        {
            if (_iterNext == null)
                return null;

            _iterPrev = _iterCurrent;
            _iterCurrent = _iterNext;
            _iterNext = _iterCurrent.next;

            return _iterCurrent.obj;
        }

        // MH: do not forget to call Next() after remove
        public T Remove()
        {
            if (_head == null)
                return null;

            if (_iterCurrent == _head)
            {
                T obj = _iterCurrent.obj;

                if (_iterCurrent.next != null)
                {
                    _head = _iterCurrent.next;
                }
                else
                {
                    _head = null;
                    _iterPrev = null;
                    _iterCurrent = null;
                    _iterNext = null;
                }

                _count--;
                return obj;
            }
            T e = _iterCurrent.obj;

            _iterCurrent = _iterPrev;
            _iterPrev.next = _iterNext;
            _iterNext = _iterCurrent.next;
            _count--;
            return e;
        }

        class ListExEnumerator : IEnumerator
        {
            ListEx<T> _lst;
            T _current;

            public ListExEnumerator(ListEx<T> lst)
            {
                _lst = lst;
            }

            public object Current
            {
                get{ return _current; }
            }

            public bool MoveNext()
            {
                if (_lst.count == 0)
                    return false;

                if (_current == null)
                {
                    _current = _lst.Iterate();
                    if (_current == null) return false;
                    return true;
                }

                _current = _lst.Next();
                if( _current == null ) return false;

                return true;
            }

            public void Reset()
            {
                _current = null;
            }
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ListExEnumerator(this);
        }

        #endregion
    }
}
