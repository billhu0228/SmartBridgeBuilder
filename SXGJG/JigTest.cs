using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using QRCoder;
using System;

[assembly: CommandClass(typeof(SmartBridgeBuilder.SXGJG.Commands))]
namespace SmartBridgeBuilder.SXGJG
{

    public class Commands
    {

        [CommandMethod("QR")]
        static public void QRCodeIn()
        {
            // Get the current value from a system variable
            int nMaxSort = Convert.ToInt32(Application.GetSystemVariable("FILLMODE"));

            // Set system variable to new value
            Application.SetSystemVariable("MAXSORT", 100);


            // 生成二维码
            string theGUID = Guid.NewGuid().ToString();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(
                "沙溪大桥钢结构加工图纸二维码测试.",
                QRCodeGenerator.ECCLevel.M, true, true,
                QRCodeGenerator.EciMode.Utf8, 5);
            QRCode code = new QRCode(qrCodeData);

            //新建块
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = doc.Database;
            Editor ed = doc.Editor;
            Transaction tr = doc.TransactionManager.StartTransaction();
            using (tr)
            {
                BlockTableRecord ms = (BlockTableRecord)tr.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite);
                BlockTable bt = (BlockTable)tr.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = new BlockTableRecord();

                btr.Name = theGUID;
                bt.UpgradeOpen();
                bt.Add(btr);
                tr.AddNewlyCreatedDBObject(btr, true);


                double x, y;
                double size = 0.5;
                double Length= qrCodeData.ModuleMatrix.Count * size;
                y = 0;
                foreach (var array in qrCodeData.ModuleMatrix)
                {
                    y -= size;
                    x = 0;
                    foreach (var ele in array)
                    {
                        x += size;
                        if ((bool)ele)
                        {
                            // Create a quadrilateral (square) solid in Model space
                            using (Solid sd = new Solid(
                                new Point3d(x, y, 0), new Point3d(x + size, y, 0),
                                new Point3d(x, y - size, 0), new Point3d(x + size, y - size, 0)))
                            {
                                btr.AppendEntity(sd);
                                tr.AddNewlyCreatedDBObject(sd, true);
                            }
                        }
                    }                  
                }
                Polyline box = new Polyline() { Closed = true };
                box.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                box.AddVertexAt(1, new Point2d(Length+size*2, 0), 0, 0, 0);
                box.AddVertexAt(2, new Point2d(Length + size * 2, -Length- size * 2), 0, 0, 0);
                box.AddVertexAt(3, new Point2d(0, -Length - size * 2), 0, 0, 0);
                btr.AppendEntity(box);
                tr.AddNewlyCreatedDBObject(box, true);

                // 插入块

                ObjectId blkRecId = bt[theGUID];
                BlockReference acBlkRef = new BlockReference(new Point3d(), blkRecId);


                ms.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);

                BlockPlacementJig bj = new BlockPlacementJig(tr, acCurDb, acBlkRef);

                PromptStatus stat = PromptStatus.Keyword;
                while (stat == PromptStatus.Keyword)
                {

                    PromptResult res = ed.Drag(bj);
                    stat = res.Status;
                    if (stat != PromptStatus.OK && stat != PromptStatus.Keyword)
                    {
                        return;
                    }

                }

                tr.Commit();
                                                                                           
            }
        }

        private class BlockPlacementJig : EntityJig
        {

            // Declare some internal state

            Database _db;
            Transaction _tr;
            Point3d _position;
            double _factor;

            // Constructor

            public BlockPlacementJig(Transaction tr, Database db, Entity ent) : base(ent)
            {
                _db = db;
                _tr = tr;                
                _factor = 1;
            }
            protected override SamplerStatus Sampler(JigPrompts jp)
            {
                // We acquire a point but with keywords

                JigPromptPointOptions po = new JigPromptPointOptions("\n选择二维码位置:");

                po.UserInputControls =
                  (UserInputControls.Accept3dCoordinates |
                    UserInputControls.NullResponseAccepted |
                    UserInputControls.NoNegativeResponseAccepted |
                    UserInputControls.GovernedByOrthoMode);
                                

                PromptPointResult ppr = jp.AcquirePoint(po);

                if (ppr.Status == PromptStatus.OK)
                {
                    // Check if it has changed or not (reduces flicker)

                    if (_position.DistanceTo(ppr.Value) < Tolerance.Global.EqualPoint)
                    {
                        return SamplerStatus.NoChange;
                    }

                    _position = ppr.Value;
                    return SamplerStatus.OK;
                }

                return SamplerStatus.Cancel;
            }

            protected override bool Update()
            {
                // Set properties on our text object

                BlockReference blkref = (BlockReference)Entity;

                blkref.Position = _position;
                blkref.ScaleFactors = new Scale3d(_factor);

                return true;
            }
        }
    }
}
