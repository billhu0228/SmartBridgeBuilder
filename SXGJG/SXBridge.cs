using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBridgeBuilder.SXGJG
{

    public class SXBridge
    {


        public List<int> LCHLeft, LCHRight, KeyPoint;
        public List<int> UCHLeft, UCHRight;
        public List<int> UCHLeftPC, UCHRightPC, LCHLeftPC, LCHRightPC;

               
        public SXBridge()
        {
            int x = 0;
            LCHLeft = new List<int>();
            LCHLeft.Add(x);
            LCHLeft.Add2(5000, 2);
            LCHLeft.Add2(11000 / 2, 2);
            LCHLeft.Add2(12000 / 2, 22);
            LCHLeft.Add2(11750 / 2, 2);
            LCHLeft.Add2(11250 / 2, 2);
            LCHLeft.Add2(11000 / 2, 4);
            LCHLeft.Add2(11250 / 2, 2);
            LCHLeft.Add2(11750 / 2, 2);
            LCHLeft.Add2(12000 / 2, 18);
            LCHLeft.Add2(11750 / 2, 2);
            LCHLeft.Add2(11250 / 2, 2);
            LCHLeft.Add2(11000 / 2, 4);
            LCHLeft.Add2(11250 / 2, 2);
            LCHLeft.Add2(11750 / 2, 2);
            LCHLeft.Add2(12000 / 2, 22);
            LCHLeft.Add2(11000 / 2, 2);
            LCHLeft.Add2(5000, 2);
            x = 0;
            LCHRight = new List<int>();
            LCHRight.Add(x);
            LCHRight.Add2(5000, 2);
            LCHRight.Add2(11000 / 2, 2);
            LCHRight.Add2(12000 / 2, 22);
            LCHRight.Add2(11750 / 2, 2);
            LCHRight.Add2(11250 / 2, 2);
            LCHRight.Add2(11000 / 2, 4);
            LCHRight.Add2(11250 / 2, 2);
            LCHRight.Add2(11750 / 2, 2);
            LCHRight.Add2(12000 / 2, 18);
            LCHRight.Add2(11750 / 2, 2);
            LCHRight.Add2(11250 / 2, 2);
            LCHRight.Add2(11000 / 2, 4);
            LCHRight.Add2(11250 / 2, 2);
            LCHRight.Add2(11750 / 2, 2);
            LCHRight.Add2(12000 / 2, 22);
            LCHRight.Add2(11000 / 2, 2);
            LCHRight.Add2(5000, 1);
            LCHRight.Add2(5000 - 250);


            x = 0;
            KeyPoint = new List<int>();
            KeyPoint.Add(x);
            KeyPoint.Add2(10000);
            KeyPoint.Add2(11000);
            KeyPoint.Add2(12000, 11);
            KeyPoint.Add2(11750);
            KeyPoint.Add2(11250);
            KeyPoint.Add2(11000, 2);
            KeyPoint.Add2(11250);
            KeyPoint.Add2(11750);
            KeyPoint.Add2(12000, 9);
            KeyPoint.Add2(11750);
            KeyPoint.Add2(11250);
            KeyPoint.Add2(11000, 2);
            KeyPoint.Add2(11250);
            KeyPoint.Add2(11750);
            KeyPoint.Add2(12000, 11);
            KeyPoint.Add2(11000);
            KeyPoint.Add2(10000 - 250);
            KeyPoint.Add2(250);


            x = 0;
            UCHLeft = new List<int>();
            UCHLeft.Add(x);
            UCHLeft.Add2(5000, 3);
            UCHLeft.Add2(6000, 24);
            UCHLeft.Add2(11500 / 2, 2);
            UCHLeft.Add2(11000 / 2, 6);
            UCHLeft.Add2(11500 / 2, 2);
            UCHLeft.Add2(6000, 20);
            UCHLeft.Add2(11500 / 2, 2);
            UCHLeft.Add2(11000 / 2, 6);
            UCHLeft.Add2(11500 / 2, 2);
            UCHLeft.Add2(6000, 24);
            UCHLeft.Add2(5000, 3);

            UCHRight = new List<int>(UCHLeft);
            UCHRight.Remove(550000);
            UCHRight.Add(549750);


            UCHLeftPC = new List<int>
            {
                0,27,51,74,91,108,112,115,108,101,89,77,64,51,36,21,11,
                0,19,39,73,107,136,164,187,210,226,243,253,264,269,274,274,274,269,264,253,243,226,210,187,164,136,107,73,39,19,
                0,19,39,73,107,136,164,187,210,226,243,253,264,269,274,274,274,269,264,253,243,226,210,187,164,136,107,73,39,19,
                0,11,21,36,51,64,77,89,101,108,115,112,108,91,74,51,27,0
            };
            UCHRightPC = new List<int>
            {
                0,27,51,74,91,108,112,115,108,101,89,77,64,51,36,21,11,
                0,19,39,73,107,136,164,187,210,226,243,253,264,269,274,274,274,269,264,253,243,226,210,187,164,136,107,73,39,19,
                0,19,39,73,107,136,164,187,210,226,243,253,264,269,274,274,274,269,264,253,243,226,210,187,164,136,107,73,39,19,
                0,11,21,36,51,64,77,89,101,108,115,112,108,91,74,51,27,0
            };
            LCHLeftPC = new List<int>
            {
                0,25,51,71,91,101,112,110,108,98,89,76,64,50,36,18,0,
                0,0,37,73,105,136,161,187,207,226,240,253,261,269,271,274,271,269,261,253,240,226,207,187,161,136,105,73,37,0,
                0,0,37,73,105,136,161,187,207,226,240,253,261,269,271,274,271,269,261,253,240,226,207,187,161,136,105,73,37,0,
                0,0,18,36,50,64,76,89,98,108,110,112,101,91,71,51,25,0
            };
            LCHRightPC = new List<int>
            {
                0,25,51,71,91,101,112,110,108,98,89,76,64,50,36,18,0,
                0,0,37,73,105,136,161,187,207,226,240,253,261,269,271,274,271,269,261,253,240,226,207,187,161,136,105,73,37,0,
                0,0,37,73,105,136,161,187,207,226,240,253,261,269,271,274,271,269,261,253,240,226,207,187,161,136,105,73,37,0,
                0,0,18,36,50,64,76,89,98,108,110,112,101,91,71,51,25,0
            };


        }

        internal double InterPor(double x0, double y0, double x1, double y1, double xi)
        {
            double k = (y1 - y0) / (x1 - x0);
            double b = y0 - k * x0;
            return k * xi + b;
        }

        internal double GetHP(int x0, bool isRight = true)
        {
            if (isRight)
            {
                return 0.025;
            }
            else
            {
                double pk0 = GetPK(x0);
                if (pk0 <= 239687.7)
                {
                    return -0.025;
                }
                else
                {
                    return InterPor(239687.707, -0.025, 239907.707, 0.04, pk0);
                }
            }
        }

        private double GetPK(int x0)
        {
            // x=0,pk=239216.5;
            return x0 / 1000.0 + 239216.5;
        }
        private int GetX(double pk0)
        {
            // x=0,pk=239216.5;
            return (int)(1000 * (pk0 - 239216.5));
        }


        internal double GetUCH(int x0)
        {
            return x0 * -0.02;
        }

        internal double GetLCH(int x0)
        {

            if (!KeyPoint.Contains(x0))
            {
                if (x0==545000)
                {
                    return InterPor(540000, GetLCH(540000),550000, GetLCH(550000), 545000);
                }

                var tmp = KeyPoint.ToList();
                tmp.Add(x0);
                tmp.Sort((x, y) => x.CompareTo(y));
                int kk = tmp.IndexOf(x0);
                double z0 = GetLCH(KeyPoint[kk - 1]);
                double z1 = GetLCH(KeyPoint[kk]);

                return InterPor(KeyPoint[kk - 1], z0, KeyPoint[kk], z1, x0);

            }
            else
            {
                if (x0 <= 93000)
                {
                    return -1.21401E-06 * x0 * x0 - 0.02 * x0 - 5618.75;
                }
                else if (x0 <= 269000)
                {
                    return -1.56157049375372E-06 * x0 * x0 + 5.64027364663891E-01 * x0 - 6.02253085960738E+04;
                }
                else if (x0 <= 445000)
                {
                    return -1.56157049375372E-06 * x0 * x0 + 1.11370017846520E+00 * x0 - 2.11385332391433E+05;                    
                }
                else
                {
                    return -1.21401318073004E-06 * x0 * x0 + 1.31541449880160E+00 * x0 - 3.72857737170064E+05;
                }
            }

        }
    }
}
