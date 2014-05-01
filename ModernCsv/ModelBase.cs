using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernCsv
{
    [Serializable]
    public abstract class ModelBase
    {
        public bool IsValid { get; set; }
        public int LineNumber { get; set; }
        public string ErrorMessage { get; set; }
    }
}
