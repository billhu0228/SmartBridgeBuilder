using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CADImport;
using CADImport.DWG;
using SmartBridgeBuilder.Extension;

namespace SmartBridgeBuilder.Structure
{
    public struct Block
    {
        public int typeID;
        public string dwgname;
        public double pk, H;
        public int factor;

    }
    public class AcBlocks
    {
        public List<Block> BlockList;
        public string shapePath;
        public AcBlocks(string supfile, string path)
        {
            shapePath = path;

            BlockList = new List<Block>();
            string[] altext = File.ReadAllLines(supfile);

            foreach (string item in altext)
            {
                if (item.StartsWith("//") || item == "")
                {
                    continue;
                }
                Block pt = new Block();
                try
                {
                    string line = item.TrimEnd('\r');
                    line = line.TrimEnd('\t');
                    var xx = Regex.Split(line, @"\s+");
                    pt.typeID = int.Parse(xx[0]);
                    pt.dwgname = xx[1];
                    pt.pk = double.Parse(xx[2]);
                    pt.H = double.Parse(xx[3]);
                    pt.factor = int.Parse(xx[4]);

                    BlockList.Add(pt);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void ToDXF(ref DWGImage vDrawing)
        {
            Dictionary<string, CADBlock> blkdic = new Dictionary<string, CADBlock>();
            string fulldwgfile;
            DWGImage input;

            CADBlock bk;
            //刷新图块

            foreach (var item in BlockList)
            {

                if (blkdic.Keys.Contains(item.dwgname))//已有相同块
                {
                    bk = blkdic[item.dwgname];
                }
                else
                {
                    fulldwgfile = Path.Combine(shapePath, item.dwgname);
                    if (!Path.HasExtension(fulldwgfile))
                    {
                        fulldwgfile += ".dwg";
                    }

                    CADBlock bt = new CADBlock();
                    bt.Name = Guid.NewGuid().ToString();
                   

                    input = new DWGImage();
                    input.LoadFromFile(fulldwgfile);
                    foreach (var ent in input.CurrentLayout.Entities)
                    {
                        var aa = ent.GetCopy(ref vDrawing);
                        if (aa != null)
                        {
                            bt.AddEntity(aa);
                        }
                    }

                    vDrawing.Converter.Blocks.Add(bt);
                    vDrawing.Converter.Loads(bt);

                    blkdic.Add(item.dwgname, bt);

                }     

            }


            foreach (var item in BlockList)
            {

                if (item.typeID == 1)//立面图块;
                {
                    continue;
                }
                else if (item.typeID == 2)// 平面图块
                {
                    continue;
                }
                else if (item.typeID == 3)// 任意位置图块
                {
                    NetCADTool.AddBlock(ref vDrawing,  blkdic[item.dwgname], new DPoint(item.pk, item.H, 0), item.factor);
                }
                else
                {
                    continue;
                }

            }


        }
    }
}
