using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBridgeBuilder.SXGJG
{
    public static class ListTools
    {

        public static void Add2(this List<int> list, int item,int times=1)
        {
            int x0 = list.Last();
            for (int i = 0; i < times; i++)
            {
                x0 += item;
                list.Add(x0);
            }
        }
    }


}
