using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace BKnapsack.Tests.Common
{
    public class Constants
    {
        public static string DataPath = ConfigurationManager.AppSettings["DataPath"];
    }
}
