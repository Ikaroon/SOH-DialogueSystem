using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    public static class Iterator
    {
        //USAGE:
        //Tools.Iterate(new IntVector3(lengthX, lengthY, lengthZ), delegate(int x, int y, int z) {
        //      field[x, y, z] = v;
        //    } );
        /// <summary>
        /// Iterates over an intVector3 and calls the given callback function
        /// </summary>
        /// <param name="size">The 3D dimensions of the iteration</param>
        /// <param name="callback">The callback function simply use: delegate(int x, int y, int z) { ... }</param>
        public static void Iterate(IntVector3 size, System.Action<int, int, int> callback = null)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        if (callback != null)
                        {
                            callback.Invoke(x, y, z);
                        }
                    }
                }
            }
        }

        //USAGE:
        //Tools.Iterate(new IntVector3(lengthX, lengthY), delegate(int x, int y) {
        //      field[x, y] = v;
        //    } );
        /// <summary>
        /// Iterates over an intVector2 and calls the given callback function
        /// </summary>
        /// <param name="size">The 2D dimensions of the iteration</param>
        /// <param name="callback">The callback function simply use: delegate(int x, int y) { ... }</param>
        public static void Iterate(IntVector2 size, System.Action<int, int> callback = null)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    if (callback != null)
                    {
                        callback.Invoke(x, y);
                    }
                }
            }
        }

        //USAGE:
        //Tools.Iterate(length, delegate(int x) {
        //      field[x] = v;
        //    } );
        /// <summary>
        /// Iterates over length and calls the given callback function
        /// </summary>
        /// <param name="length">The length of the iteration</param>
        /// <param name="callback">The callback function simply use: delegate(int x) { ... }</param>
        public static void Iterate(int length, System.Action<int> callback = null)
        {
            for (int x = 0; x < length; x++)
            {
                if (callback != null)
                {
                    callback.Invoke(x);
                }
            }
        }
    }
}