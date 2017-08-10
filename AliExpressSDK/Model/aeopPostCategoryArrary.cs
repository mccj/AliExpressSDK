using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDK.Platform.AliExpressApi.Model
{
    public class aeopPostCategoryArrary
    {
        public List<aeopPostCategory> aeopPostCategoryList { get; set; }
    }
    public class aeopPostCategory
    {
        public string id { get; set; }
        public string level { get; set; }
        public bool isleaf { get; set; }
        public names names { get; set; }
    }
    public class names
    {
        public string zh { get; set; }
        public string pt { get; set; }
        public string fr { get; set; }
        public string en { get; set; }
        public string ru { get; set; }
        public string es { get; set; }
        public string @in { get; set; }
    }
}
