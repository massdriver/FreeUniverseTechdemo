using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Arch
{
    [Serializable]
    public class IndexArchStorage<T>
        where T: ArchObject, new()
    {
        private Dictionary<ulong, T> map { get; set; }
        private T[] index { get; set; }
        private bool locked { get; set; }

        public IndexArchStorage()
        {
            locked = false;
            map = new Dictionary<ulong, T>();
        }

        public int Count
        {
            get
            {
                return map.Count;
            }
        }

        public void BuildIndexData()
        {
            index = new T[map.Count];
            int i = 0;

            foreach (KeyValuePair<ulong, T> e in map)
            {
                e.Value.index = i;
                index[i] = e.Value;
                i++;
            }

            locked = true;
        }

        public void Add(T val)
        {
            if (locked)
                return;

            map.Add(val.id, val);
        }

        public T GetByIndex(int id)
        {
            return index[id];
        }

        public T GetByKey(ulong key)
        {
            return map[key];
        }

        public T GetByNickname(string nickname)
        {
            return map[HashUtils.StringToUINT64(nickname)];
        }
    }
}
