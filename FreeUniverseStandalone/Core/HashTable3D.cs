using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FreeUniverse.Core
{
    public class HashTable3D<T>
    {
        private class Cell
        {
            public List<T> items { get; set; }

            public Cell()
            {
                items = new List<T>();
            }

            public void Add(T obj)
            {
                items.Add(obj);
            }

            public bool Remove(T obj)
            {
                return items.Remove(obj);
            }

            public int Count
            {
                get { return items.Count; }
            }
        }

        private Cell[] hashTable { get; set; }
        public uint tableSize { get; private set; }
        public uint cellSize { get; private set; }
        public int totalItems { get; private set; }

        public HashTable3D(uint cellSize, uint tableSize)
        {
            this.cellSize = cellSize;
            this.tableSize = tableSize;

            hashTable = new Cell[tableSize];

            for (int i = 0; i < hashTable.Length; i++)
                hashTable[i] = new Cell();
        }

        public void Remove(Vector3 position, T obj)
        {
            if (hashTable[MakePositionHash(position)].Remove(obj))
                totalItems--;
        }

        public void Add(Vector3 position, T obj)
        {
            hashTable[MakePositionHash(position)].Add(obj);
            totalItems++;
        }

        public int CountAtPosition(Vector3 position)
        {
            return hashTable[MakePositionHash(position)].Count;
        }

        public uint MakePositionHash(Vector3 pos)
        {
            return ((uint)(pos.x / cellSize) * 73856093) ^ ((uint)(pos.y / cellSize) * 19349663) ^ ((uint)(pos.z / cellSize) * 83492791) % tableSize; 
        }
    }
}
