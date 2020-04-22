#pragma checksum "C:\Users\Will\Desktop\CS\5530-Database_Systems\LMS_handout\LMS\Views\Student\Catalog.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "883752746a6953959a18ef26babe84b1c7c19bc0"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Student_Catalog), @"mvc.1.0.view", @"/Views/Student/Catalog.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Student/Catalog.cshtml", typeof(AspNetCore.Views_Student_Catalog))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\Will\Desktop\CS\5530-Database_Systems\LMS_handout\LMS\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#line 2 "C:\Users\Will\Desktop\CS\5530-Database_Systems\LMS_handout\LMS\Views\_ViewImports.cshtml"
using LMS;

#line default
#line hidden
#line 3 "C:\Users\Will\Desktop\CS\5530-Database_Systems\LMS_handout\LMS\Views\_ViewImports.cshtml"
using LMS.Models;

#line default
#line hidden
#line 4 "C:\Users\Will\Desktop\CS\5530-Database_Systems\LMS_handout\LMS\Views\_ViewImports.cshtml"
using LMS.Models.AccountViewModels;

#line default
#line hidden
#line 5 "C:\Users\Will\Desktop\CS\5530-Database_Systems\LMS_handout\LMS\Views\_ViewImports.cshtml"
using LMS.Models.ManageViewModels;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"883752746a6953959a18ef26babe84b1c7c19bc0", @"/Views/Student/Catalog.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"363c4fd446cecdc21217d95f921ea2b5901a3ca3", @"/Views/_ViewImports.cshtml")]
    public class Views_Student_Catalog : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "C:\Users\Will\Desktop\CS\5530-Database_Systems\LMS_handout\LMS\Views\Student\Catalog.cshtml"
  
  ViewData["Title"] = "Catalog";
  Layout = "~/Views/Shared/StudentLayout.cshtml";

#line default
#line hidden
            BeginContext(94, 47, true);
            WriteLiteral("\r\n<h4 id=\"topElement\">Course Catalog</h4>\r\n\r\n\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(162, 2227, true);
                WriteLiteral(@"
  <script type=""text/javascript"">

    LoadData();

    //$(function () {
    //  LoadData();
    //});

    function MakeDepartmentTable() {

      var topDiv = document.createElement(""div"");
      topDiv.setAttribute(""class"", ""col-md-12"");

      var panelDiv = document.createElement(""div"");
      panelDiv.setAttribute(""class"", ""panel panel-primary"");

      var headingDiv = document.createElement(""div"");
      headingDiv.setAttribute(""class"", ""panel-heading"");

      var bodyPanel = document.createElement(""div"");
      bodyPanel.setAttribute(""class"", ""panel-body"");

      var tbl = document.createElement(""table"");
      tbl.setAttribute(""class"", ""table table-bordered table-striped table-responsive table-hover"");

      bodyPanel.appendChild(tbl);

      panelDiv.appendChild(headingDiv);
      panelDiv.appendChild(bodyPanel);

      topDiv.appendChild(panelDiv);

      return topDiv;
    }

    function SetDepartmentName(depDiv, name) {

      var heading = depDiv.getE");
                WriteLiteral(@"lementsByClassName(""panel-heading"")[0];
      var title = document.createElement(""h4"");
      title.appendChild(document.createTextNode(name));
      heading.appendChild(title);

    }

    function PopulateTable(tbl, courses, subject) {
      var newBody = document.createElement(""tbody"");

      courses.sort(function (a, b) {
        return parseInt(a.number) - parseInt(b.number);

      });

      $.each(courses, function (i, item) {
        var tr = document.createElement(""tr"");

        var td = document.createElement(""td"");
        var a = document.createElement(""a"");
        a.setAttribute(""href"", ""/Student/ClassListings/?subject="" + subject + ""&num="" + item.number);
        a.appendChild(document.createTextNode(item.number));
        td.appendChild(a);
        tr.appendChild(td);

        var td = document.createElement(""td"");
        td.appendChild(document.createTextNode(item.cname));
        tr.appendChild(td);

        newBody.appendChild(tr);
      });

      tbl.ap");
                WriteLiteral("pendChild(newBody);\r\n\r\n    }\r\n\r\n    function LoadData() {\r\n\r\n\r\n      var prevNode = document.getElementById(\"topElement\");\r\n\r\n      $.ajax({\r\n        type: \'POST\',\r\n        url: \'");
                EndContext();
                BeginContext(2390, 35, false);
#line 92 "C:\Users\Will\Desktop\CS\5530-Database_Systems\LMS_handout\LMS\Views\Student\Catalog.cshtml"
         Write(Url.Action("GetCatalog", "Student"));

#line default
#line hidden
                EndContext();
                BeginContext(2425, 865, true);
                WriteLiteral(@"',
        dataType: 'json',
        success: function (data, status) {
          //alert(JSON.stringify(data));
          $.each(data, function (i, item) {
            var newDiv = MakeDepartmentTable();
            SetDepartmentName(newDiv, item.subject + "": "" + item.dname);
            var newTable = newDiv.getElementsByClassName(""table"")[0];
            PopulateTable(newTable, item.courses, item.subject);

            prevNode.parentNode.insertBefore(newDiv, prevNode.nextSibling);
            prevNode = newDiv;
          });
          
        },
        error: function (ex) {
          var r = jQuery.parseJSON(response.responseText);
          alert(""Message: "" + r.Message);
          alert(""StackTrace: "" + r.StackTrace);
          alert(""ExceptionType: "" + r.ExceptionType);
        }
        });

    }



  </script>

");
                EndContext();
            }
            );
            BeginContext(3293, 4, true);
            WriteLiteral("\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
