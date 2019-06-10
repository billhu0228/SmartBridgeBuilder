using SmartBridgeBuilder.CableStayed;
using System;
using System.IO;
using Tekla.Structures.Model;

namespace SmartBridgeBuilder
{
    class Program
    {
        static void Main(string[] args)
        {

            // 1、配置路径[cad路径、自定义库路径]；
            string AcadDir = @"D:\Program Files\Autodesk\AutoCAD 2018\";
            string LibsDir = @"D:\Bill\source\repos\SmartBridgeBuilder\OutLibs\";

            // 2、将dll(2)(3)从自定义库路径复制到cad路径；
            if (!CopyDLL(LibsDir, AcadDir))
            {
                return;
            }

            // 3、实例化Bridge类；
            // 4、Bridge类在cad路径写出CSV参数表；
            Bridge Bpjdq = new Bridge(720000, 256000, 1685000, 1518000, 1416000, 1438000);
            //Bridge Bpjdq = new Bridge(1000000, 356000, 1685000, 1518000, 1416000, 1438000);

            // 5、Bridge类调用Build公共方法，连接Tekla建模、输出IFC模型
            Model myModel = new Model(); // Tekla指针                       
            Bpjdq.Build(ref myModel, "D:\\OutPut\\IFC_23");

            // 6、Bridge类调用Draw公共方法，在cad路径写出scr脚本、调用accoreconsole控制台应用，执行脚本，输出PDF；
            Bpjdq.Draw(@"D:\OutPut\PDF_314",AcadDir);

            // 退出
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        private static bool CopyDLL( string LibsDir,  string AcadDir)
        {
            try
            {
                // Copy DLL 文件
                string DllName = "SmartBridgeBuilder.CableStayed.Drawer.dll";
                string ExtName = "SmartBridgeBuilder.Extension.dll";
                File.Copy(Path.Combine(LibsDir, DllName), Path.Combine(AcadDir, DllName), true);
                File.Copy(Path.Combine(LibsDir, ExtName), Path.Combine(AcadDir, ExtName), true);             
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }
    }
}
