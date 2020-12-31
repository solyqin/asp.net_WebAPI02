using Atlas_WebAPI_V02.Report;
using Flir.Atlas.Image;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;


namespace Atlas_WebAPI_V02.Models
{
    public struct Result_pic_info  //区域解析结果
    {
        public int rect_id;
        public double max;
        public double min;
        public double avg;

        public Point pt_max;
        public Point pt_min;

        public Rectangle target_rect;

        public Result_pic_info(double max,double min,double avg,Point pt_max,Point pt_min,int id, Rectangle rect)
        {
            this.max = max;
            this.min = min;
            this.avg = avg;
            this.pt_max = pt_max;
            this.pt_min = pt_min;
            this.rect_id = id;
            this.target_rect = rect;
            
        }
    }

    public struct TargetRectMaxMinValue
    {
        public double rect_max;//目标区域最高温
        public double rect_min;//目标区域最低温
        public double rect_avg;//目标区域平均温
    }

    public class Analysis_Image
    {
        private List<Result_pic_info> result_Rect_Info_lsit { set; get; }
        public List<Result_pic_info> Result_Rect_Info_lsit { get { return this.result_Rect_Info_lsit; } }
        private string result_file_name { set; get; }
        public string Result_file_name {  get { return this.result_file_name; } }
        private string picBaseInfo { set; get; }
        public string PicBaseInfo { get { return this.picBaseInfo; } }

        /// <summary>
        /// 解析路径下的红外图片，获取相关信息
        /// </summary>
        /// <param name="file_path">文件路径</param>
        /// <param name="pic_info">识别区域信息</param>
        /// <returns></returns>
        public  void OpenPicAsync(string file_path, List<Rect_param> pic_param_list)
        {
            try
            {
                using (ThermalImageFile th = new ThermalImageFile(file_path))   //打开热成像图片文件  
                {
                    //拼接结果文件路径
                    string savepath = System.IO.Path.Combine(FileManage.GetResultFolderPath() , th.Title);
                   
                    //处理图片，获取结果信息
                    using (Bitmap saveimage = DrawRectangleInPicture(th, pic_param_list, Color.Red, 2, DashStyle.Solid))
                    {
                        saveimage.Save(savepath); //保存结果图片
                        this.result_file_name = System.IO.Path.GetFileName(savepath); // 保存结果文件路径
                    }
                    this.picBaseInfo = JsonConvert.SerializeObject(DetailInfo(th)); 
                }
            }
            catch(Exception ex)
            {
                if (System.IO.File.Exists(file_path)) //删除非红外图片
                    System.IO.File.Delete(file_path);
                throw new MyException("解析对象不是红外图片！"+ex.Message.ToString());
            }
        }

        public bool IsThermalImage(string filename)
        {
            if (ThermalImageFile.IsThermalImage(filename))
                return true;

            Console.WriteLine("图片不是热成像图片文件，无法打开");
            return false;
        }

        /// <summary>
        /// 在图片上画框
        /// </summary>
        /// <param name="thermal">红外图对象</param>
        /// <param name="pic_info">起始点</param>
        /// <param name="p1">终止点</param>
        /// <param name="RectColor">矩形框颜色</param>
        /// <param name="LineWidth">矩形框边界</param>
        /// <param name="ds">边界样式</param>
        // /// <param name="HotSpot">最高温度点</param>
        // /// <param name="ColdSpot">最低温度点</param>
        /// <returns>Bitmap 画上矩形和标记出最高、低温度点的图</returns>
        public Bitmap DrawRectangleInPicture(ThermalImageFile thermal, List<Rect_param> param_list, Color RectColor, int LineWidth, DashStyle ds)
        {
            if (thermal == null) return null;

            Graphics g = Graphics.FromImage(thermal.Image);
            g.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            Brush brush = new SolidBrush(RectColor);
            Pen pen = new Pen(brush, LineWidth);
            pen.DashStyle = ds;  //边界样式

            //获取目标区域信息 并 标记高低温点 (画小三角)
            GetTargetResult(thermal, param_list, g);
            
            foreach (var item in this.result_Rect_Info_lsit)
            {
                g.DrawRectangle(pen, item.target_rect); //画框

                DrawStringMsg(g,item);  //画框序号 和 图片左上角信息
            }

            brush.Dispose();
            pen.Dispose();
            g.Dispose();
           
            return thermal.Image;
        }
        public enum Dire
        {
            UP,
            DOWN
        }

        /// <summary>
        /// 填充三角形,用三角形标记出点的位置
        /// </summary>
        /// <param name="g">Graphics类</param>
        /// <param name="point">三角尖尖</param>
        /// <param name="dire">三角形的指向</param>
        /// <returns></returns>
        private void FillTriangle_1(Graphics g, Point point, Dire dire)
        {
            int dev = 8;  //三角形的高
            Point cornerleft;
            Point cornerright;

            switch (dire)
            {
                case Dire.UP:
                    cornerleft = new Point(point.X - dev / 2, point.Y - dev);
                    cornerright = new Point(point.X + dev / 2, point.Y - dev);
                    Point[] pntArr = { point, cornerleft, cornerright };
                    g.FillPolygon(Brushes.Red, pntArr);
                    break;
                case Dire.DOWN:
                    cornerleft = new Point(point.X - dev / 2, point.Y + dev);
                    cornerright = new Point(point.X + dev / 2, point.Y + dev);
                    Point[] pntArr1 = { point, cornerleft, cornerright };
                    g.FillPolygon(Brushes.Blue, pntArr1);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 标记指定矩形区域内的最高和最低温度点
        /// </summary>
        /// <param name="th">红外图片对象</param>
        /// <param name="rect">指定矩形</param>
        public void GetTargetResult(ThermalImageFile th, List<Rect_param> param_list, Graphics g)
        {
            List<Result_pic_info> templist = new List<Result_pic_info>();
            foreach (var rect in param_list)
            {
                Rectangle target_rect = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                double[] tempertureRect = th.GetValues(target_rect); //Math.Round(d, 2).ToString()
                Point pt_max = GetMaxPointInRectangle(tempertureRect, target_rect);//获取高温点坐标
                Point pt_min = GetMinPointInRectangle(tempertureRect, target_rect);//获取高温点坐标
                var value = GetMaxMinInRectangle(tempertureRect);

                //标记点
                FillTriangle_1(g, pt_max, Dire.UP);
                FillTriangle_1(g, pt_min, Dire.DOWN);

                //结果数值列表
                templist.Add(new Result_pic_info(value.rect_max, value.rect_min, value.rect_avg, pt_max, pt_min, rect.id,target_rect));
            }
            this.result_Rect_Info_lsit = templist;
        }

        /// <summary>
        /// 画矩形框序号
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rectangle"></param>
        /// <param name="digit"></param>
        public void DrawStringMsg(Graphics g, Result_pic_info info)
        {
            Color font_color = Color.White;//字体颜色
            StringFormat stringFormat = new StringFormat();//文字格式
            stringFormat.Alignment = StringAlignment.Near;//居中对齐

            //框左边的序号标识
            int width = 30;
            int height = 15;
            Rectangle rect = new Rectangle(info.target_rect.Location.X - width, info.target_rect.Location.Y , width, height);
            g.DrawString("Bx"+ info.rect_id.ToString(), new Font("微软雅黑", 9), new SolidBrush(font_color), rect, stringFormat);//框序号

            //图片左上角信息
            int info_width = 135;
            int info_height = 50;
            string line0 = "max  " + info.max.ToString() + " ℃\r\n";
            string line1 = "         min   " + info.min.ToString() + " ℃\r\n";
            string line2 = "         avg   " + info.avg.ToString() + " ℃\r\n";
            string msg = String.Format("Bx{0}:  {1}{2}{3}", info.rect_id, line0, line1, line2);

            Rectangle info_rect = new Rectangle(5, 5 + (info_height+5) * info.rect_id - 1, info_width, info_height); //设计框ID从1开始
            g.DrawString(msg, new Font("微软雅黑", 9), new SolidBrush(font_color), info_rect, stringFormat);//目标矩形内的温度信息
          
            //g.DrawRectangle(new Pen(Color.White), info_rect);//矩形
            // Color circle_color = Color.FromArgb(192, 192, 192);//圈 颜色
            // g.DrawEllipse(new Pen(circle_color), rect); //空心圈
        }

        public void DrawStringMsg(Graphics g, Rectangle rectangle, string msg)
        {
            int size = 13;
            Color font_color = Color.White;//字体颜色

            Rectangle msg_rect = new Rectangle(rectangle.Location.X - size, rectangle.Location.Y, size, size);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;//居中显示

            g.DrawString(msg, new Font("微软雅黑", 7), new SolidBrush(font_color), msg_rect, stringFormat);  //message
          
        }

        /// <summary>
        /// 获取框选区域温度最高点的坐标
        /// </summary>
        /// <param name="arr">目标矩形区域内所有点的温度集合</param>
        /// <param name="rect">目标矩形</param>
        /// <returns>Point 最高温度点的坐标</returns>
        public Point GetMaxPointInRectangle(double[] arr, Rectangle rect)
        {
            int index = Array.IndexOf(arr, arr.Max());
            int x = rect.Location.X + index % rect.Width;
            int y = rect.Location.Y + index / rect.Width;
            //Console.WriteLine(arr.Max().ToString()+" MaxPoint:({0},{1}),  index:{2}, location:({3},{4})", x, y, index, rect.Location.X, rect.Location.Y);
            return new Point(x, y);
        }

        /// <summary>
        /// 获取框选区域温度最低点的坐标
        /// </summary>
        /// <param name="arr">目标矩形区域内所有点的温度集合</param>
        /// <param name="rect">目标矩形</param>
        /// <returns>Point 最低温度点的坐标</returns>
        public Point GetMinPointInRectangle(double[] arr, Rectangle rect)
        {
            int index = Array.IndexOf(arr, arr.Min());
            int x = rect.Location.X + index % rect.Width;
            int y = rect.Location.Y + index / rect.Width;
            //Console.WriteLine(arr.Min().ToString() + "  MaxPoint:({0},{1}),  index:{2}, location:({3},{4})", x, y, index, rect.Location.X, rect.Location.Y);
            return new Point(x, y);
        }
        /// <summary>
        /// 获取区域温度合集中的max,min,avg
        /// </summary>
        /// <param name="arr">温度合集</param>
        /// <returns></returns>
        public TargetRectMaxMinValue GetMaxMinInRectangle(double[] arr)
        {
            double tempertureRectMax = Math.Round((arr.Max()), 2);
            double tempertureRectMin = Math.Round((arr.Min()), 2);
            double tempertureRectAvg = Math.Round((arr.Sum() / arr.Length), 2);

            return new TargetRectMaxMinValue()
            {
                rect_max = tempertureRectMax,
                rect_min = tempertureRectMin,
                rect_avg = tempertureRectAvg,
            };
        }

        public static System.IO.Stream GetOnlyMode(string path)
        {
            using (ThermalImageFile th = new ThermalImageFile(path))
            {
                th.Fusion.Mode = th.Fusion.VisualOnly; //可见光模式
                ImageConverter imgconv = new ImageConverter();
                byte[] bytes = (byte[])imgconv.ConvertTo(th.Image, typeof(byte[]));
                System.IO.Stream stream = new System.IO.MemoryStream(bytes);
                return stream;
            }
        }
      
        public static string DetailInfoFromPic(string filepath)
        {
            using (ThermalImageFile th = new ThermalImageFile(filepath))
            {
                string json = JsonConvert.SerializeObject(DetailInfo(th));
                return json;
            }
        }

        public static DetailInfo DetailInfo(ThermalImageFile th)
        {
            ImageParameters imageParameters = th.ThermalParameters;
            CameraInformation camera = th.CameraInformation;

            DetailInfo detailInfo = new DetailInfo();
            //文件信息
            detailInfo.Title = th.Title; //图片名
            detailInfo.Width = th.Size.Width;
            detailInfo.Height = th.Size.Height;
            detailInfo.Max = th.Statistics.Max.Value; //全图最高温
            detailInfo.Min = th.Statistics.Min.Value; //全图最低温
            detailInfo.DateTaken = th.DateTaken.ToString();//拍摄日期
            //图像目标参数
            detailInfo.AtmosphericTemperature = Math.Round(imageParameters.AtmosphericTemperature, 2).ToString();//大气温度
            //相机信息                                                                                                     
            detailInfo.Lens = camera.Lens; //相机镜头信息
            detailInfo.Model = camera.Model; //相机模型信息
            detailInfo.Range_max = Math.Round(camera.Range.Maximum, 2).ToString();  //测量范围
            detailInfo.Range_min = Math.Round(camera.Range.Minimum, 2).ToString();   //测量范围
            detailInfo.SerialNumber = camera.SerialNumber; //相机的序列号
            //图像目标参数
            detailInfo.DistanceUnit = th.DistanceUnit.ToString(); //距离单位
            detailInfo.Distance = imageParameters.Distance; //到被聚焦对象的距离
            detailInfo.Emissivity = imageParameters.Emissivity; //红外图像的默认发射率
            detailInfo.RelativeHumidity = imageParameters.RelativeHumidity; //相对湿度(0.0 - 1.0)
            detailInfo.ReflectedTemperature = imageParameters.ReflectedTemperature; //反射温度

            return detailInfo;
        }
    }
}