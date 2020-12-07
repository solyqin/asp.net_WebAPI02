using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas_WebAPI_V02.Models
{
    enum data_type
    {
        max = 1001,
        min ,
        avg ,
    }
  
    struct TargetPoint
    {
        public int type;
        public string name;
        public double value;
        public int point_x;
        public int point_y;

        public TargetPoint(int v1, string v2, double v3, int v4, int v5) : this()
        {
            this.type = v1;
            this.name = v2;
            this.value = v3;
            this.point_x = v4;
            this.point_y = v5;
        }
    }
    public class ResultCollection
    {
        public Object[] Opints_result { get; set; }
        public string FileName { get; set; }
        public ResultCollection(Object[] obj, string name)
        {
            this.Opints_result = obj;
            this.FileName = name;
        }
    }

    public class ResultBody
    {
        public int code { get; set; }
        public string message { get; set; }
        //public ResultCollection resultCollection { get; set; }
        public Object resultCollection { get; set; }

        public  ResultBody(int def_code = 200,string def_message = "已接收请求并处理图片！")
        {
            this.code = def_code;
            this.message = def_message;
        }

        public void Log(int code, string msg)
        {
            this.code = code;
            this.message = msg;
        }

        public void NotFound()
        {
            this.code = 404; 
            this.message = "访问资源不存在！";
            this.resultCollection = null;
        }
    }

    class DetailInfo
    {
        public string Title { set; get; }
        public string Width { set; get; }
        public string Height { set; get; }
        public string DateTaken { set; get; }
        public string AtmosphericTemperature { set; get; }
        public string Lens { set; get; }
        public string Model { set; get; }
        public string Range_max { set; get; }
        public string Range_min { set; get; }
        public string SerialNumber { set; get; }
      
    }
}