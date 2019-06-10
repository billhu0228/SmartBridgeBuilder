using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartBridgeBuilder.BOQ;
using SmartBridgeBuilder.RoadDesign;

namespace SmartBridgeBuilder.Structure
{
    public struct PierItem
    {
        public int typeID;
        public double pk;
        public double offsetL;
        public double offsetT;
        public double offsetH;
        public double H0;
        public double H1;
        public double PierLBot;
        public double PierTBot;
        public double PierLTop;
        public double PierTTop;
        public double WallTh;
        public double TopTh;
        public double BotTh;
        public double LengthTop;
        internal double BeamH;

        internal double GetVolumn2()
        {

            double L = H1 - H0;
            double Ss;
            double Vgb,Vst,Vtop;
            if (L<=30)
            {
                Ss = (pk >= 3169.8 && pk <= 10629.8) ? 7.2 : 6.7;
                Vgb = 0;
                Vst = (pk >= 3169.8 && pk <= 10629.8) ? 16.8 : 12.6;
                Vtop = (pk >= 3169.8 && pk <= 10629.8) ? 303.4 :275.05;
            }
            else if (L <= 50)
            {
                Ss = (pk >= 3169.8 && pk <= 10629.8) ? 12.32 : 11.62;
                Vgb = (pk >= 3169.8 && pk <= 10629.8) ? 8.736 : 7.056;
                Vst = (pk >= 3169.8 && pk <= 10629.8) ? 37.44 : 30.24;
                Vtop = (pk >= 3169.8 && pk <= 10629.8) ? 324.66 : 295.585;
            }
            else
            {
                Ss = (pk >= 3169.8 && pk <= 10629.8) ? 16.96:16.16;
                Vgb = (pk >= 3169.8 && pk <= 10629.8) ? 30.464 : 25.984;
                Vst = (pk >= 3169.8 && pk <= 10629.8) ? 76.16 :64.96;
                Vtop = (pk >= 3169.8 && pk <= 10629.8) ? 341.78 : 312.53;
            }
            return (L - 9.5) * Ss + Vtop + Vgb + Vst;
        }
        internal double GetVolumn()
        {
            
            double S1 = PierLBot * PierTBot;
            double S2 = PierLTop * PierTTop;
            double H = LengthTop;

            double V1= H / 3*(S1 + S2 +Math.Sqrt(S1*S2));

            double V2 = (PierLBot - 2 * WallTh) * (PierTBot - 2 * WallTh) * (H1 - H0 - TopTh - BotTh);

            double V3 = (H1 - H0 - LengthTop) * S1;

            if (V1*V2*V3<0)
            {
                throw new Exception("墩身体积计算错误");
            }
            else
            {
                return V3 + V1 - V2;
            }           
        }
    }

    public class Pier
    {
        public List<PierItem> PierList;
        public bool isValid;

        public Pier(string PierFile)
        {
            isValid = false;
            PierList = new List<PierItem>();
            string[] altext = File.ReadAllLines(PierFile);

            string[] xx;
            string line;

            foreach (string item in altext)
            {
                if (item.StartsWith("//"))
                {
                    continue;
                }
                PierItem pt = new PierItem();
                try
                {
                    line = item.TrimEnd('\r');
                    line = line.TrimEnd('\t');
                    xx = Regex.Split(line, @"\s+");
                    pt.typeID = int.Parse(xx[0]);
                    pt.pk = double.Parse(xx[1]);
                    pt.offsetL = double.Parse(xx[2]);
                    pt.offsetT = double.Parse(xx[3]);
                    pt.offsetH = double.Parse(xx[4]);
                    pt.H0 = double.Parse(xx[5]);
                    pt.H1 = double.Parse(xx[6]);
                    pt.PierLBot = double.Parse(xx[7]);
                    pt.PierTBot = double.Parse(xx[8]);

                    pt.PierLTop = double.Parse(xx[9]);
                    pt.PierTTop = double.Parse(xx[10]);

                    pt.WallTh = double.Parse(xx[11]);
                    pt.TopTh = double.Parse(xx[12]);
                    pt.BotTh = double.Parse(xx[13]);
                    pt.LengthTop = double.Parse(xx[14]);
                    pt.BeamH = double.Parse(xx[15]);
                    PierList.Add(pt);
                }
                catch (Exception)
                {
                    throw;
                }

            }
            PierList.Sort((x, y) => x.pk.CompareTo(y.pk));
        }

        public void Valid(ref DesignLine maggie)
        {
            if (isValid)
            {
                return;
            }

            for (int i = 0; i < PierList.Count; i++)
            {
                PierItem cur = PierList[i];
                double pk0 = PierList[i].pk;
                double sjbg = maggie.sQX.GetBG(pk0);
                double H1 = sjbg - PierList[i].BeamH;
                if (H1 > PierList[i].H0)
                {
                    cur.H1 = H1;
                }
                else
                {
                    cur.H1 = PierList[i].H0;
                }
                PierList[i] = cur;
            }

            isValid = true;
            return;          


        }

        public void ToBoQ(ref BoQ curBoq)
        {
            if (!isValid)
            {
                throw new Exception("桥墩标高未经验证");                
            }

            BoQItem item = new BoQItem();
            foreach (var fd in PierList)
            {
                item = new BoQItem();
                item.type = "桥墩";
                item.name = "混凝土";
                item.quantity = fd.GetVolumn2();
                item.quantity2 = fd.H1-fd.H0;
                curBoq.Add(item);

                item = new BoQItem();
                item.type = "桥墩";
                item.name = "钢筋";
                item.quantity = fd.GetVolumn2() * 180;
                curBoq.Add(item);

            }

        }
    }
}
