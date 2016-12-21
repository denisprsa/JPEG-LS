using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    public class JPEG
    {
        private int NEAR;
        private int MAXVAL;
        private int RANGE;
        private int[] N;
        private int[] A;
        private int[] B;
        private int[] C;
        private int RUNindex;
        private int[] J;
        private int qbpp;
        private int bpp;
        private int LIMIT;

        /**
         *  NAPOLNIMO ARRAY Z DOLOCENIMI VREDNOSTMI
         * 
         */
        private void Populate(this int[] arr, int value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        public void init()
        {
            this.N = new int[367];
            this.A = new int[367];
            this.B = new int[365];
            this.C = new int[365];
            this.Populate(this.N, 1);
            this.Populate(this.B, 0);
            this.Populate(this.C, 0);
            this.RUNindex = 0;
            this.J = new int[] {
                0, 0, 0, 0,
                1, 1, 1, 1,
                2, 2, 2, 2,
                3, 3, 3, 3,
                4, 4, 5, 5,
                6, 6, 7, 7,
                8, 9,10,11,
                12,13,14,15
            };
            this.RANGE = 256;
            this.MAXVAL = 255;
            this.qbpp = (int) Math.Log(this.RANGE, 2);
            this.bpp = 8; // max(2, 8) -> nas primer
        }


        private void RunModeProcessing()
        {

        }

        private void RegularModeProcessing()
        {

        }

        private void GetNextSample()
        {

        }

        private void Quantize(int i)
        {

        }

        private void ModRange(int i)
        {

        }

        private void ComputeRx()
        {

        }
    }
}
