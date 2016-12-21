using System;
using System.Collections.Generic;
using System.Drawing;
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
        private int qbpp;
        private int bpp;
        private int LIMIT;
        private int RUNindex;
        private int N_MAX;
        private int C_MIN;
        private int C_MAX;

        private int[] N;
        private int[] A;
        private int[] B;
        private int[] C;
        private int[] J;

        private int a;
        private int b;
        private int c;
        private int d;
        private int x;
        private int D1;
        private int D2;
        private int D3;
        

        /**
         *  NAPOLNIMO ARRAY Z DOLOCENIMI VREDNOSTMI
         * 
         * 
         */
        private void Populate(int[] arr, int value)
        {
            for (int i = 0; i < arr.Length; i++)
                arr[i] = value;
        }

        /**
         *  INIT DATA
         * 
         * 
         */
        private void init()
        {
            this.N = new int[367];
            this.A = new int[367];
            this.B = new int[365];
            this.C = new int[365];
            this.Populate(this.N, 1);
            this.Populate(this.B, 0);
            this.Populate(this.C, 0);
            //  PO ENCABI DOBIMO 4
            this.Populate(this.A, 4);
            this.N[365] = 0;
            this.N[366] = 0;
            this.RUNindex = 0;
            this.J = new int[] {
                0, 0, 0, 0,
                1, 1, 1, 1,
                2, 2, 2, 2,
                3, 3, 3, 3,
                4, 4, 5, 5,
                6, 6, 7, 7,
                8, 9, 10, 11,
                12, 13, 14, 15
            };
            this.RANGE = 256;
            this.MAXVAL = 255;
            this.LIMIT = 2 * (bpp + bpp);
            this.qbpp = (int)Math.Log(this.RANGE, 2);
            // max(2, 8) -> nas primer
            this.bpp = 8;
        }

        /**
         *  JPEG-LS ENCODER
         *  
         *  @image  : string
         *  @n      : int
         *  @return : byte[]
         */
        public byte[] Encode(string image, int n = 0)
        {
            Bitmap file = new Bitmap(image);
            this.init();
            int x = 0;
            int y = 0;



            return new byte[1];
        }

        /**
         *  RETURN MAX VALUE BETWEEN TWO VALUES
         * 
         *  @i      : int
         *  @j      : int
         *  @return : int
         */
        private int max(int i, int j)
        {
            return (i < j) ? j : i;
        }

        private int min(int i, int j)
        {
            return (i < j) ? i : j;
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
