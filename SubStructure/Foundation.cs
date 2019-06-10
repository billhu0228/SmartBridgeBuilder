using SmartBridgeBuilder.BOQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartBridgeBuilder.Structure
{
    public struct PileFoundation
    {
        public int typeID;
        public double pk;
        public double offsetL, offsetT, offsetH;
        public double H0;
        public double PileCapL, PileCapT, PileCapH;
        public double Lz, Dz;
        public int PileLNum, PileTNum;
        public double PileSpaceL, PileSpaceT;
    }


    public class Foundation
    {

        public List<PileFoundation> FoundList;


        public Foundation(string FoundationFile)
        {
            FoundList = new List<PileFoundation>();
            string[] altext = File.ReadAllLines(FoundationFile);

            string[] xx;
            string line;

            foreach (string item in altext)
            {
                if (item.StartsWith("//"))
                {
                    continue;
                }
                PileFoundation pt = new PileFoundation();
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
                    pt.PileCapL = double.Parse(xx[6]);
                    pt.PileCapT = double.Parse(xx[7]);
                    pt.PileCapH = double.Parse(xx[8]);

                    pt.Lz = double.Parse(xx[9]);
                    pt.Dz = double.Parse(xx[10]);

                    pt.PileLNum = int.Parse(xx[11]);
                    pt.PileTNum = int.Parse(xx[12]);
                    pt.PileSpaceL = double.Parse(xx[13]);
                    pt.PileSpaceT = double.Parse(xx[14]);                    

                    FoundList.Add(pt);
                }
                catch (Exception)
                {
                    throw;
                }

            }
            FoundList.Sort((x, y) => x.pk.CompareTo(y.pk));
        }

        public void ToBoQ(ref BoQ curBoq)
        {
            BoQItem item = new BoQItem();
            foreach (var fd in FoundList)
            {
                item = new BoQItem();
                item.type = "承台";
                item.name = "混凝土";                
                item.quantity = fd.PileCapH * fd.PileCapL * fd.PileCapT;
                item.quantity2 = 1;
                curBoq.Add(item);

                item = new BoQItem();
                item.type = "承台";
                item.name = "混凝土";
                item.quantity =0.1* (fd.PileCapL+0.2) * (fd.PileCapT+0.2);                
                curBoq.Add(item);

                item = new BoQItem();
                item.type = "承台";
                item.name = "钢筋";                
                item.quantity = fd.PileCapH * fd.PileCapL * fd.PileCapT*100;
                curBoq.Add(item);

                for (int i = 0; i < fd.PileLNum*fd.PileTNum; i++)
                {
                    item = new BoQItem();
                    item.type = "桩基";
                    item.name = "混凝土";
                    item.quantity = fd.Lz;
                    item.quantity2 = fd.Lz * Math.PI * fd.Dz * fd.Dz * 0.25;
                    curBoq.Add(item);

                    item = new BoQItem();
                    item.type = "桩基";
                    item.name = "钢筋";
                    item.quantity = fd.Lz * Math.PI * fd.Dz * fd.Dz * 0.25 * 120;
                    curBoq.Add(item);

                }



            }
        }
    }
}
