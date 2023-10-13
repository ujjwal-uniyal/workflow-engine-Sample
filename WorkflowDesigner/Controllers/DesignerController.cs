using System.Collections.Specialized;
using System.Text;
using OptimaJet.Workflow;
using WorkflowLib;

using System.Web.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace WorkflowDesigner.Controllers
{
    public class DesignerController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> API()
        {
            Stream filestream = null;
            if (Request.Files.Count > 0)
                filestream = Request.Files[0].InputStream;

            var pars = new NameValueCollection();
            pars.Add(Request.QueryString);

            if (Request.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            {
                var parsKeys = pars.AllKeys;
                //foreach (var key in Request.Form.AllKeys)
                foreach (string key in Request.Form.Keys)
                {
                    if (!parsKeys.Contains(key))
                    {
                        pars.Add(key, Request.Unvalidated[key]);
                    }
                }
            }

            (string res, bool hasError) = await WorkflowInit.Runtime.DesignerAPIAsync(pars, filestream);

            var operation = pars["operation"].ToLower();
            if (operation == "downloadscheme" && !hasError)
                return File(Encoding.UTF8.GetBytes(res), "text/xml");
            else if (operation == "downloadschemebpmn" && !hasError)
                return File(UTF8Encoding.UTF8.GetBytes(res), "text/xml");

            return Content(res);
        }


    }
}