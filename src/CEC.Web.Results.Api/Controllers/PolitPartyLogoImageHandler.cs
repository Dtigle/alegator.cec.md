using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CEC.Web.Results.Api.Infrastructure;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace CEC.Web.Results.Api.Controllers
{

    public class PolitPartyLogoImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/jpeg";

            string logoId = context.Request.QueryString["logoId"];

            LiveResultsLogic lrl = new LiveResultsLogic{ ElectionId = StatsTimer.ElectionsId };
            byte[] logo = lrl.LogoByCandidateId(int.Parse(logoId));

            if (logo == null || logo.Length == 0)
                logo = LoadResource(Assembly.GetExecutingAssembly(), "nologo.png");

            Image img = ByteArrayToImage(logo);
            
            using (var bitmap = new Bitmap(64, 64, PixelFormat.Format24bppRgb))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                g.DrawImage(img, 0, 0, 64, 64);
                //    context.Response.ContentType = "image/png";
                //    bitmap.Save(context.Response.OutputStream, ImageFormat.Png);
                logo = ImageToByteArray(bitmap);
            }

            context.Response.Write(LiveResultsLogic.HexToWebImage(logo));
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public static Image ByteArrayToImage(byte[] imageByte)
        {
            MemoryStream ms = new MemoryStream(imageByte);
            Image image = Image.FromStream(ms);
            return image;
        }

        public static byte[] ImageToByteArray(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            byte[] bmpBytes = ms.ToArray();
            return bmpBytes;
        }
         
        public static byte[] LoadResource(Assembly assembly, string resource)
        {
            string name = assembly.GetName().Name;
            Stream imageStream = assembly.GetManifestResourceStream(name + "." + resource);

            if (imageStream != null)
            {
                byte[] image = new byte[imageStream.Length];
                imageStream.Read(image, 0, image.Length);
                imageStream.Close();
                return image;
            }
            return null;
        }
    }
}
