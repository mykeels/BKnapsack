using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BKnapsack.Tests.Common;

namespace BKnapsack.Tests.Helpers
{
    public class FileHelper
    {
        private static FileHelper _fileHelper;

        private FileHelper()
        {

        }

        public static FileHelper GetFileHelper()
        {
            if (_fileHelper == null) _fileHelper = new FileHelper();
            return _fileHelper;
        }

        public string GetDataFilePath(string path)
        {
            return $"{Constants.DataPath}{path}";
        }
    }
}
