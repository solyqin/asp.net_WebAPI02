using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Atlas_WebAPI_V02.Models
{
    public class Function
    {
      
        public void StringtoImage(string Base64buffer, string savePath)
        {
            try
            {
                byte[] buf = Convert.FromBase64String(Base64buffer);
                MemoryStream stream = new MemoryStream(buf);
                stream.Position = 0;
                Image image = Image.FromStream(stream);
                image.Save(savePath);
                stream.Close();
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        public string ImagetoString(string imagePath)
        {
            try
            {
                FileStream fileStream = new FileStream(imagePath, FileMode.Open);
                byte[] vs = new byte[fileStream.Length];
                fileStream.Read(vs, 0, vs.Length);
                fileStream.Close();
                string imageString = Convert.ToBase64String(vs);
                return imageString;
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }
    }
}