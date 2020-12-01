using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas_WebAPI_V02.Models
{
    
    public class FileManage
    {
        /// <summary>
        /// 生成唯一文件名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GenerateFileName(string origin_name)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(origin_name);    //返回不带扩展名的文件名 
            string Extension = System.IO.Path.GetExtension(origin_name);    //返回扩展名
            var part1 = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var part2 = Guid.NewGuid().ToString("N");
            return name + "_" + part1 + "_" + part2 + Extension;
        }

        /// <summary>
        /// 获取接收文件的文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string GetSaveFolderPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "recv";
            if (System.IO.Directory.Exists(path) == false)//如果不存在就创建file文件夹
            {
                System.IO.Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// 获取解析结果图片文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string GetResultFolderPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "result";
            if (System.IO.Directory.Exists(path) == false)//如果不存在就创建file文件夹
            {
                System.IO.Directory.CreateDirectory(path);
            }
            return path;
        }

    }
}