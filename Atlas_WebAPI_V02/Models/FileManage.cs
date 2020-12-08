using Atlas_WebAPI_V02.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Atlas_WebAPI_V02.Models
{
    
    public class FileManage
    {
        private readonly static int SaveFileTimeOUT = 19;  //默认文件保留19分钟
        private readonly static string ResultPath = AppDomain.CurrentDomain.BaseDirectory + "result";
        private readonly static string RecvPath = AppDomain.CurrentDomain.BaseDirectory + "recv";
        public static object Configuration { get; private set; }

        //删除过期文件
        public static void DeleteExpiredFiles()
        {
            DelectDir(RecvPath);
            DelectDir(ResultPath);
        }

        public static void DelectDir(string path)
        {
            if (!Directory.Exists(path))
                return;
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                foreach (FileSystemInfo item in fileinfo)
                {
                    if (DateTime.Compare(item.LastAccessTime.AddMinutes(SaveFileTimeOUT), DateTime.Now) > 0)
                    {
                        if (item is DirectoryInfo)            //判断是否文件夹
                        {
                            DirectoryInfo subdir = new DirectoryInfo(item.FullName);
                            subdir.Delete(true);          //删除子目录和文件
                        }
                        else
                        {
                            File.Delete(item.FullName);      //删除指定文件
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


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
            if (System.IO.Directory.Exists(RecvPath) == false)//如果不存在就创建file文件夹
            {
                System.IO.Directory.CreateDirectory(RecvPath);
            }
            return RecvPath;
        }

        /// <summary>
        /// 获取解析结果图片文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string GetResultFolderPath()
        {
            if (System.IO.Directory.Exists(ResultPath) == false)//如果不存在就创建file文件夹
            {
                System.IO.Directory.CreateDirectory(ResultPath);
            }
            return ResultPath;
        }
    }
}