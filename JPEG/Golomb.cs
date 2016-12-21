using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    class Golomb
    {
        /**
         *  KODIRANJE
         * 
         */
        public byte[] Encode(byte[] data, int M)
        {
            List<byte> _out = new List<byte>();
            for(int i = 0; i < data.Length; i++)
            {
                // CALCULATE Q
                int q = data[i] / M;

                for (int j = 0; j < q; j++)
                {
                    _out.Add(1);
                }
                _out.Add(0);

                // CALCULATE V 
                int v = 1;
                for (int j = 0; j < Math.Log(M,2); j++)
                {
                    _out.Add((byte)(v & data[i]));
                    v = v << 1;
                }
            }
            return _out.ToArray();
        }


        /**
         * 
         *  DEKODIRANJE
         * 
         */
        public void Decode(byte[] data, int M)
        {
            int q = 0;
            int nr = 0;

            for(int i = 0; i < data.Length; i++)
            {
                q = 0;
                nr = 0;


            }
        }
    }
}
