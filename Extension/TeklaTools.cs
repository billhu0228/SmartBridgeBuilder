using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace SmartBridgeBuilder.Extension
{
    public static class TeklaTools
    {
        public static void SpaceClean(ref Model myModel)
        {
            ModelObjectEnumerator Enumerator = myModel.GetModelObjectSelector().GetAllObjects();
            while (Enumerator.MoveNext())
            {
                ModelObject mo = Enumerator.Current as ModelObject;
                if (mo == null)
                {
                    continue;
                }
                
                else
                {
                    mo.Delete();
                }

                //Type ObjectType = mo.GetType();
                //while (ObjectType != typeof(Beam) && ObjectType != typeof(ContourPlate) && ObjectType != typeof(Brep) && ObjectType.BaseType != null)
                //{
                //    ObjectType = ObjectType.BaseType;
                //}


                //if (ObjectType == typeof(Beam))
                //{
                //    Beam curBm = Enumerator.Current as Beam;
                //    curBm.Delete();
                //}

                //if (ObjectType == typeof(ContourPlate))
                //{
                //    ContourPlate curBm = Enumerator.Current as ContourPlate;
                //    curBm.Delete();
                //}
                //if (ObjectType == typeof(Brep))
                //{
                //    Brep curBm = Enumerator.Current as Brep;
                //    curBm.Delete();
                //}
                myModel.CommitChanges();
            }



            
        }

        public static void ExportIFC(string outputFileName)
        {
            var componentInput = new ComponentInput();
            componentInput.AddOneInputPosition(new Point(0, 0, 0));
            var comp = new Component(componentInput)
            {
                Name = "ExportIFC",
                Number = BaseComponent.PLUGIN_OBJECT_NUMBER
            };

            // Parameters
            comp.SetAttribute("OutputFile", outputFileName);
            comp.SetAttribute("Format", 0);
            comp.SetAttribute("ExportType", 1);
            //comp.SetAttribute("AdditionalPSets", "");
            comp.SetAttribute("CreateAll", 1);  // 0 to export only selected objects

            // Advanced
            comp.SetAttribute("Assemblies", 1);
            comp.SetAttribute("Bolts", 1);
            comp.SetAttribute("Welds", 0);
            comp.SetAttribute("SurfaceTreatments", 1);

            comp.SetAttribute("BaseQuantities", 1);
            comp.SetAttribute("GridExport", 1);
            comp.SetAttribute("ReinforcingBars", 1);
            comp.SetAttribute("PourObjects", 1);

            comp.SetAttribute("LayersNameAsPart", 1);
            comp.SetAttribute("PLprofileToPlate", 0);
            comp.SetAttribute("ExcludeSnglPrtAsmb", 0);

            comp.SetAttribute("LocsFromOrganizer", 0);

            comp.Insert();
        }

        public static void CreatBeam(Point a, Point f, string v, Position.DepthEnum depth,double deg=0)
        {
            Beam curBeam = new Beam(a, f);
            curBeam.Profile.ProfileString = v;
            curBeam.Position.Depth = depth;
            curBeam.Position.Rotation = Position.RotationEnum.TOP;
            curBeam.Position.RotationOffset = deg;
            curBeam.Insert();
        }

        public static void CreatBeam(Point a, Point f, string v, Position.RotationEnum rot)
        {
            Beam curBeam = new Beam(a, f);
            curBeam.Profile.ProfileString = v;
            curBeam.Position.Depth = Position.DepthEnum.MIDDLE;
            curBeam.Position.Rotation = rot;
            curBeam.Insert();
        }

        public static void CreatPlate(Point a, Point b, Point c, Point d, int thick)
        {
            ContourPlate curPlate = new ContourPlate();

            Profile pf = new Profile();
            pf.ProfileString = string.Format("PL{0}", thick);
            var contor = new Contour();
            contor.AddContourPoint(new ContourPoint(a, null));
            contor.AddContourPoint(new ContourPoint(b, null));
            contor.AddContourPoint(new ContourPoint(c, null));
            contor.AddContourPoint(new ContourPoint(d, null));

            curPlate.Contour = contor;
            curPlate.Profile = pf;
            curPlate.Insert();

        }
                                    
    }
}
