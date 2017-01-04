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
        private Bits byteManager = new Bits();

        private int NEAR;
        private int MAXVAL;
        private int RANGE;
        private int RESET;
        private int qbpp;
        private int bpp;
        private int LIMIT;
        private int RItype;
        private int RUNindex;
        private int RUNval;
        private int RUNcnt;
        private int N_MAX;
        private int C_MIN;
        private int C_MAX;
        private int contextOfX;
        private int Px;
        private int Rx;
        private int SIGN;
        private int PrevRUNindex;

        private int[] N;
        private int[] Nn;
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
        private bool EOLinteruption;

        /**
         *  INIT DATA
         * 
         * 
         */
        private void init(int near)
        {
            this.PrevRUNindex = 0;
            this.NEAR = near;
            this.C_MAX = 127;
            this.C_MIN = -128;
            this.N = new int[367];
            this.Nn = new int[367];
            this.A = new int[367];
            this.B = new int[365];
            this.C = new int[365];
            this.Populate(this.N, 1);
            this.Populate(this.Nn, 0);
            this.Populate(this.B, 0);
            this.Populate(this.C, 0);
            this.Populate(this.A, 4);
            //this.N[365] = 0;
            //this.N[366] = 0;
            this.RESET = 64;
            this.RUNindex = 0;
            this.EOLinteruption = false;
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
            this.qbpp = (int)Math.Log(this.RANGE, 2);
            this.bpp = 8;
            this.LIMIT = 2 * (bpp + bpp);
            this.SIGN = 0;
            this.T = new int[] {3, 7, 12};
            this.D = new int[] { 0, 0, 0 };
            this.Q = new int[] { 0, 0, 0 };
            this.width = image.Width;
            this.height = image.Height;
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
            this.LD = new byte[16]{
                0,0,90,74,
                68,50,43,205,
                64,145,145,145,
                100,145,145,145
            };
            this.width = 4;
            this.height = 4;

            while (posX * posY < (width - 1) * (height - 1))
            {
                this.GetNextSample();

                Console.WriteLine("0: " + D[0] + " 1: " + D[1] + " 2: " + D[2]);
                Console.WriteLine("a: " + a + " b: " + b + " c: " + c + " d: " + d + " x: " + x);
                if (this.D[0] == 0 && this.D[1] == 0 && this.D[2] == 0)
                    this.RunModeProcessing();
                else
                    this.RegularModeProcessing();

                PrintBits();
                Console.WriteLine();
                byteManager.bits.Clear();
            }

            return new byte[1];
        }

        private void PrintBits()
        {
            foreach (var i in byteManager.bits)
            {
                Console.Write(i ? "1" : "0");
            }
        }

        /**
         *  RUN MODE
         * 
         * 
         */
        private void RunModeProcessing()
        {
            Console.WriteLine("RunMode");
            RunLengthDetermination();
            EncodeRunLengthSegment();
            EncodeInteruptedValue();

        }

        private void RunLengthDetermination()
        {
            RUNval = this.a;
            RUNcnt = 0;
            bool endLine = false;
            while (Math.Abs(this.x - RUNval) <= NEAR)
            {
                RUNcnt = RUNcnt + 1; Rx = RUNval;
                if (endLine)
                {
                    break;
                }
                else
                {
                    endLine = GetNextSample();
                }
            }
        }

        /**
         *  The variable RUNcnt computed following the procedure 
         *  in Code segment A.14 represents the run length.
         * 
         * 
         */
        private void EncodeRunLengthSegment()
        {
            // A.15
            while (RUNcnt >= (1 << J[RUNindex]))
            {
                byteManager.add(true);
                RUNcnt = RUNcnt - (1 << J[RUNindex]);
                if (RUNindex < 31)
                    RUNindex = RUNindex + 1;
            }

            // A.16
            this.PrevRUNindex = RUNindex;
            this.EOLinteruption = false;
            if (Math.Abs(this.x - RUNval) > NEAR)
            {
                this.EOLinteruption = true;
                byteManager.add(false);
                for (int i = J[RUNindex]; i > 0; i--)
                {
                    if ((RUNcnt & J[RUNindex]) == 1)
                        byteManager.add(true);
                    else
                        byteManager.add(false);

                    RUNcnt <<= 1;
                }
                if (RUNindex > 0)
                    RUNindex = RUNindex - 1;
            }
            else if (RUNcnt > 0)
                byteManager.add(true);
        }


        /**
         * 
         * 
         * 
         */
        private void EncodeInteruptedValue()
        {
            IndexComuptation();
            int Errval = PredictionErrorRunInteruption();
            Errval = ErrorCumpotationForRunInterruption(Errval);

            int TEMP = 0;
            if (RItype == 0)
                TEMP = A[365];
            else
                TEMP = A[366] + (N[366] >> 1);

            contextOfX = RItype + 365;

            int k = 0;
            for (k = 0; (N[contextOfX] << k) < TEMP; k++) ;

            //
            int map = 0;
            if ((k == 0) && (Errval > 0) && (2 * Nn[contextOfX] < N[contextOfX]))
                map = 1;
            else if ((Errval < 0) && (2 * Nn[contextOfX] >= N[contextOfX]))
                map = 1;
            else if ((Errval < 0) && (k != 0))
                map = 1;
            else
                map = 0;
            
            //
            int EMErrval = 2 * Math.Abs(Errval) - RItype - map;

            if (EOLinteruption)
            {
                Golomb GB = new Golomb();
                GB.Encode(ref byteManager, k, EMErrval, (LIMIT - J[PrevRUNindex] - 1), qbpp);
            }

            //
            if (Errval < 0)
                Nn[contextOfX] = Nn[contextOfX] + 1;

            A[contextOfX] = A[contextOfX] + ((EMErrval + 1 - RItype) >> 1);
            
            if (N[contextOfX] == RESET)
            {
                A[contextOfX] = A[contextOfX] >> 1;
                N[contextOfX] = N[contextOfX] >> 1;
                Nn[contextOfX] = Nn[contextOfX] >> 1;
            }

            N[contextOfX] = N[contextOfX] + 1;
        }

        /**
         * 
         */
        private void IndexComuptation()
        {
            if (Math.Abs(a - b) <= NEAR)
                RItype = 1;
            else
                RItype = 0;
        }


        /**
         * 
         * 
         */
        private int PredictionErrorRunInteruption()
        {
            if (RItype == 1)
                Px = this.a;
            else
                Px = this.b;
            return x - Px;
        }

        /**
         * 
         * 
         */
        private int ErrorCumpotationForRunInterruption(int Errval)
        {
            if (RItype == 0 && a > b)
            {
                Errval = -Errval;
                SIGN = -1;
            }
            else
                SIGN = 1;

            if (NEAR > 0)
            {
                //Errval = Quantize();
                //x = ComputeX();
            }
            else
                Rx = x;

            ReductionOfError(ref Errval);

            return Errval;
        }



        /**
         *  REGULAR MODE
         * 
         * 
         */
        private void RegularModeProcessing()
        {
            Console.WriteLine("RegularMode");
            this.Quantize();
            this.PredictionPx();
            this.PredictionCorrect();
            int errval = CalculateErrorValue();
            this.ComputeRx(ref errval);
            this.ReductionOfError(ref errval);

            int k = 0;
            for (k = 0; (N[contextOfX] << k) < A[contextOfX]; k++);

            int MError = this.ErrorMapping(errval, k);
            Golomb GB = new Golomb();
            GB.Encode(ref byteManager, k, MError, LIMIT, qbpp);
            this.UpdateVariables(errval);
        }
        
        /**
         *  GET NEXT SAMPLE OF IMAGE
         * 
         *
         */
        private bool GetNextSample()
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
                return true;
            }
            return false;
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
                this.d = this.LD[(posY - 1) * width + posX];
            else
                this.d = this.LD[((posY - 1) * width) + posX + 1];

            this.x = this.LD[(posY) * width + posX];
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

            if (Q[0] < 0 || (Q[0] == 0 && Q[1] < 0) ||
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
         *  EDGE-DETECTING PREDICTOR
         * 
         * 
         */
        private void PredictionPx()
        {
            if (c >= max(a, b))
                Px = min(a, b);
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
                Px = Px + C[contextOfX];
            else
                Px = Px - C[contextOfX];

            if (Px > MAXVAL)
                Px = MAXVAL;
            else if (Px < 0)
                Px = 0;
        }

        private int CalculateErrorValue()
        {
            int errval = x - Px;
            if (SIGN == -1)
                errval = -errval;
            return errval;
        }

        /**
         *  COMPUTATION OF PREDICTION ERROR 
         * 
         * 
         */
        private void ComputeRx(ref int errval)
        {
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
            
        }

        private void ReductionOfError(ref int errval)
        {
            if (errval < 0)
                errval = errval + RANGE;
            if (errval >= (RANGE + 1) / 2)
                errval = errval - RANGE;
        }

        private int ErrorMapping(int errval, int gK)
        {
            int MErrval = -1;
            if (NEAR == 0 && errval < 0)
                MErrval = -errval;
            else
            {
                if (NEAR == 0 && gK == 0 && (2 * B[contextOfX] <= -N[contextOfX]))
                {
                    if (errval >= 0)
                        MErrval = 2 * errval + 1;
                    else
                        MErrval = -2 * (errval + 1);
                }
                else
                {
                    if (errval >= 0)
                        MErrval = 2 * errval;
                    else
                        MErrval = -2 * errval - 1;  
                }
            }
            return MErrval;
        }

        private void UpdateVariables(int errval)
        {
            B[contextOfX] = B[contextOfX] + errval * (2 * NEAR + 1);
            A[contextOfX] = A[contextOfX] + Math.Abs(errval);
            if (N[contextOfX] == RESET)
            {
                A[contextOfX] = A[contextOfX] >> 1;
                if (B[contextOfX] >= 0)
                    B[contextOfX] = B[contextOfX] >> 1;
                else
                    B[contextOfX] = -((1 - B[contextOfX]) >> 1);

                N[contextOfX] = N[contextOfX] >> 1;
            }
            N[contextOfX] = N[contextOfX] + 1;
        }

        private void UpdateBiasVariable()
        {
            if (B[contextOfX] <= -N[contextOfX])
            {
                B[contextOfX] = B[contextOfX] + N[contextOfX];
                if (C[contextOfX] > C_MIN)
                    C[contextOfX] = C[contextOfX] - 1;
                if (B[contextOfX] <= -N[contextOfX])
                    B[contextOfX] = -N[contextOfX] + 1;
            }
            else if (B[contextOfX] > 0)
            {
                B[contextOfX] = B[contextOfX] - N[contextOfX];
                if (C[contextOfX] < C_MAX)
                    C[contextOfX] = C[contextOfX] + 1;
                if (B[contextOfX] > 0)
                    B[contextOfX] = 0;
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
         *  @arr    : int[]
         *  @value  : int
         *  @return : void
         */
        private void Populate(int[] arr, int value)
        {
            for (int i = 0; i < arr.Length; i++)
                arr[i] = value;
        }
    }
}
