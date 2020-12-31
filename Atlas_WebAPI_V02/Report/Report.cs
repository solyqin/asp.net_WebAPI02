using Atlas_WebAPI_V02.Models;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Atlas_WebAPI_V02.Report
{
    public class GenerateReport
    {
        private static readonly string ReportFormat = ".doc";
        private static BindingFlags s_flag = BindingFlags.Instance | BindingFlags.Public;
        //private static Document doc = new Document(FileManage.GetReportModelFolderPath());
        public static string SaveReport(ReportModel report, string[] img_path)
        {
           
            using (Document doc = new Document(FileManage.GetReportModelFolderPath()))//使用报告模板
            {
                doc.Properties.FormFieldShading = true; //清除表单域阴影

                try
                {
                    Type type = typeof(ReportModel);
                    PropertyInfo[] properties = type.GetProperties(s_flag);
                    int length = properties.Length;
                    Dictionary<string, PropertyInfo> dict = new Dictionary<string, PropertyInfo>(length, StringComparer.OrdinalIgnoreCase);
                    foreach (PropertyInfo prop in properties)
                    {
                        dict[prop.Name] = prop;
                    }

                    //填入表格信息
                    object value = null;
                    foreach (FormField field in doc.Sections[0].Body.FormFields)
                    {
                        Console.WriteLine("field.name: " + field.Name);
                        //field.name对应设置模版文本域名称
                        PropertyInfo prop = dict[field.Name];
                        //Image1
                        if (prop != null)
                        {
                            value = prop.GetValue(report, null);
                            if (value != null && value != DBNull.Value)
                            {
                                switch (field.Type)
                                {
                                    case FieldType.FieldFormTextInput:
                                        field.Text = value.ToString();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    //插入图片
                    for (int i = 0; i < img_path.Length; i++)
                    {
                        DocPicture pic = new DocPicture(doc);
                        TextSelection selection = doc.FindString("Illustration"+i, true, true);
                        if (selection.Count < 1)
                            continue;
                        pic.LoadImage(Image.FromFile(img_path[i]));
                        pic.Width = 250;
                        pic.Height = 240;
                        TextRange range = selection.GetAsOneRange();
                        int index = range.OwnerParagraph.ChildObjects.IndexOf(range);
                        range.OwnerParagraph.ChildObjects.Insert(index, pic);
                        range.OwnerParagraph.ChildObjects.Remove(range);
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
                string reprotname = FileManage.GenerateFileName(null) + ReportFormat;
                string save_path = Path.Combine(FileManage.GetReportSaveFolderPath(), reprotname);
                doc.SaveToFile(save_path, FileFormat.Doc);
                return reprotname;
            }
           
        }
    }
}
