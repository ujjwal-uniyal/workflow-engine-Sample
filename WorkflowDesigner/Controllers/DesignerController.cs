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
        public async Task<ActionResult> Api()
        {
            Stream filestream = null;
            var parameters = new NameValueCollection();

            //Defining the request method
            var isPost = Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase);

            //Parse the parameters in the query string
            foreach (var q in Request.QueryString)
            {
                parameters.Add(q.ToString(), Request.QueryString[q.ToString()]);
            }

            if (isPost)
            {
                //Parsing the parameters passed in the form
                var keys = parameters.AllKeys;

                foreach (string key in keys)
                {
                    if (!keys.Contains(key))
                    {
                        parameters.Add(key, Request.Form[key].ToString());
                    }
                }

                //If a file is passed
                if (Request.Files.Count > 0)
                {
                    //Save file
                    filestream = Request.Files[0].InputStream;
                }
            }

            //Calling the Designer Api and store answer
            var (result, hasError) = await WorkflowInit.Runtime.DesignerAPIAsync(parameters, filestream);

            //If it returns a file, send the response in a special way
            if (parameters["operation"]?.ToLower() == "downloadscheme" && !hasError)
                return File(Encoding.UTF8.GetBytes(result), "text/xml");

            if (parameters["operation"]?.ToLower() == "downloadschemebpmn" && !hasError)
                return File(Encoding.UTF8.GetBytes(result), "text/xml");

            //response
            return Content(result);
        }
    }
}