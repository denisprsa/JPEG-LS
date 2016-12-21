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
        private int computeContext;

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

        private int[] D;
        private int[] T;
        private int[] Q;

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
        private void init(int near)
        {
            this.NEAR = near;
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
            this.T = new int[] {3, 7, 12};
            this.D = new int[] { 0, 0, 0 };
            this.Q = new int[] { 0, 0, 0 };

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
            this.init(n);

            this.GetNextSample();
            if (this.D[0] == 0 && this.D[1] == 0 && this.D[2] == 0)
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
        private void GetQuantizationGradients()
        {
            for(int i = 0; i < 3; i++)
            {
                if (D[i] <= -T[2]) Q[i] = -4;
                else if (D[i] <= -T[1]) Q[i] = -3;
                else if (D[i] <= -T[0]) Q[i] = -2;
                else if (D[i] < -NEAR) Q[i] = -1;
                else if (D[i] <= NEAR) Q[i] = 0;
                else if (D[i] < T[0]) Q[i] = 1;
                else if (D[i] < T[1]) Q[i] = 2;
                else if (D[i] < T[2]) Q[i] = 3;
                else Q[i] = 4;
            }
        }

        private void RegularModeProcessing()
        {
            
        }

        /**
         *  GET NEXT SAMPLE OF IMAGE
         * 
         *
         */
        private void GetNextSample()
        {
            this.SetVariablesABCD();
            this.D[0] = this.d - this.b;
            this.D[1] = this.b - this.c;
            this.D[2] = this.c - this.a;

            posX += 1;

            if (posX == width)
            {
                posX = 0;
                posY += 1;
            }
        }

        private void Quantize(int i)
        {
            this.GetQuantizationGradients();

            int sx = 1;

            if(Q[0] < 0 || (Q[0] == 0 && Q[1] < 0) ||
                (Q[0] == 0 && Q[1] == 0 && Q[2] < 0))
            {
                Q[0] *= -1;
                Q[1] *= -1;
                Q[2] *= -1;
                sx = -1;
            }

            computeContext = 81 * Q[0] + 9 * Q[1] + Q[2];
        }

        private void ModRange(int i)
        {

        }

        private void ComputeRx()
        {

        }
    }
}
