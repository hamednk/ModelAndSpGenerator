using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPGenerator.DataModel
{
    public class Settings
    {
        public string prefixWhereParameter = "@w_";
        public string prefixInputParameter = "@p_";
        public string prefixInsertSp = "usp_";
        public string prefixUpdateSp = "usp_";
        public string prefixGetSp = "usp_";
        public string prefixModelGenSp = "usp_";
        public string  errorHandling = "Yes";

    }
}
