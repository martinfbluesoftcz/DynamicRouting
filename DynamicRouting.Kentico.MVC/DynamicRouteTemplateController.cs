﻿using CMS.Base;
using CMS.DocumentEngine;
using DynamicRouting.Implementations;
using DynamicRouting.Interfaces;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Web.Mvc;
using System.Web.Mvc;


namespace DynamicRouting.Kentico.MVC
{
    public class DynamicRouteTemplateController : PageTemplateController
    {
        private IDynamicRouteHelper mDynamicRouteHelper;

        public DynamicRouteTemplateController()
        {
            this.mDynamicRouteHelper = new BaseDynamicRouteHelper();
        }

        /// <summary>
        /// Gets the node based on the current request url and then renders the template result.
        /// </summary>
        public ActionResult Index(string TemplateControllerName = null, bool? IncludeDocumentInOutputCache = null)
        {
            if (!IncludeDocumentInOutputCache.HasValue)
            {
                IncludeDocumentInOutputCache = DynamicRouteInternalHelper.GetDefaultAddPageToCacheDependency();
            }
            ITreeNode FoundNode = mDynamicRouteHelper.GetPage(Columns: new string[] { "DocumentID" }, AddPageToCacheDependency: IncludeDocumentInOutputCache.Value);
            if (FoundNode != null)
            {
                HttpContext.Kentico().PageBuilder().Initialize(FoundNode.DocumentID);
                if (!string.IsNullOrWhiteSpace(TemplateControllerName))
                {
                    // Adjust the route data to point to the template's controller if it has one.
                    HttpContext.Request.RequestContext.RouteData.Values["Controller"] = TemplateControllerName;
                }
                return new TemplateResult(FoundNode.DocumentID);
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }
    }
}
