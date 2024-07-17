using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class BytesUtils
    {
        public static byte[] RemoveRange(this byte[] bytes, int index, int count)
        {
            List<byte> result = new List<byte>();
            result.RemoveRange(index, count);
            return result.ToArray();
        }
    }
}
