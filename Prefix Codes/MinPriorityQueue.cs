//-----------------------------------------------------------------------
// <copyright file="MinPriorityQueue.cs" company="FTL">
//     FTL Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PrefixCodes
{
    using System;

    /// <summary>
    /// A minimal priority queue.
    /// </summary>
    /// <typeparam name="Item">Comparable type of the items.</typeparam>
    public class MinPriorityQueue<Item> where Item : IComparable<Item>
    {
        /// <summary>
        /// The default initial capacity
        /// </summary>
        private static readonly int InitialCapacity = 50;

        /// <summary>
        /// The binary heap organized as an array, indexing starts at 1
        /// </summary>
        private Item[] pq;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinPriorityQueue{Item}"/> class.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <exception cref="System.ArgumentException">Capacity can't be less than 1.</exception>
        public MinPriorityQueue(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentException("Capacity can't be less than 1.");
            }

            this.pq = new Item[capacity + 1];
            this.Size = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MinPriorityQueue{Item}"/> class.
        /// </summary>
        public MinPriorityQueue()
            : this(InitialCapacity)
        {
        }

        /// <summary>
        /// Gets the number of elements in the queue.
        /// </summary>
        /// <value>
        /// Size of the queue.
        /// </value>
        public int Size { get; private set; }

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns>True if there are no elements in the queue, false otherwise.</returns>
        public bool IsEmpty()
        {
            return this.Size == 0;
        }

        /// <summary>
        /// Returns the largest element without deleting it from the queue.
        /// </summary>
        /// <returns>Maximal element in the queue.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The queue may not be empty.</exception>
        public Item Peek()
        {
            if (this.IsEmpty())
            {
                throw new IndexOutOfRangeException();
            }

            return this.pq[1];
        }

        /// <summary>
        /// Adds the specified item to the queue.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Enqueue(Item item)
        {
            this.Enlarge();
            this.pq[++this.Size] = item;
            this.Swim(this.Size);
        }

        /// <summary>
        /// Returns the largest element and deletes it from the queue.
        /// </summary>
        /// <returns>The maximal element.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The queue may not be empty.</exception>
        public Item Dequeue()
        {
            if (this.IsEmpty())
            {
                throw new IndexOutOfRangeException();
            }

            Item max = this.pq[1];
            this.Exchange(1, this.Size--);
            this.Sink(1);
            this.pq[this.Size + 1] = default(Item);
            this.Shrink();
            return max;
        }

        /// <summary>
        /// Moves element at specified index up in the heap until the order is maintained.
        /// </summary>
        /// <param name="k">Index of the element.</param>
        private void Swim(int k)
        {
            while (k > 1 && this.Larger(k / 2, k))
            {
                this.Exchange(k, k / 2);
                k = k / 2;
            }
        }

        /// <summary>
        /// Moves element at specified index down in the heap until the order is maintained.
        /// </summary>
        /// <param name="k">Index of the element.</param>
        private void Sink(int k)
        {
            while (2 * k <= this.Size)
            {
                int j = 2 * k;
                if (j < this.Size && this.Larger(j, j + 1))
                {
                    j++;
                }

                if (!this.Larger(k, j))
                {
                    break;
                }

                this.Exchange(k, j);
                k = j;
            }
        }

        /// <summary>
        /// Compares elements at two indices in the heap.
        /// </summary>
        /// <param name="i">The first element.</param>
        /// <param name="j">The second element.</param>
        /// <returns>True if the first element is larger than the second one.</returns>
        private bool Larger(int i, int j)
        {
            return this.pq[i].CompareTo(this.pq[j]) > 0;
        }

        /// <summary>
        /// Exchanges items at specified indices in the heap.
        /// </summary>
        /// <param name="i">Index of the first item.</param>
        /// <param name="j">Index of the second item.</param>
        private void Exchange(int i, int j)
        {
            Item t = this.pq[i];
            this.pq[i] = this.pq[j];
            this.pq[j] = t;
        }

        /// <summary>
        /// Enlarges the array under the heap.
        /// </summary>
        private void Enlarge()
        {
            if (this.Size == this.pq.Length - 1)
            {
                Item[] largerPQ = new Item[2 * this.pq.Length];

                for (int i = 1; i <= this.Size; ++i)
                {
                    largerPQ[i] = this.pq[i];
                    this.pq[i] = default(Item);
                }

                this.pq = largerPQ;
            }
        }

        /// <summary>
        /// Shrinks the array under the heap.
        /// </summary>
        private void Shrink()
        {
            if (this.Size * 4 < this.pq.Length && this.pq.Length >= InitialCapacity * 2)
            {
                Item[] smallerPQ = new Item[this.pq.Length / 2];

                for (int i = 1; i <= this.Size; ++i)
                {
                    smallerPQ[i] = this.pq[i];
                    this.pq[i] = default(Item);
                }

                this.pq = smallerPQ;
            }
        }
    }
}