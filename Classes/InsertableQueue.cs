using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piealytics
{
    /// <summary>
    /// A threadsafe queue. Items can be inserted at a desired position in the queue at any time
    /// </summary>
    /// <typeparam name="T">The data type of objects to store in the queue</typeparam>
    class InsertableQueue<T>
    {

        // the length of the queue is the array length of data
        public int Length { get { return data.Length; } private set { } }

        private T[] data;
        private int first = 0;

        /// <summary>
        /// Creates a new insertable queue
        /// </summary>
        /// <param name="length">The length of the queue</param>
        public InsertableQueue(int length)
        {
            data = new T[length];
        }

        /// <summary>
        /// Resizes the queue
        /// </summary>
        /// <param name="size"></param>
        public void Resize( int size )
        {

            // lock mutex
            lock(data)
            {

                // the array to return
                var oldData = new T[Length];

                // only one copy operation if index is zero
                if (first == 0)
                {
                    Array.Copy(data, oldData, Length);
                }
                else
                {
                    Array.Copy(data, first, oldData, 0, Length - first);
                    Array.Copy(data, 0, oldData, Length - first, first);
                }
                first = 0;

                // copy to new resized data array
                var newData = new T[size];
                Array.Copy(oldData, newData, Math.Min(size, oldData.Length));
                data = newData;

            }

        }

        /// <summary>
        /// Adds a single Item to the queue and overrides an item if the queue is full
        /// </summary>
        /// <param name="item">The new data object</param>
        public void AddItem(T item)
        {

            lock(data)
            {
                data[first] = item;
                first = (first + 1) % Length;
            }

        }

        /// <summary>
        /// Sets an item at a specific position relative to the oldest one. 0 is the oldest one.
        /// </summary>
        /// <param name="item">The item to set</param>
        /// <param name="position">The position to set the item at</param>
        public void SetItem(T item, int position)
        {

            lock (data)
            {

                position = position % Length;
                data[position] = item;
                if (position + 1 == Length)
                {
                    first = 0;
                }
                else
                {
                    first = Math.Max(first, position + 1);
                }

            }

        }

        /// <summary>
        /// Gets the whole queue as array sorted with the oldest item at index 0
        /// </summary>
        /// <returns>The queue as array</returns>
        public T[] GetAllItems()
        {

            lock(data)
            {

                // the array to return
                var res = new T[Length];

                // only one copy operation if index is zero
                if (first == 0)
                {
                    Array.Copy(data, res, Length);
                }
                else
                {
                    Array.Copy(data, first, res, 0, Length - first);
                    Array.Copy(data, 0, res, Length - first, first);
                }

                return res;

            }

        }

        /// <summary>
        /// Private helper function to calculate negative modulo.
        /// </summary>
        /// <param name="k">First number</param>
        /// <param name="n">Second number</param>
        /// <returns></returns>
        private int mod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }

    }
}
