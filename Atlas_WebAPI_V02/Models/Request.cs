using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
//using System.Web;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Atlas_WebAPI_V02.Models
{
    public struct Rect_param
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public int id;
        public IEnumerator<double> GetEnumerator()
        {
            return MyIE();
        }

        public IEnumerator<double> MyIE()
        {
            yield return x;
            yield return y;
            yield return width;
            yield return height;
            yield return id;
        }
    }
    
    public class Request_param
    {
        public string img_file_path { get; set; }
        private List<Rect_param> rect_Params { get; set; } 

        public void GetParam(HttpContext context)
        {
            string error_param = "";
            IFormFileCollection filelist = context.Request.Form.Files; //上传文件
            string param = context.Request.Form["rect_param"];
            int count = ToInt(context.Request.Form["count"]); //

            List<Rect_param> list_rect = JsonConvert.DeserializeObject<List<Rect_param>>(param);

            //参数校验
            if (count == 0 )
                throw new MyException("缺少目标区域数量参数！");
           
            foreach (Rect_param item in list_rect)
            {
                int index = list_rect.IndexOf(item); //当前循环的索引
                
                error_param += item.id == 0 ? $"[{index}]id" :"";   //默认目标框的ID 不能为0，避免验证冲突
                error_param += item.x == 0 ? $"[{index}]x": "";
                error_param += item.y == 0 ? $"[{index}]y" : "";
                error_param += item.width == 0 ? $"[{index}]width" : "";
                error_param += item.height == 0 ? $"[{index}]height" : "";
            }

            try
            {
                if (!string.IsNullOrEmpty(error_param))
                    throw new MyException("必要参数:[" + error_param + "] 为空值！ ");
                if (filelist == null || filelist.Count <= 0)
                    throw new MyException("缺少上传文件！");

                this.rect_Params = list_rect;
                this.img_file_path = SaveImageFile(filelist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Rect_param> GetPicInfoList()
        {
            return this.rect_Params;
        }

        public string SaveImageFile(IFormFileCollection filelist)
        {
            try
            {
                IFormFile file = filelist[0];//默认值上传一张图片；获取第一个文件
                string filename = FileManage.GenerateFileName(file.FileName); //上传文件名(唯一)
                string recv_Path = Path.Combine(FileManage.GetSaveFolderPath(), filename); //保存路径+文件名
                
                try
                {
                    using (FileStream fs = new FileStream(recv_Path,FileMode.Create,FileAccess.Write))
                    {
                        file.CopyTo(fs);
                    }
                 
                    return recv_Path;
                }
                catch (Exception )
                {
                    throw new MyException("上传文件写入异常! 写入路径:"+ recv_Path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ToInt(object obj)
        {
            int result = default(int); //默认int 为 0
            if (obj != null && obj != DBNull.Value)
            {
                int.TryParse(obj.ToString(), out result);
            }
            return result;
        }

        public static string[] SaveFile(IFormFileCollection filelist)
        {
            string[] filenames = new string[filelist.Count];
            for (int i = 0; i < filelist.Count; i++)
            {
                try
                {
                    IFormFile file = filelist[i];//默认值上传一张图片；获取第一个文件
                    string filename = FileManage.GenerateFileName(file.FileName); //上传文件名(唯一)
                    string recv_Path = Path.Combine(FileManage.GetSaveFolderPath(), filename); //保存路径+文件名

                    using (FileStream fs = new FileStream(recv_Path, FileMode.Create, FileAccess.Write))
                    {
                        file.CopyTo(fs);
                    }
                    filenames[i] = recv_Path;
                }
                catch (Exception)
                {
                    filenames[i] = string.Empty;
                    throw new MyException("上传文件写入异常! ");
                }
            }
            return filenames;
        }
    }
}