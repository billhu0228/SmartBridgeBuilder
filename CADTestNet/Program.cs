using System;
using System.Collections.Generic;
using CADImport;
using CADImport.DWG;
using CADImport.DXF;
using CADImport.Export;
using CADImport.Export.DirectCADtoDXF;
using CADImport.RasterImage;



namespace CADTestNet
{
    class Program
    {
        static void Main(string[] args)
        {
            DWGImage vDrawing = new DWGImage();
            vDrawing.LoadFromFile(@"D:\Program Files (x86)\CADSoftTools\CAD .NET 14\Files\Gasket.dwg");
            CADLine vLine = new CADLine();

            vLine.Point = new DPoint(50, 0, 0);

            vLine.Point1 = new DPoint(50, 70, 10);


            

            vLine.LineWeight = 1;

            vDrawing.Converter.Loads(vLine);

            vDrawing.CurrentLayout.AddEntity(vLine);

            CADtoDXF ff = new CADtoDXF(vDrawing);
            ff.SaveToFile("fuckl.dxf");

            ff.Dispose();

            // Recalculates the extents of the drawing  

            //CADtoDWG.SaveAsDWG(vDrawing,"fauck");

        }
    }
}
