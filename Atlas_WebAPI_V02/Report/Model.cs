using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas_WebAPI_V02.Report
{
    public class ReportModel
    {
        public string CableName { get; set; }//电缆名称
        public string Department { get; set; }//所属班组
        public string AreaName { get; set; }//所属地区
        public string VoltageClasses { get; set; }//电压等级(kV)
        public string Site { get; set; }//检测地点
        public string InspectionDate { get; set; }//检测日期
        public string Inspector { get; set; }//检测人员
        public string DeviceName { get; set; }//检测设备
        public string AirTemperature { get; set; }//气温
        public string Humidity { get; set; }//湿度 
        public string DeviceCameraID { get; set; }//设备镜头 
        public string MeasuringRang { get; set; }//测量范围


        public string TopMaxTemperature1 { get; set; }//上部高温高温点
        public string BottomMaxTemp1 { get; set; }//下部高温高温点
        public string ReferenceTemperatu1 { get; set; }//参考高温点

        public string TopMaxTemperature2 { get; set; }
        public string BottomMaxTemp2 { get; set; }
        public string ReferenceTemperatu2 { get; set; }

        public string TopMaxTemperature3 { get; set; }
        public string BottomMaxTemp3 { get; set; }
        public string ReferenceTemperatu3 { get; set; }
    }
}
