using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQController.Entity.ViewModel
{
    public class SendedMessageOutViewModel:ICloneable
    {
        public string From { set; get; }

        public string To { set; get; }

        public string Message { set; get; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
