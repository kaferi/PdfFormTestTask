﻿using Aspose.Pdf;
using Aspose.Pdf.Forms;
using PdfFormTestTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PdfFormTestTask.Service.Controllers.Api
{
    public class PdfController : ApiController
    {
        public List<PfsFormField> Get(string username, string password, string id)
        {
            PfsPdfFile pdfFile = PfsRepository.Current.GetUser(username, password).GetPdfFileByLocalName(id);
            List<PfsFormField> ret = new List<PfsFormField>();

            if (null != pdfFile)
            {
                using (Document document = new Document(HttpContext.Current.Server.MapPath("~/App_Data") + "/" + pdfFile.LocalName))
                {
                    for (int i = 1; i <= document.Form.Count; i++)
                    {
                        try
                        {
                            ret.Add(new PfsFormField(document.Form[i] as Field));
                        }
                        catch { }
                    }
                }
            }
            return ret;
        }


        public List<PfsFormField> Post(string username, string password, string id, [FromBody] List<PfsFormField> values)
        {
            PfsPdfFile pdfFile = PfsRepository.Current.GetUser(username, password).GetPdfFileByLocalName(id);
            List<PfsFormField> ret = new List<PfsFormField>();

            string documentPath = HttpContext.Current.Server.MapPath("~/App_Data") + "/" + pdfFile.LocalName;

            using (Document document = new Document(documentPath))
            {

                //foreach (Field field in document.Form.Fields)
                for (int i = 1; i <= document.Form.Count; i++)
                {
                    Field field = null;
                    try
                    {
                        field = document.Form[i] as Field;
                    }
                    catch { }

                    if (null == field) continue;

                    foreach (PfsFormField pfsField in values)
                    {
                        if (field.FullName == pfsField.Name)
                        {

                            field.Value = pfsField.Value;
                            if (field is CheckboxField)
                            {
                                (field as CheckboxField).Checked = pfsField.Checked;
                            }
                            break;
                        }
                    }
                }

                document.Save(documentPath);

                ret = document.Form.Fields
                    .Select(f => new PfsFormField(f))
                    .ToList();

            }

            return ret;
        }
    }
}