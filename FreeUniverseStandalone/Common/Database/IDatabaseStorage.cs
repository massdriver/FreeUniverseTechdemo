using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Common.Database
{
    public interface IDatabaseStorage<T>
    where T : DatabaseElement<T>, new()
    {
        int Insert(T element);
        int Insert(List<T> elements);
        int Remove(ulong key);
        int Remove(List<ulong> keys);
        int Update(T element);
        int Update(List<T> elements);
        int Get(ulong key, out T element);
        int Get(List<ulong> keys, ref Dictionary<ulong, T> elements);
        int Mark(ulong key, uint value);
        int GetMark(ulong key, out uint value);
        int SetParent(ulong key, ulong parent);
        int GetParent(ulong key, out ulong parent);
        int GetByParent(ulong parent, ref Dictionary<ulong, T> elements);
    }
}
