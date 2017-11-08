using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Northwind.Essentials.Html
{
    public class HtmlWriter
    {
        //private string fileName;
        //private string htmlHeader;
        //private string htmlBody;

        public HtmlWriter(string name)
        {
          //  fileName = name;
        }

        public void Export(string path)
        {
            //System.IO.File.WriteAllText(path + "/" + fileName + ".html", "<html><header>" + htmlHeader + "</header><body>" + htmlBody + "</body></html>", System.Text.Encoding.Unicode);
        }

        public void AddObject(string content, CssStyle style = null)
        {
           // htmlBody += "<div>" + content + "</div>";
        }
        
    }
}