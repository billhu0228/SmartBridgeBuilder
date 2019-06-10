using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBridgeBuilder.BOQ
{
    public class BoQItem
    {
        public string bridge;
        public string type;
        public string loc;
        public string detial;
        public string name;
        public string spec;
        public double quantity;
        public double quantity2;
        public int xmh;
        public int NewID;

        public BoQItem()
        {
            bridge = "";
            type = "";
            loc = "";
            detial = "";
            name = "";
            spec = "";
            quantity = 0;
            quantity2 = 0;
            xmh = 0;
            NewID = 0;
        }

        public BoQItem(string bridge, string type, string loc, string detial, string name, string spec, double quantity, double quantity2, int xmh, int newID)
        {
            this.bridge = bridge;
            this.type = type;
            this.loc = loc;
            this.detial = detial;
            this.name = name;
            this.spec = spec;
            this.quantity = quantity;
            this.quantity2 = quantity2;
            this.xmh = xmh;
            NewID = newID;
        }
    }
    public class BoQ
    {
        List<BoQItem> BoQList;

        public BoQ()
        {
            BoQList = new List<BoQItem>();
        }




        public void Add(BoQItem bt)
        {
            BoQList.Add(bt);
        }


        public void WriteCSV(string file_name)
        {
            StreamWriter fw = new StreamWriter(file_name, false, Encoding.GetEncoding("GBK"));

            fw.WriteLine("bridge,type,loc,detial,name,spec,quantity,quantity2,xmh,NewID");
            fw.Flush();
            foreach (var item in BoQList)
            {
                var ff = new string[] { item.bridge, item.type,item.loc,item.detial,item.name,item.spec,
                    item.quantity.ToString(),item.quantity2.ToString(),item.xmh.ToString(),item.NewID.ToString() };
                fw.WriteLine(ff.Together());
                fw.Flush();
            }
            fw.Close();
        }

    }




    public static class ExtForData
    {
        public static string Together(this string[] arr, string sep = ",")
        {
            string res = "";
            for (int i = 0; i < arr.Length; i++)
            {
                res += arr[i];
                if (i != arr.Length - 1)
                {
                    res += sep;
                }

            }
            return res;

        }

        public static void DataTableToCSV(this DataTable dt, string file_name)
        {
            StreamWriter fw = null;
            try
            {
                fw = new StreamWriter(file_name, false, Encoding.GetEncoding("GBK"));
                //写入表头

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    fw.Write(dt.Columns[i].ColumnName);
                    if (i != dt.Columns.Count - 1)
                    {
                        fw.Write(",");

                    }
                    else
                    {
                        fw.Write("\n");
                    }
                    fw.Flush();
                }



                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] arrBody = new string[dt.Columns.Count];
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        try
                        {
                            var f = dt.Rows[i][j];
                            if (f.GetType() == typeof(List<int>))
                            {
                                List<int> data = (List<int>)dt.Rows[i][j];
                                string output = string.Join("|", data);
                                arrBody[j] = output;
                            }
                            else if (f.GetType() == typeof(List<double>))
                            {
                                List<double> data = (List<double>)dt.Rows[i][j];
                                var m = from a in data select string.Format("{0:F1}", a);
                                string output = string.Join("|", m.ToList());
                                arrBody[j] = output;
                            }
                            else
                            {
                                arrBody[j] = f.ToString();
                            }

                        }
                        catch (Exception)
                        {

                        }
                    }
                    //arrBody[dt.Columns.Count] = GetInfo(arrBody[0], ref lkbob, ref rkbob);
                    fw.WriteLine(arrBody.Together());
                    fw.Flush();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                fw.Close();
            }
        }

    }
}
