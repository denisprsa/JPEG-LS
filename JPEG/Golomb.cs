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
        public void Encode(ref Bits byteManager, int k, int MErrval, int limit, int qbpp)
        {
            int M = Convert.ToInt32(Math.Pow(2, k));
            int x = Convert.ToInt32(Math.Floor(Convert.ToDouble(MErrval / M)));
            int r = Convert.ToInt32(MErrval % M);

            if(x < limit - qbpp - 1)
            {
                for (int i = 0; i < x; i++)
                    byteManager.add(false);

                byteManager.add(true);

                int comp = Convert.ToInt32(Math.Pow(2, k-1));
                for (int i = k; i > 0; i--)
                {
                    if ((comp & r) == comp)
                        byteManager.add(true);
                    else
                        byteManager.add(false);

                    r <<= 1;
                }
            }
            else
            {
                for (int i = 0; i < limit - qbpp - 1; i++)
                    byteManager.add(false);

                byteManager.add(true);

                MErrval -= 1;
                for (int i = qbpp; i > 0; i--)
                {
                    if ((128 & MErrval) == 128)
                        byteManager.add(true);
                    else
                        byteManager.add(false);

                    MErrval <<= 1;
                }
            }
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
