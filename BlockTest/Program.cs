using CADImport;
using CADImport.DWG;
using CADImport.Export.DirectCADtoDXF;
using netDxf;
using netDxf.Entities;
using QRCoder;
using SmartBridgeBuilder.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockTest
{
    class Program
    {
        static void Main(string[] args)
        {



            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                Guid.NewGuid().ToString(),
                QRCodeGenerator.ECCLevel.M, true, true, 
                QRCodeGenerator.EciMode.Utf8, 10);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap bmp = qrCode.GetGraphic(10, Color.Black, Color.White,true);

            bmp.Save(@"E:\1.bmp");

            DWGImage vDrawing = new DWGImage();
            vDrawing.LoadFromFile(@"C:\Users\IBD2\Desktop\M2.dwg");

            DWGImage input = new DWGImage();
            input.LoadFromFile(@"C:\Users\IBD2\Desktop\I2.dwg");

            CADCircle c = new CADCircle();
            c.Radius = 1000;
            c.Point = new DPoint(1, 1, 1);

            CADBlock bt = new CADBlock();
    
            bt.Name =Guid.NewGuid().ToString();





            vDrawing.Converter.Blocks.Add(bt);
            CADtoDXF dfdf = new CADtoDXF(vDrawing);

            dfdf.SaveToFile("ouf32t.dxf");
            dfdf.Dispose();
            DxfDocument dxfDocument = DxfDocument.Load("ouf32t.dxf");

            
            

 
            

 
        }
    }
}
