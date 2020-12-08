using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Atlas_WebAPI_V02.Log;
using Atlas_WebAPI_V02.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_WebAPI_V02.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        //声明一个日志常量
        private readonly ILoggerHelper _logger;
        //构造函数注入 service
        public ValuesController(ILoggerHelper loggerHelper)
        {
            _logger = loggerHelper;
        }

        // GET api/values
        [HttpPost]
        public ResultBody AnalysisResult()
        {
            List<object> list_points = new List<object>();

            ResultBody result = new ResultBody();//实例化响应对象
            Request_param request = new Request_param();//实例化请求参数对象
            Analysis_Image analysis_Image = new Analysis_Image();//实例化红外解析对象

            _logger.Error(typeof(ValuesController), "这是错误日志", new Exception("123"));
            _logger.Debug(typeof(ValuesController), "这个是bug日志");

            try
            {
                //接收请求参数，文件
                request.GetParam(HttpContext);

                //解析红外图片对象，生成结果集
                analysis_Image.OpenPic(request.img_file_path, request.GetPicInfoList());
            }
            catch (Exception ex)
            {
                result.Log(404, ex.Message.ToString());
                return result;
            }
            finally
            {
                //if (System.IO.File.Exists(request.img_file_path)) //删除接收临时文件
                //    System.IO.File.Delete(request.img_file_path);
            }

            try
            {
                //构建客户端要求的数据结构
                result.resultCollection = GetResultCollection(analysis_Image.Result_file_name, analysis_Image.Result_Rect_Info_lsit);//生成结果集
            }
            catch (Exception ex)
            {
                result.Log(404, ex.Message.ToString());
                return result;
            }
            finally
            {
                //if (System.IO.File.Exists(result_Info.result_path)) //删除结果临时文件
                //    System.IO.File.Delete(result_Info.result_path);
            }

            return result;
        }

        public ResultCollection GetResultCollection(string name, List<Result_pic_info> list)
        {
            Object[] result_obj = new object[list.Count];

            try
            {
                int n = 0;
                foreach (var item in list)
                {
                    List<object> list_points = new List<object>();
                    list_points.Add(new TargetPoint((int)data_type.max, "最高温", item.max, item.pt_max.X, item.pt_max.Y));
                    list_points.Add(new TargetPoint((int)data_type.min, "最低温", item.min, item.pt_min.X, item.pt_min.Y));
                    list_points.Add(new TargetPoint((int)data_type.avg, "平均温", item.avg, 0, 0));
                    result_obj[n++] = list_points;
                }

                return new ResultCollection(result_obj, name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string TranscodingFile(string filePath)
        {
            byte[] FileByte = System.IO.File.ReadAllBytes(filePath);
            string str_img_file = Convert.ToBase64String(FileByte);
            return str_img_file;
        }

        //// GET api/values
        [HttpGet]
        public Object GetResultFile(string filename)
        {
            if (filename == null)
                return "仅提供POST()方法和附带filename参数的Get()方法";

            IQueryCollection request = HttpContext.Request.Query;
            String value = request["filename"];

            if (value == null)
                return "请确认参数名完整正确！";

            string path = Path.Combine(FileManage.GetResultFolderPath(), filename);//文件结果路径;
            if (!System.IO.File.Exists(path))
            {
                return filename + ":文件不存在!";
            }
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            return fs;
        }

        //获取图片详细信息
        [HttpGet]
        public Object GetDetailInfo(string filename)
        {
            if (filename == null)
                return "请确认参数名完整正确！";
            string path = Path.Combine(FileManage.GetSaveFolderPath(), filename);//文件结果路径;
            
            if (!System.IO.File.Exists(path))
            {
                return path + ":文件不存在!";
            }
            return Analysis_Image.DetailInfoFromPic(path);
        }

        [HttpGet]
        //获取可见光文件
        public Object GetVisualFile(string filename)
        {
            if (filename == null)
                return "请确认参数名完整正确！";
            string path = Path.Combine(FileManage.GetSaveFolderPath(), filename);//文件结果路径;
            if (!System.IO.File.Exists(path))
            {
                return filename + ":文件不存在!";
            }
            return Analysis_Image.GetOnlyMode(path);
        }
    }
}
