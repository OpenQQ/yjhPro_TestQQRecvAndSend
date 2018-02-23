using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QQController.DAL;

namespace QQController
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Boolean createdNew;
            System.Threading.Mutex instance = new System.Threading.Mutex(true, "TestSingleStart", out createdNew); //同步基元变量   
            if (createdNew)
            {
                using (var dbcontext = new MainDbContext())
                {
                    var objectContext = ((IObjectContextAdapter)dbcontext).ObjectContext;
                    var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
                    mappingCollection.GenerateViews(new List<EdmSchemaError>());
                    //对程序中定义的所有DbContext逐一进行这个操作
                }

                foreach (var p in Process.GetProcessesByName("CQA"))
                {
                    p.Kill();
                    p.Close();
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                MessageBox.Show("已经启动了一个程序，请先退出","错误");
                Application.Exit();
            }
            
        }
    }
}
