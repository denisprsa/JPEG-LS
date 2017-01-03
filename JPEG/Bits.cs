using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    class Bits
    {
        public List<byte> bytes = new List<byte>();
        public List<bool> bits = new List<bool>();

        public void add(bool value)
        {
            this.bits.Add(value);
        }

    }
}
