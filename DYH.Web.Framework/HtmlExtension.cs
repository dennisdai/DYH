using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DYH.Models;

namespace DYH.Web.Framework
{
    public static class HtmlExtension
    {
        public static MvcHtmlString Pager(this HtmlHelper html, PageEntry model, string url)
        {
            var sbHtml = new StringBuilder();
            if (model.PageCount <= 1) return MvcHtmlString.Create(sbHtml.ToString());

            var containerId = string.Empty;
            if (!string.IsNullOrEmpty(model.ContainerId))
            {
                containerId = String.Format("id=\"{0}\"", model.ContainerId);
            }

            sbHtml.AppendFormat("<div class=\"pagination\" {0}>", containerId);
            sbHtml.Append("<ul>");
            if (model.Current == 1)
            {
                sbHtml.Append("<li class=\"disabled\"><span><i class=\"icon-angle-double-left\"></i></span></li>");
            }
            else
            {
                sbHtml.AppendFormat("<li><a href=\"{0}\" title=\"First\"><i class=\"icon-angle-double-left\"></i></a></li>", (url + model.First));
            }

            if (model.Prev <= 0)
            {
                sbHtml.Append("<li class=\"disabled\"><span><i class=\"icon-angle-left\"></i></span></li>");
            }
            else
            {
                sbHtml.AppendFormat("<li> <a href=\"{0}\" title=\"Previous\"><i class=\"icon-angle-left\"></i></a></li>", (url + model.Prev));
            }
            var showButtons = model.ShowButtons;

            var begin = showButtons / 2;

            var start = model.Current - begin;
            if (start > model.PageCount - showButtons)
            {
                start = model.PageCount - showButtons;
            }
            if (start <= 0)
            {
                start = 1;
            }

            var end = start + showButtons;
            if (end > model.PageCount)
            {
                end = model.PageCount;
            }

            for (var i = start; i <= end; i++)
            {
                if (i == model.Current)
                {
                    sbHtml.AppendFormat("<li class=\"active\"><span>{0}</span></li>", i);
                }
                else
                {
                    sbHtml.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", (url + i), i);
                }
            }

            if (model.Next > model.Last)
            {
                sbHtml.Append("<li class=\"disabled\"><span><i class=\"icon-angle-right\"></i></span></li>");
            }
            else
            {
                sbHtml.AppendFormat("<li><a href=\"{0}\" title=\"Next\"><i class=\"icon-angle-right\"></i></a></li>", (url + model.Next));
            }
            if (model.PageCount  == model.Current)
            {
                sbHtml.Append("<li class=\"disabled\"><span><i class=\"icon-angle-double-right\"></i></span></li>");
            }
            else
            {
                sbHtml.AppendFormat("<li><a href=\"{0}\" title=\"Last\"><i class=\"icon-angle-double-right\"></i></a></li>", (url + model.Last));
            }

            sbHtml.Append("</ul>");
            sbHtml.Append("</div>");

            return MvcHtmlString.Create(sbHtml.ToString());
        }
    }
}
