using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartBridgeBuilder.RoadDesign;
using Tekla.Structures.Model;

namespace SmartBridgeBuilder.Structure
{
    public struct SupStr
    {
        public int typeID;
        public double pk0, pk1, dpk;
        public string dxfname;
    }


    public class SupStruture
    {
        public List<SupStr> SupList;
        public string shapePath;

        public SupStruture(string supfile, string path)
        {
            shapePath = path;

            SupList = new List<SupStr>();
            string[] altext = File.ReadAllLines(supfile);

            foreach (string item in altext)
            {
                if (item.StartsWith("//") || item == "")
                {
                    continue;
                }
                SupStr pt = new SupStr();
                try
                {
                    string line = item.TrimEnd('\r');
                    line = line.TrimEnd('\t');
                    var xx = Regex.Split(line, @"\s+");
                    pt.typeID = int.Parse(xx[0]);
                    pt.dxfname = xx[1];
                    pt.pk0 = double.Parse(xx[2]);
                    pt.pk1 = double.Parse(xx[3]);
                    pt.dpk = double.Parse(xx[4]);
                    SupList.Add(pt);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void ToTekla(ref Model myModel, ref DesignLine maggie)
        {
            SteelBeam stb;
            ConcretBeam conb;
            string fulldxfpath;
            foreach (var item in SupList)
            {
                fulldxfpath = Path.Combine(shapePath, item.dxfname);
                if (!Path.HasExtension(fulldxfpath))
                {
                    fulldxfpath += ".dxf";
                }

                if (item.typeID==1)
                {
                    conb = new ConcretBeam(fulldxfpath);
                    conb.ToTekla(ref myModel, ref maggie, item.pk0, item.pk1, item.dpk);
                }
                else if (item.typeID==2)
                {
                    stb = new SteelBeam(fulldxfpath);
                    stb.ToTekla(ref myModel, ref maggie, item.pk0, item.pk1, item.dpk);
                }
                else
                {
                    continue;
                }
            }
        }
    }
}
