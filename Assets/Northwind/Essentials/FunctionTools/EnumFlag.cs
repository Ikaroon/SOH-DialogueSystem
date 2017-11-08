using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials
{
    public static class EnumFlag
    {

        public static void SetFlag(int a, int b)
        {
            a |= b;
        }

        public static void UnsetFlag(int a, int b)
        {
            a &= ~b & a;
        }

        public static bool HasFlag(int a, int b)
        {
            return (a & b) != 0;
        }

        public static void ToogleFlag(int a, int b)
        {
            a ^= b;
        }

    }
}