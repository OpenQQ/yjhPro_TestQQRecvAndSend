using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQController.Common
{
    public class Logger
    {
        public static void Info(string info)
        {
            Console.WriteLine("{0}\tINFO\tMessage: {1}", DateTime.Now, info);
        }

        public static void Debug(string info)
        {
            Console.WriteLine("{0}\tDEBUG\tMessage: {1}", DateTime.Now, info);
        }

        public static void Warn(string info)
        {
            Console.WriteLine("{0}\tWARN\tMessage: {1}", DateTime.Now, info);
        }

        public static void Error(string info)
        {
            Console.WriteLine("{0}\tERROR\tMessage: {1}", DateTime.Now, info);
        }
        
    }
}
