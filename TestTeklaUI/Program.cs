using SmartBridgeBuilder.Extension;
using SmartBridgeBuilder.RoadDesign;
using Tekla.Structures.Geometry3d;
using SmartBridgeBuilder.Structure;
using System;
using System.IO;
using System.Linq;
using Tekla.Structures.Model;
using Tekla.Structures.Catalogs;
using System.Collections.Generic;
using System.Collections;
using CADImport.DWG;
using SmartBridgeBuilder.BOQ;
using CADImport.Export.DirectCADtoDXF;

namespace TestTeklaUI
{
    class Program
    {
        static PQX bill;
        static SQX kitty;
        static DMX tony;
        static DesignLine maggie;
        static SUB oo;
        static SupStruture bb;
        static AcBlocks blc;
        static Cables cab;
        static ConcShapeItem md;

        static string dwtfile;
        static string ShapeItemPath;

        static void Main(String[] PathInput = null)
        {
            if (PathInput.Count() == 0)
            {
                PathInput = new string[] { @"E:\Bill\20190411 - CCTV\03 TestDir\Input", @"E:\Bill\20190411 - CCTV\03 TestDir\Output" };
            }
            MakeBridge(PathInput[0]);
            RunDwg(PathInput[1]);
            RunTekla(PathInput[1]);
            
        }

        private static void MakeBridge(string wkdir)
        {
            try
            {
                ShapeItemPath = Path.Combine(wkdir, "shapeItems");

                string jdfile = Directory.GetFiles(wkdir, "*.JD")[0];
                string sqxfile = Directory.GetFiles(wkdir, "*.SQX")[0];
                string dmxfile = Directory.GetFiles(wkdir, "*.DMX")[0];
                string subfile = Directory.GetFiles(wkdir, "*.XB")[0];
                string cabfile = Directory.GetFiles(wkdir, "*.CAB")[0];
                string brebfile = Directory.GetFiles(wkdir, "*.BRE")[0];
                string supfile = Directory.GetFiles(wkdir, "*.SB")[0];
                string blcfile = Directory.GetFiles(wkdir, "*.TK")[0];

                bill = new PQX(jdfile);
                kitty = new SQX(sqxfile);
                tony = new DMX(dmxfile);
                maggie = new DesignLine(bill, kitty, tony, 50);
                oo = new SUB(subfile);
                bb = new SupStruture(supfile, ShapeItemPath);
                blc = new AcBlocks(blcfile, ShapeItemPath);
                cab = new Cables(cabfile);
                md = new ConcShapeItem(brebfile);
                dwtfile = Directory.GetFiles(wkdir, "*.dwg")[0];          

                Console.WriteLine("Parameters initialized successfully...");
                return;
            }
            catch (Exception e)
            {
                throw new Exception("Parameters initialized filed:" + e.Message);
            }
        }

        static void RunDwg(string outpath)
        {
            DWGImage vDrawing = new DWGImage();
            vDrawing.LoadFromFile(dwtfile);
            cab.ToDXF(ref vDrawing, ref maggie);
            oo.ToDXF(ref vDrawing, ref maggie);
            maggie.ToDXF(ref vDrawing, 0, 2025, 50);
            blc.ToDXF(ref vDrawing);
            CADtoDXF ff = new CADtoDXF(vDrawing);
            ff.SaveToFile(Path.Combine(outpath, "out.dxf"));
            ff.Dispose();
           // NetCADTool.CleanDxfFile(Path.Combine(outpath, "out.dxf"));
            Console.WriteLine("DXF file created successfully...");
        }

        static void RunTekla(string outpath)
        {
            try
            {
                ModelHandler handler = new ModelHandler();

                handler.CreateNewSingleUserModel(Guid.NewGuid().ToString(), outpath);
                Console.WriteLine("Model initialized successfully...");
            }
            catch (Exception)
            {

                throw new Exception("Model initialized filed...");
            }


            Model myModel = new Model(); // Tekla指针      

            bool x = myModel.GetConnectionStatus();
            if (x)
            {
                Console.WriteLine("API connected successfully...");
            }
            else
            {
                throw new Exception("API connected filed...");
            }

            LoadShapeItems(ShapeItemPath);        

            md.ToTekla(ref myModel, ref maggie);
            oo.ToTekla(ref myModel, ref maggie);
            bb.ToTekla(ref myModel, ref maggie);
            maggie.ToTekla(ref myModel, cab.PkB1, cab.PkB2, 50);
            cab.ToTekla(ref myModel, ref maggie);

            myModel.CommitChanges();

            Console.WriteLine("Model created successfully...");
            Console.WriteLine("Writing to IFC file...");
            TeklaTools.ExportIFC(Path.Combine(outpath, "out.ifc"));
            Console.WriteLine("IFC file created successfully...");
        }

        private static void LoadShapeItems(string shapeItemDwgPath)
        {
            try
            {           
                var ff = new CatalogHandler().ImportShapeItems(shapeItemDwgPath);
                Console.WriteLine("Brep loaded successfully...");
            }
            catch (Exception)
            {

                throw new Exception("Brep loaded failed...");
            }


        }
    }
}