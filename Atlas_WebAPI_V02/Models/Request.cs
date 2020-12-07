using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
//using System.Web;
using Microsoft.AspNetCore.Http;


namespace Atlas_WebAPI_V02.Models
{
    public struct Target_info  //目标区域信息
    {
        public int start_X;
        public int start_Y;
        public int Width;
        public int Heigth;
        public Target_info(int x,int y,int w,int h)
        {
            start_X = x;
            start_Y = y;
            Width = w;
            Heigth = h;
         }
    }
    public class Request_param
    {
        public int tartRectCount { get; set; }
        public string img_file_path { get; set; }
        private List<Rectangle> targ_Rect_list { get; set; }

        public Request_param()
        {
            targ_Rect_list = new List<Rectangle>();
        }
         
        public void GetParam(HttpContext context)
        {
            tartRectCount = ToInt(context.Request.Form["count"]); //
            if (tartRectCount == 0 )
                throw new MyException("缺少目标区域数量参数！");

            for (int i = 0; i < tartRectCount; i++)
            {
                targ_Rect_list.Add(new Rectangle(
                   ToInt(context.Request.Form["startx_" + i]),
                   ToInt(context.Request.Form["starty_" + i]),
                   ToInt(context.Request.Form["width_" + i]),
                   ToInt(context.Request.Form["height_" + i])));
            }

            IFormFileCollection filelist = context.Request.Form.Files; //上传文件
            try
            {
                string error_param = "";
                for (int i = 0; i < targ_Rect_list.Count; i++)
                {
                    Rectangle rect = targ_Rect_list[i];
                    if (rect.X == 0 || rect.Y == 0 || rect.Width == 0 || rect.Height == 0)
                    {
                        error_param += rect.X == 0 ? "startx_" + i.ToString() : "_" + ",";
                        error_param += rect.Y == 0 ? "starty_" + i.ToString() : "_" + ",";
                        error_param += rect.Width == 0 ? "width_"  + i.ToString() : "_" + ",";
                        error_param += rect.Height == 0 ? "height_" + i.ToString() : "_";
                    }
                }

                if (!string.IsNullOrEmpty(error_param))
                    throw new MyException("必要参数:[" + error_param + "] 为空值！ ");
                if (filelist == null || filelist.Count <= 0)
                    throw new MyException("缺少上传文件！");

                this.img_file_path = SaveImageFile(filelist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Rectangle> GetPicInfoList()
        {
            return this.targ_Rect_list;
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

    }
}