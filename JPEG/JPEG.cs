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
        private int T1;
        private int T2;
        private int T3;
        private int Q1;
        private int Q2;
        private int Q3;

        // IMAGE
        private int posX = 0;
        private int posY = 0;
        private Bitmap image;
        private int layers;
        private int width;
        private int height;
        private byte[] LD;


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
            this.C_MAX = 127;
            this.C_MIN = -128;
            this.N = new int[367];
            this.A = new int[367];
            this.B = new int[365];
            this.C = new int[365];
            this.Populate(this.N, 1);
            this.Populate(this.B, 0);
            this.Populate(this.C, 0);
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
            this.bpp = 8;
            this.T1 = 3;
            this.T2 = 7;
            this.T3 = 21;

            this.GetNextSample();
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
            this.image = new Bitmap(image);
            this.init();

            this.GetNextSample();
            if (this.D1 == 0 && this.D2 == 0 && this.D3 == 0)
                this.RunModeProcessing();
            else
                this.RegularModeProcessing();

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

        /**
         *  RETURN MIN VALUE BETWEEN TWO VALUES
         * 
         *  @i      : int
         *  @j      : int
         *  @return : int
         */
        private int min(int i, int j)
        {
            return (i < j) ? i : j;
        }

        /**
         *  RUN MODE
         * 
         * 
         */
        private void RunModeProcessing()
        {
            
        }

        /**
         * 
         * 
         * 
         */
        private void SetVariablesABCD()
        {
            // SET a
            if (posX == 0 && posY == 0)
                this.a = 0;
            else if (posY > 0 && posX == 0)
                this.a = this.LD[(posY - 1) * width];
            else if (posY == 0 && posX > 0)
                this.a = this.LD[posX - 1];
            else
                this.a = this.LD[((posY - 1) * width) + posX - 1];

            // SET b
            if (posY == 0)
                this.b = 0;
            else
                this.b = this.LD[((posY - 1) * width) + posX];

            // SET c
            if (posY == 0 || (posY == 1 && posX == 0))
                this.c = 0;
            else if (posY > 1 && posX == 0)
                this.c = this.LD[(posY - 2) * width];
            else
                this.c = this.LD[((posY - 1) * width) + posX - 1];

            // SET d
            if (posY == 0)
                this.d = 0;
            else if (posY > 0 && posX == width - 1)
                this.d = this.LD[(posY - 1) * width - 1];
            else
                this.d = this.LD[((posY - 1) * width) + posX + 1];
        }

        /**
         *  CALCULATE QUANTIZATION GRADIENTS
         * 
         *  
         */
        private void QuantizationOfGradients()
        {
            int[] Qi = new int[3];
            for(int i = 0; i < 3; i++)
            {
                int Di = 0;
                if (i == 0)
                    Di = D1;
                else if (i == 1)
                    Di = D2;
                else
                    Di = D3;

                if (Di <= -T3) Qi[i] = -4;
                else if (Di <= -T2) Qi[i] = -3;
                else if (Di <= -T1) Qi[i] = -2;
                else if (Di < -NEAR) Qi[i] = -1;
                else if (Di <= NEAR) Qi[i] = 0;
                else if (Di < T1) Qi[i] = 1;
                else if (Di < T2) Qi[i] = 2;
                else if (Di < T3) Qi[i] = 3;
                else Qi[i] = 4;
            }
            Q1 = Qi[0];
            Q2 = Qi[1];
            Q3 = Qi[2];
        }

        private void RegularModeProcessing()
        {
            
        }

        private void GetNextSample()
        {
            this.SetVariablesABCD();
            this.D1 = this.d - this.b;
            this.D2 = this.b - this.c;
            this.D3 = this.c - this.a;
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
