using System;
using System.Linq;
using System.Collections.Generic;

namespace CircuitSimulator
{
    public class PriorityQueue<T>
    {
        List<T> list = new List<T>();
        IComparer<T> comp = Comparer<T>.Default;
        class Comparer : IComparer<T>
        {
            Comparison<T> comparison;
            public Comparer(Comparison<T> comparison) { this.comparison = comparison; }
            public int Compare(T x, T y) { return comparison(x, y); }
        }
        public PriorityQueue() { }
        public PriorityQueue(Comparison<T> comp) { this.comp = new Comparer(comp); }
        public void Enqueue(T item) { int i = list.BinarySearch(item, comp); list.Insert(i < 0 ? ~i : i, item); }
        public T Dequeue() { T r = list[0]; list.RemoveAt(0); return r; }
        public T Peek() { return list[0]; }
        public int Count { get { return list.Count; } }
        public T this[int i] { get { return list[i]; } set { list[i] = value; } }
        public bool Any() { return list.Any(); }
    }

}
