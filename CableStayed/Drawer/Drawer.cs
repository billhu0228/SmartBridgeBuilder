using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using SmartBridgeBuilder.Extension;
using Autodesk.AutoCAD.Colors;

[assembly: CommandClass(typeof(SmartBridgeBuilder.CableStayed.Drawer))]


namespace SmartBridgeBuilder.CableStayed
{
    public class Drawer
    {

        //-------------------------------------------------------------------------------------------
        [CommandMethod("sbbini")]
        public static void CADini()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor ed = acDoc.Editor;
            // Start a transaction
            using (Transaction tr = acCurDb.TransactionManager.StartTransaction())
            {
                Dictionary<string, short> ldic = new Dictionary<string, short>()
                {
                    ["粗线"] = 4,
                    ["细线"] = 2,
                    ["标注"] = 7,
                    ["中心线"] = 1,
                    ["虚线"] = 3,
                    ["填充"] = 8,
                    ["图框"] = 8,
                    ["地质"] = 8,
                };
                List<string> Lname = new List<string>() { "CENTER", "DASHED" };
                LayerTable acLyrTbl;
                acLyrTbl = tr.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;
                LinetypeTable acLinTbl;
                acLinTbl = tr.GetObject(acCurDb.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                foreach (string ltname in Lname)
                {
                    if (!acLinTbl.Has(ltname))
                    {
                        acCurDb.LoadLineTypeFile(ltname, "acad.lin");
                    }
                }
                LayerTableRecord acLyrTblRec = new LayerTableRecord();
                foreach (string key in ldic.Keys)
                {
                    short cid = ldic[key];
                    acLyrTblRec = new LayerTableRecord();
                    if (!acLyrTbl.Has(key))
                    {
                        acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, cid);
                        if (cid != 4) { acLyrTblRec.LineWeight = LineWeight.LineWeight013; }
                        else { acLyrTblRec.LineWeight = LineWeight.LineWeight030; }
                        if (cid == 1) { acLyrTblRec.LinetypeObjectId = acLinTbl["CENTER"]; }
                        if (cid == 3) { acLyrTblRec.LinetypeObjectId = acLinTbl["DASHED"]; }
                        if (key == "图框") { acLyrTblRec.IsPlottable = false; }
                        if (key == "地质") { acLyrTblRec.IsPlottable = false; }
                        acLyrTblRec.Name = key;
                        if (acLyrTbl.IsWriteEnabled == false) acLyrTbl.UpgradeOpen();
                        acLyrTbl.Add(acLyrTblRec);
                        tr.AddNewlyCreatedDBObject(acLyrTblRec, true);
                    }
                    else
                    {
                        acLyrTblRec = tr.GetObject(acLyrTbl[key], OpenMode.ForWrite) as LayerTableRecord;
                        acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, cid);
                        if (cid != 4) { acLyrTblRec.LineWeight = LineWeight.LineWeight013; }
                        else { acLyrTblRec.LineWeight = LineWeight.LineWeight030; }
                        if (cid == 1) { acLyrTblRec.LinetypeObjectId = acLinTbl["CENTER"]; }
                        if (cid == 3) { acLyrTblRec.LinetypeObjectId = acLinTbl["DASHED"]; }
                        if (key == "图框") { acLyrTblRec.IsPlottable = false; }
                        if (key == "地质") { acLyrTblRec.IsPlottable = false; }
                    }
                }
                if (!acLyrTbl.Has("sjx"))
                {
                    acLyrTblRec = new LayerTableRecord();
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
                    acLyrTblRec.Name = "sjx";
                    acLyrTblRec.LineWeight = LineWeight.LineWeight015;
                    if (acLyrTbl.IsWriteEnabled == false) acLyrTbl.UpgradeOpen();
                    acLyrTbl.Add(acLyrTblRec);
                    tr.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }
                if (!acLyrTbl.Has("dmx"))
                {
                    acLyrTblRec = new LayerTableRecord();
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 8);
                    acLyrTblRec.Name = "dmx";
                    acLyrTblRec.LineWeight = LineWeight.LineWeight015;
                    if (acLyrTbl.IsWriteEnabled == false) acLyrTbl.UpgradeOpen();
                    acLyrTbl.Add(acLyrTblRec);
                    tr.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }


                //-------------------------------------------------------------------------------------------
                TextStyleTable st = tr.GetObject(acCurDb.TextStyleTableId, OpenMode.ForWrite) as TextStyleTable;
                if (!st.Has("EN"))
                {
                    TextStyleTableRecord str = new TextStyleTableRecord()
                    {
                        Name = "En",
                        FileName = "times.ttf",
                        XScale = 0.85,
                    };
                    st.Add(str);
                    tr.AddNewlyCreatedDBObject(str, true);
                }
                else
                {
                    TextStyleTableRecord str = tr.GetObject(st["En"], OpenMode.ForWrite) as TextStyleTableRecord;
                    str.FileName = "times.ttf";
                    str.XScale = 0.85;
                }
                if (!st.Has("fsdb"))
                {
                    TextStyleTableRecord str2 = new TextStyleTableRecord()
                    {
                        Name = "fsdb",
                        FileName = "fsdb_e.shx",
                        BigFontFileName = "fsdb.shx",
                        XScale = 0.75,
                    };
                    ObjectId textstyleid = st.Add(str2);
                    tr.AddNewlyCreatedDBObject(str2, true);
                }
                else
                {
                    TextStyleTableRecord str = tr.GetObject(st["fsdb"], OpenMode.ForWrite) as TextStyleTableRecord;
                    str.FileName = "fsdb_e.shx";
                    str.BigFontFileName = "fsdb.shx";
                    str.XScale = 0.75;
                }
                if (!st.Has("仿宋"))
                {
                    TextStyleTableRecord str2 = new TextStyleTableRecord()
                    {
                        Name = "仿宋",
                        FileName = "仿宋_GB2312.ttf",
                        XScale = 0.8,
                    };
                    ObjectId textstyleid = st.Add(str2);
                    tr.AddNewlyCreatedDBObject(str2, true);
                }
                else
                {
                    TextStyleTableRecord str = tr.GetObject(st["仿宋"], OpenMode.ForWrite) as TextStyleTableRecord;
                    str.FileName = "仿宋_GB2312.ttf";
                    str.XScale = 0.8;
                }
                if (!st.Has("钢筋"))
                {
                    TextStyleTableRecord str2 = new TextStyleTableRecord()
                    {
                        Name = "钢筋",
                        FileName = "FS-GB2312-Rebar.ttf",
                        XScale = 0.8,
                    };
                    ObjectId textstyleid = st.Add(str2);
                    tr.AddNewlyCreatedDBObject(str2, true);
                }
                else
                {
                    TextStyleTableRecord str = tr.GetObject(st["钢筋"], OpenMode.ForWrite) as TextStyleTableRecord;
                    str.FileName = "FS-GB2312-Rebar.ttf";
                    str.XScale = 0.8;
                }
                //-------------------------------------------------------------------------------------------
                DimStyleTable dst = (DimStyleTable)tr.GetObject(acCurDb.DimStyleTableId, OpenMode.ForWrite);
                foreach (int thescale in new int[] {1000, 2000,5000 })
                {
                    string scname = "1-" + thescale.ToString();
                    DimStyleTableRecord dstr = new DimStyleTableRecord();
                    if (!dst.Has(scname))
                    {
                        dstr.Name = "1-" + thescale.ToString();
                        dstr.Dimscale = thescale;
                        dstr.Dimtxsty = st["仿宋"];
                        dstr.Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimdli = 5.0;
                        dstr.Dimexe = 1.0;
                        dstr.Dimexo = 1.0;
                        dstr.DimfxlenOn = true;
                        dstr.Dimfxlen = 4;
                        dstr.Dimtxt = 2.5;
                        dstr.Dimasz = 1.5;
                        dstr.Dimtix = true;
                        dstr.Dimtmove = 1;
                        dstr.Dimtad = 1;
                        dstr.Dimgap = 0.8;
                        dstr.Dimdec = 0;
                        dstr.Dimtih = false;
                        dstr.Dimtoh = false;
                        dstr.Dimdsep = '.';
                        //dstr.Dimlfac = 0.1;
                        dst.Add(dstr);
                        tr.AddNewlyCreatedDBObject(dstr, true);
                    }
                    else
                    {
                        dstr = tr.GetObject(dst[scname], OpenMode.ForWrite) as DimStyleTableRecord;
                        dstr.Name = "1-" + thescale.ToString();
                        dstr.Dimscale = thescale;
                        dstr.Dimtxsty = st["fsdb"];
                        dstr.Dimclrd = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimclre = Color.FromColorIndex(ColorMethod.ByAci, 6);
                        dstr.Dimdli = 5.0;
                        dstr.Dimexe = 1.0;
                        dstr.Dimexo = 1.0;
                        dstr.DimfxlenOn = true;
                        dstr.Dimfxlen = 4;
                        dstr.Dimtxt = 2.5;
                        dstr.Dimasz = 1.5;
                        dstr.Dimtix = true;
                        dstr.Dimtmove = 1;
                        dstr.Dimtad = 1;
                        dstr.Dimgap = 0.8;
                        dstr.Dimdec = 0;
                        dstr.Dimtih = false;
                        dstr.Dimtoh = false;
                        dstr.Dimdsep = '.';
                        dstr.Dimlfac = 0.1;
                    }

                }
                //-------------------------------------------------------------------------------------------
                // 自定义块
                //-------------------------------------------------------------------------------------------
                BlockTable bt = (BlockTable)tr.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = new BlockTableRecord();

                //-------------------------------------------------------------------------------------------
                if (!bt.Has("BG"))
                {
                    btr.Name = "BG";
                    bt.UpgradeOpen();
                    bt.Add(btr);
                    tr.AddNewlyCreatedDBObject(btr, true);
                    Polyline Paa = new Polyline()
                    {
                        //Color = Color.FromColorIndex(ColorMethod.ByAci, 9),
                        //Layer = "标注",
                    };
                    Paa.AddVertexAt(0, new Point2d(0, 0), 0, 0, 200);
                    Paa.AddVertexAt(1, new Point2d(0, 200), 0, 0, 0);
                    btr.AppendEntity(Paa);
                    tr.AddNewlyCreatedDBObject(Paa, true);
                    AttributeDefinition curbg = new AttributeDefinition();
                    curbg.Position = new Point3d(120, 200, 0);
                    curbg.Height = 250;
                    curbg.WidthFactor = 0.75;
                    curbg.Tag = "标高";
                    //curbg.Layer = "标注";
                    curbg.TextStyleId = st["fsdb"];
                    btr.AppendEntity(curbg);
                    tr.AddNewlyCreatedDBObject(curbg, true);
                }
                //-------------------------------------------------------------------------------------------
                if (!bt.Has("ZP"))
                {
                    BlockTableRecord btr2 = new BlockTableRecord();
                    btr2.Name = "ZP";
                    bt.UpgradeOpen();
                    bt.Add(btr2);
                    tr.AddNewlyCreatedDBObject(btr2, true);
                    Polyline Paa2 = new Polyline()
                    {
                        Color = Color.FromColorIndex(ColorMethod.ByAci, 9),
                    };
                    Paa2.AddVertexAt(0, new Point2d(0 - 350, 0), 0, 0, 80);
                    Paa2.AddVertexAt(1, new Point2d(200 - 350, 0), 0, 0, 0);
                    Paa2.AddVertexAt(2, new Point2d(900 - 350, 0), 0, 0, 0);
                    btr2.AppendEntity(Paa2);
                    tr.AddNewlyCreatedDBObject(Paa2, true);
                    AttributeDefinition curzp = new AttributeDefinition();
                    curzp.Position = new Point3d(220 - 350, 0, 0);
                    curzp.Height = 250;
                    curzp.WidthFactor = 0.75;
                    curzp.Tag = "左坡";
                    curzp.TextStyleId = st["fsdb"];
                    btr2.AppendEntity(curzp);
                    tr.AddNewlyCreatedDBObject(curzp, true);
                }

                //-------------------------------------------------------------------------------------------
                if (!bt.Has("YP"))
                {
                    BlockTableRecord btr3 = new BlockTableRecord();
                    btr3.Name = "YP";
                    bt.UpgradeOpen();
                    bt.Add(btr3);
                    tr.AddNewlyCreatedDBObject(btr3, true);
                    Polyline Paa3 = new Polyline()
                    {
                        Color = Color.FromColorIndex(ColorMethod.ByAci, 9),
                    };
                    Paa3.AddVertexAt(0, new Point2d(0 + 350, 0), 0, 0, 80);
                    Paa3.AddVertexAt(1, new Point2d(-200 + 350, 0), 0, 0, 0);
                    Paa3.AddVertexAt(2, new Point2d(-900 + 350, 0), 0, 0, 0);
                    btr3.AppendEntity(Paa3);
                    tr.AddNewlyCreatedDBObject(Paa3, true);
                    AttributeDefinition curyp = new AttributeDefinition();
                    curyp.Position = new Point3d(-220 + 350, 0, 0);
                    curyp.HorizontalMode = TextHorizontalMode.TextRight;
                    curyp.AlignmentPoint = curyp.Position;
                    curyp.Height = 250;
                    curyp.WidthFactor = 0.75;
                    curyp.Tag = "右坡";
                    curyp.TextStyleId = st["fsdb"];
                    btr3.AppendEntity(curyp);
                    tr.AddNewlyCreatedDBObject(curyp, true);
                }
                //-------------------------------------------------------------------------------------------

                //-------------------------------------------------------------------------------------------
                tr.Commit();
            }
        }
        //-------------------------------------------------------------------------------------------    
        [CommandMethod("PrintPDF")]
        public static void PlotLayout()
        {
            // Get the current document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            Editor ed = acDoc.Editor;

            PromptResult dir = ed.GetString("");
            

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Reference the Layout Manager
                LayoutManager acLayoutMgr = LayoutManager.Current;

                // Get the current layout and output its name in the Command Line window
                Layout acLayout = acTrans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout),
                                                    OpenMode.ForRead) as Layout;

                // Get the PlotInfo from the layout
                using (PlotInfo acPlInfo = new PlotInfo())
                {
                    acPlInfo.Layout = acLayout.ObjectId;

                    // Get a copy of the PlotSettings from the layout
                    using (PlotSettings acPlSet = new PlotSettings(acLayout.ModelType))
                    {
                        acPlSet.CopyFrom(acLayout);

                        // Update the PlotSettings object
                        PlotSettingsValidator acPlSetVdr = PlotSettingsValidator.Current;

                        // Set the plot type
                        //acPlSetVdr.SetPlotType(acPlSet, Autodesk.AutoCAD.DatabaseServices.PlotType.Window);

                        // Set the plot scale
                        //acPlSetVdr.SetUseStandardScale(acPlSet, true);
                        //acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.ScaleToFit);

                        // Center the plot
                        //acPlSetVdr.SetPlotCentered(acPlSet, true);

                        // Set the plot device to use
                        //acPlSetVdr.SetPlotConfigurationName(acPlSet, "DWF6 ePlot.pc3", "ANSI_A_(8.50_x_11.00_Inches)");

                        // Set the plot info as an override since it will
                        // not be saved back to the layout
                        acPlInfo.OverrideSettings = acPlSet;

                        // Validate the plot info
                        using (PlotInfoValidator acPlInfoVdr = new PlotInfoValidator())
                        {
                            acPlInfoVdr.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                            acPlInfoVdr.Validate(acPlInfo);

                            // Check to see if a plot is already in progress
                            if (PlotFactory.ProcessPlotState == ProcessPlotState.NotPlotting)
                            {
                                using (PlotEngine acPlEng = PlotFactory.CreatePublishEngine())
                                {
                                    // Track the plot progress with a Progress dialog
                                    using (PlotProgressDialog acPlProgDlg = new PlotProgressDialog(false, 1, true))
                                    {
                                        using ((acPlProgDlg))
                                        {
                                            // Define the status messages to display 
                                            // when plotting starts
                                            //acPlProgDlg.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Plot Progress");
                                            //acPlProgDlg.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job");
                                            //acPlProgDlg.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet");
                                            //acPlProgDlg.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress");
                                            //acPlProgDlg.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress");

                                            // Set the plot progress range
                                            //acPlProgDlg.LowerPlotProgressRange = 0;
                                            //acPlProgDlg.UpperPlotProgressRange = 100;
                                            //acPlProgDlg.PlotProgressPos = 0;

                                            // Display the Progress dialog
                                            //acPlProgDlg.OnBeginPlot();
                                            //acPlProgDlg.IsVisible = true;

                                            // Start to plot the layout
                                            acPlEng.BeginPlot(acPlProgDlg, null);

                                            // Define the plot output
                                            //acPlEng.BeginDocument(acPlInfo, acDoc.Name, null, 1, true, "C:\\Users\\Bill\\Desktop\\Test6PDF");
                                            acPlEng.BeginDocument(acPlInfo, acDoc.Name, null, 1, true, dir.StringResult);
                                            // Display information about the current plot
                                           // acPlProgDlg.set_PlotMsgString(PlotMessageIndex.Status, "Plotting: " + acDoc.Name + " - " + acLayout.LayoutName);

                                            // Set the sheet progress range
                                            //acPlProgDlg.OnBeginSheet();
                                            //acPlProgDlg.LowerSheetProgressRange = 0;
                                            //acPlProgDlg.UpperSheetProgressRange = 100;
                                            //acPlProgDlg.SheetProgressPos = 0;

                                            // Plot the first sheet/layout
                                            using (PlotPageInfo acPlPageInfo = new PlotPageInfo())
                                            {
                                                acPlEng.BeginPage(acPlPageInfo, acPlInfo, true, null);
                                            }

                                            acPlEng.BeginGenerateGraphics(null);
                                            acPlEng.EndGenerateGraphics(null);

                                            // Finish plotting the sheet/layout
                                            acPlEng.EndPage(null);
                                            //acPlProgDlg.SheetProgressPos = 100;
                                            //acPlProgDlg.OnEndSheet();

                                            // Finish plotting the document
                                            acPlEng.EndDocument(null);

                                            // Finish the plot
                                            //acPlProgDlg.PlotProgressPos = 100;
                                            //acPlProgDlg.OnEndPlot();
                                            acPlEng.EndPlot(null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //-------------------------------------------------------------------------------------------
        [CommandMethod("DrawBridge")]
        public static void DrawBridge()
        {
            //  CAD 基本指针
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            ObjectId paperSpace = db.CreatPaperSpace();

            // 读取参数
            string CSVPath =Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "ParaList.csv");
            //string CSVPath = Path.Combine(@"D:\OutPut\", "ParaList.csv");
            System.Data.DataTable dt = OpenCSV(CSVPath);            
            var tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "SizeMiddle" select a["value"] as string;
            int SizeMiddle = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "SizeSide" select a["value"] as string;
            int SizeSide = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "NumMiddle" select a["value"] as string;
            int NumMiddle = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "NumSide" select a["value"] as string;
            int NumSide = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "NumList_0" select a["value"] as string;
            int NumList_0 = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "NumList_1" select a["value"] as string;
            int NumList_1 = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "BeamHeight" select a["value"] as string;
            int BeamHeight = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "BeamWidth" select a["value"] as string;
            int BeamWidth = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "CableOnTower" select a["value"] as string;
            int CableOnTower = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "TowerAboveRoad" select a["value"] as string;
            int TowerAboveRoad = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "TowerBelowRoadLeft" select a["value"] as string;
            int TowerBelowRoadLeft = int.Parse(tmp.First());
            tmp = from DataRow a in dt.AsEnumerable() where (string)a["name"] == "TowerBelowRoadRight" select a["value"] as string;
            int TowerBelowRoadRight = int.Parse(tmp.First());


            // 配置参数
            int HalfLength = (NumMiddle * SizeMiddle + NumSide * SizeSide);
            int NumList_2 = (NumSide + NumMiddle) / 2 - NumList_0 - NumList_1;
            double xstart = -HalfLength;
            var ff = GetGridXString(HalfLength, new List<int>() { NumList_0, NumList_1, NumList_2 }, SizeMiddle, SizeSide, NumSide, NumMiddle).Item1.Split(' ');
            int xt1 = -HalfLength + int.Parse(ff[1]) + int.Parse(ff[2]) + int.Parse(ff[3]);
            int xt2 = -HalfLength + int.Parse(ff[1]) + int.Parse(ff[2]) + int.Parse(ff[3]) + int.Parse(ff[4]) * 2;

            
            //  主梁绘制
            Point2d A = new Point2d(xstart, 0);
            DrawBeam(ref db,A,BeamHeight, SizeSide,NumSide,true,true,false);
            int nn = (NumList_0 + NumList_1 + NumList_2 - NumSide);
            DrawBeam(ref db, A.Convert2D(NumSide * SizeSide), BeamHeight, SizeMiddle, nn, true, true, false);
            DrawBeam(ref db, A.Convert2D(xt1 - xstart), BeamHeight, SizeMiddle, NumMiddle - nn, false, true, false);
            DrawBeam(ref db, A.Convert2D(HalfLength), BeamHeight, SizeMiddle, NumMiddle - nn, true, true, false);
            DrawBeam(ref db, A.Convert2D(xt2-xstart), BeamHeight, SizeMiddle, nn,false, true, false);
            DrawBeam(ref db, A.Convert2D(xt2 - xstart+nn*SizeMiddle), BeamHeight, SizeMiddle, NumSide, false, true, true);



            // 桥塔绘制
            int z0Left =-TowerBelowRoadLeft;
            int z0Right = -TowerBelowRoadRight;
            int z1 = 0 - 55000;
            int z2 = 0 - BeamHeight - 1000;
            int z3 = TowerAboveRoad;
            int z4 = z3 + ((NumMiddle + NumSide) / 2) * CableOnTower;

            DrawTower(ref db, xt1, z0Left, z1, z2, z3, z4);
            DrawTower(ref db, xt2, z0Right, z1, z2, z3, z4);

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Layout lay = (Layout)tr.GetObject(paperSpace, OpenMode.ForWrite);

                var vpIds = lay.GetViewports();
                Viewport vpA, vpB;
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(lay.BlockTableRecordId, OpenMode.ForWrite);

                Viewport Vports = (Viewport)tr.GetObject(db.PaperSpaceVportId, OpenMode.ForRead);

                var viewportIDs = db.GetViewports(true);
                foreach (ObjectId id in viewportIDs)
                {
                    //通过遍历视口ID方式访问视口对象
                    Viewport vp1 = id.GetObject(OpenMode.ForWrite) as Viewport;

                    if (!vp1.Equals(Vports))
                    {
                        vp1.Erase();
                    }
                }


                vpA = new Viewport();
                btr.AppendEntity(vpA);
                tr.AddNewlyCreatedDBObject(vpA, true);
                vpA.On = true;
                vpA.GridOn = false;
                vpA.DrawMyViewport(1, Point3d.Origin, Point3d.Origin.Convert2D(), 5000);
                vpB = new Viewport();
                btr.AppendEntity(vpB);
                tr.AddNewlyCreatedDBObject(vpB, true);
                vpB.On = true;
                vpB.GridOn = false;
                vpB.DrawMyViewport(2, Point3d.Origin, Point3d.Origin.Convert2D(), 5000);

                tr.Commit();
            }



        }

        private static void DrawTower(ref Database db, int xt1,int z0, int z1, int z2, int z3, int z4)
        {

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                Point3d Pt1, Pt2, Pt3, Pt4;
                Point3d Pt0 = new Point3d(xt1 - 7000, z0, 0);
                Pt1 = new Point3d(xt1 - 6000, z1, 0);
                Pt2 = new Point3d(xt1 - 5500, z2, 0);
                Pt3 = new Point3d(xt1 - 7500 * 0.5, z3, 0);
                Pt4 = new Point3d(xt1 - 7500 * 0.5, z4, 0);


                Line L1 = new Line(Pt1,Pt1.Convert3D(12000));
                L1.Layer = "细线";
                modelSpace.AppendEntity(L1);
                tr.AddNewlyCreatedDBObject(L1, true);

                Line L2 = new Line(Pt2,Pt2.Convert3D(11000));
                L2.Layer = "细线";
                modelSpace.AppendEntity(L2);
                tr.AddNewlyCreatedDBObject(L2, true);

                Line L3 = new Line(Pt3,Pt3.Convert3D(7500));
                L3.Layer = "细线";
                modelSpace.AppendEntity(L3);
                tr.AddNewlyCreatedDBObject(L3, true);

                Polyline Tw = new Polyline()
                {
                    Closed = false,
                    Layer = "粗线",
                };
                Tw.AddVertexAt(0, Pt1.Convert2D(), 0, 0, 0);
                Tw.AddVertexAt(1, Pt2.Convert2D(), 0, 0, 0);
                Tw.AddVertexAt(2, Pt3.Convert2D(), 0, 0, 0);
                Tw.AddVertexAt(3, Pt4.Convert2D(), 0, 0, 0);
                Tw.AddVertexAt(4, Pt4.Convert2D(7500), 0, 0, 0);
                Tw.AddVertexAt(5, Pt3.Convert2D(7500), 0, 0, 0);
                Tw.AddVertexAt(6, Pt2.Convert2D(11000), 0, 0, 0);
                Tw.AddVertexAt(7, Pt1.Convert2D(12000), 0, 0, 0);
                modelSpace.AppendEntity(Tw);
                tr.AddNewlyCreatedDBObject(Tw, true);


                Polyline Base = new Polyline()
                {
                    Closed = true,
                    Layer = "粗线",
                };
                Base.AddVertexAt(0, Pt0.Convert2D(), 0, 0, 0);
                Base.AddVertexAt(1, Pt0.Convert2D(0,z1-z0), 0, 0, 0);
                Base.AddVertexAt(2, Pt0.Convert2D(14000,z1-z0), 0, 0, 0);
                Base.AddVertexAt(3, Pt0.Convert2D(14000), 0, 0, 0);
                modelSpace.AppendEntity(Base);
                tr.AddNewlyCreatedDBObject(Base, true);






                tr.Commit();
            }
        }

            /// <summary>
            /// 绘制主梁
            /// </summary>
            /// <param name="db"></param>
            /// <param name="StartPt">左上起点</param>
            /// <param name="heigth">梁高</param>
            /// <param name="size">节长</param>
            /// <param name="num">节数</param>
            /// <param name="isUp">是否左下向右上</param>
            /// <param name="isStartLine">是否有起点线</param>
            /// <param name="isEndLine">是否有终点线</param>
            private static void DrawBeam(ref Database db, Point2d StartPt,int heigth, int size, int num,bool isUp, bool isStartLine, bool isEndLine)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable blockTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord modelSpace = tr.GetObject(blockTbl[BlockTableRecord.ModelSpace],
                    OpenMode.ForWrite) as BlockTableRecord;

                Point3d A, B, C, D;
                double x0;                

                for (int i = 0; i < num; i++)
                {
                    x0 = StartPt.X + i * size;
                    A = new Point3d(x0, StartPt.Y, 0);
                    B = A.Convert3D(0, -heigth, 0);
                    C = B.Convert3D(size, 0, 0);
                    D = C.Convert3D(0, heigth, 0);

                    Line AD = new Line(A, D);
                    AD.Layer = "粗线";
                    modelSpace.AppendEntity(AD);
                    tr.AddNewlyCreatedDBObject(AD, true);

                    Line BC = new Line(B, C);
                    BC.Layer = "细线";
                    modelSpace.AppendEntity(BC);
                    tr.AddNewlyCreatedDBObject(BC, true);

                    if (isStartLine && i==0)
                    {
                        Line AB = new Line(A, B);
                        AB.Layer = "细线";
                        modelSpace.AppendEntity(AB);
                        tr.AddNewlyCreatedDBObject(AB, true);
                    }

                    if (isUp)
                    {
                        Line BD = new Line(B,D);
                        BD.Layer = "细线";
                        modelSpace.AppendEntity(BD);
                        tr.AddNewlyCreatedDBObject(BD, true);
                    }
                    else
                    {
                        Line AC = new Line(A,C);
                        AC.Layer = "细线";
                        modelSpace.AppendEntity(AC);
                        tr.AddNewlyCreatedDBObject(AC, true);
                    }
                    if (i != num - 1)
                    {
                        Line CD = new Line(C, D);
                        CD.Layer = "细线";
                        modelSpace.AppendEntity(CD);
                        tr.AddNewlyCreatedDBObject(CD, true);
                    }
                    if (i==num-1 && isEndLine)
                    {
                        Line CD = new Line(C, D);
                        CD.Layer = "粗线";
                        modelSpace.AppendEntity(CD);
                        tr.AddNewlyCreatedDBObject(CD, true);
                    }
                }
                tr.Commit();
            }
        }

        static Tuple<string, string> GetGridXString(int HalfLength, List<int> NumList, int SizeMiddle, int SizeSide, int NumSide, int NumMiddle)
        {
            List<int> res = new List<int>();
            int x0 = -HalfLength;
            res.Add(x0);

            int i = 1;
            while (i <= NumList.Sum())
            {
                if (i <= NumSide)
                {
                    x0 += SizeSide;

                }
                else
                {
                    x0 += SizeMiddle;
                }
                if (i == NumList[0] || i == (NumList[0] + NumList[1]) || i == (NumSide + NumMiddle) / 2)
                {
                    res.Add(x0);
                }
                i++;
            }
            var aa = res.ToList();
            foreach (int item in aa)
            {
                res.Add(-item);
            }
            res.Add(0);
            res.Sort();

            var tmp2 = new List<int>();
            for (int ii = 0; ii < res.Count; ii++)
            {
                if (ii == 0)
                {
                    tmp2.Add(res[ii]);
                }
                else
                {
                    tmp2.Add(res[ii] - res[ii - 1]);
                }
            }
            var strres = from a in tmp2 select a.ToString();




            string s1 = string.Join(" ", strres.ToArray());
            string s2 = string.Join(" ", Enumerable.Range(0, (int)strres.LongCount()));
            return new Tuple<string, string>(s1, s2);
        }

















        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static System.Data.DataTable OpenCSV(string filePath)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            //StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            //string fileContent = sr.ReadToEnd();
            //encoding = sr.CurrentEncoding;
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                //strLine = Common.ConvertStringUTF8(strLine, encoding);
                //strLine = Common.ConvertStringUTF8(strLine);

                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        System.Data.DataColumn dc = new System.Data.DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }
















    }
}
