using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace DuSwToglTFUnitTest
{
    public class swAppGeter
    {
       public ISldWorks ActiveSwApp
        {
            get
            {
                try
                {

                    var progId = "SldWorks.Application";

                    var progType = System.Type.GetTypeFromProgID(progId);

                    var app = System.Activator.CreateInstance(progType) as SolidWorks.Interop.sldworks.ISldWorks;
                    app.Visible = true;
                    return app;
                }
                catch 
                {
                    return null;
                }
            }
        }
     public ISldWorks NewSwApp
        {

            get { return new SldWorks(); }
        }
    }
}
