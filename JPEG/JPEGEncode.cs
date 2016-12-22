using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPEG
{
    public class JPEGEncode
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
        private int contextOfX;
        private int Px;
        private int Rx;
        private int SIGN;

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
            this.SIGN = 0;
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

            while (posX * posY == (width - 1) * (height - 1))
            {
                this.GetNextSample();
                if (this.D[0] == 0 && this.D[1] == 0 && this.D[2] == 0)
                    this.RunModeProcessing();
                else
                    this.RegularModeProcessing();
            }

            return new byte[1];
        }

        /**
         *  RUN MODE
         * 
         * 
         */
        private void RunModeProcessing()
        {
            this.Quantize();
            this.PredictionPx();
            this.PredictionCorrect();
            this.ComputeRx();
        }


        /**
         *  REGULAR MODE
         * 
         * 
         */
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

            this.x = this.LD[(posY) * width + posX];
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

        /**
         *  QUANTIZATION OF GRADIENTS
         * 
         * 
         */
        private void Quantize()
        {
            this.GetQuantizationGradients();

            SIGN = 1;

            if(Q[0] < 0 || (Q[0] == 0 && Q[1] < 0) ||
                (Q[0] == 0 && Q[1] == 0 && Q[2] < 0))
            {
                Q[0] *= -1;
                Q[1] *= -1;
                Q[2] *= -1;
                SIGN = -1;
            }

            contextOfX = 81 * Q[0] + 9 * Q[1] + Q[2];
        }

        /**
         *  EDGE-DETECTING PREDICTOR
         * 
         * 
         */
        private void PredictionPx()
        {
            if (c >= max(a, b))
            {
                Px = min(a, b);
            }
            else
            {
                if (c <= min(a, b))
                    Px = max(a, b);
                else
                    Px = a + b - c;
            }
        }

        /**
         *  PREDICTION CORRECTION FROM THE BIAS
         * 
         * 
         */
        private void PredictionCorrect()
        {
            if (SIGN == 1)
                Px = Px - C[contextOfX];
            else
                Px = Px - C[contextOfX];

            if (Px > MAXVAL)
                Px = MAXVAL;
            else if (Px < 0)
                Px = 0;
        }


        /**
         *  COMPUTATION OF PREDICTION ERROR 
         * 
         * 
         */
        private void ComputeRx()
        {
            int errval = x - Px;
            if (SIGN == -1)
                errval *= -1;

            if(NEAR == 0)
            {
                Rx = x;
            }
            else
            {
                if (errval > 0)
                    errval = (errval + NEAR) / (2 * NEAR + 1);
                else
                    errval = -(NEAR - errval) / (2 * NEAR + 1);

                Rx = Px + SIGN * errval * (2 * NEAR + 1);

                if (Rx < 0)
                    Rx = 0;
                else if (Rx > MAXVAL)
                    Rx = MAXVAL;
            }

            if (errval < 0)
                errval = errval + RANGE;
            if(errval >= (RANGE + 1) / 2)
                errval = errval - RANGE;

            for(int k = 0; (N[contextOfX] << k) < A[contextOfX]; k++)
            {
                k = Math.Log(A[contextOfX] / N[contextOfX], 2);
            }
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
         *  NAPOLNIMO ARRAY Z DOLOCENIMI VREDNOSTMI
         * 
         * 
         */
        private void Populate(int[] arr, int value)
        {
            for (int i = 0; i < arr.Length; i++)
                arr[i] = value;
        }
    }
}
