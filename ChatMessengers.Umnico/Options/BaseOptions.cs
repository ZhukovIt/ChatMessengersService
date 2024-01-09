using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiMed.ChatMessengers.Umnico
{
    public abstract class BaseOptions
    {
        protected BaseOptions() { }

        public abstract string Pack();
        public abstract BaseOptions Unpack(string _Source);
    }
}
