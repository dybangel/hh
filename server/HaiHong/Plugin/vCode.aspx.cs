using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace Fooke.Web.Plugin
{
    public partial class vCode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Buffer = false;
            Response.ClearHeaders();
            Response.Clear();
            CreateRand("2");
        }
        //网站验证码页面,生成随即验证码
        /***************************************************************
         * 预设参数
         * CodeStr
         * Length
         * Type
         * Function CreateRand(int Length,string Type)//参数说明
         * Length 验证码长度 Type 验证码类型 纯数字，纯拼音或者混合汉字
         * 生成图片函数
         * Function CreatePhoto(string CodeStr,string BackGround)
         * CodeStr验证字符串,BackGround图片串背景
         * 
         * ReturnVal=返回值
         * 
         * *************************************************************/

        public string CreateRand(string Type)
        {
            string ReturnVal = string.Empty;
            string StrABC = "A,1,a,2,B,3,b,4,C,5,c,D,6,d,7,E,8,e,9,F,f,0,G,g,H,h,I,i,J,j,K,k,L,l,M,m,N,n,O,o,P,p,Q,q,R,r,S,s,T,t,U,u,V,v,W,w,X,x,Y,y,Z,z,1,2,3,4,5,6,7,8,9,0";
            string[] TempABC = StrABC.Split(',');
            //Response.Write(TempABC.Length.ToString());
            Random Rand = new Random();
            string ForStr = string.Empty;
            string RandStr = (Rand.NextDouble() * Rand.NextDouble()).ToString();
            string Length = RandStr.Substring(RandStr.Length - 1, 1);
            int ForLen = (Convert.ToInt32(Length) * Convert.ToInt32(Length) * Convert.ToInt32(Length) % 10);
            if (ForLen < 4 || ForLen > 5)
            {
                ForLen = 5;
            }
            string CodeStr = string.Empty;
            for (int n = 0; n < ForLen; n++)
            {
                string TempStr = RandStr.Substring(RandStr.Length - (n + 3), 2);
                if (Convert.ToInt32(TempStr) >= TempABC.Length)
                {
                    TempStr = TempStr.Substring(TempStr.Length - 1, 1);
                }
                else if (Convert.ToInt32(TempStr) <= 9)
                {
                    TempStr = TempStr.Substring(TempStr.Length - 1, 1);
                }
                CodeStr += TempABC[Convert.ToInt32(TempStr)];
            }
            CreatePhoto(CodeStr, 120, 36);
            return ReturnVal;
        }
        public void CreatePhoto(string CodeStr, int Width, int Height)
        {
            int FontSize = 24;
            Bitmap Bit = new Bitmap(Width, Height);
            Graphics gh = Graphics.FromImage(Bit);
            gh.Clear(Color.Transparent);
            //画出背景噪音线
            Random random = new Random();
            for (int n = 0; n <= 10; n++)
            {
                gh.DrawLine(new Pen(Color.Gray), random.Next(Bit.Width),
                    random.Next(Bit.Width), random.Next(Bit.Width),
                    random.Next(Bit.Width));
            }
            try { Fooke.Function.SessionHelper.Add("vCode", CodeStr.ToString(), 90); }
            catch { }
            try { Fooke.Function.CookieHelper.Add("vCode", CodeStr, 90); }
            catch { }
            Matrix Mat = new Matrix();
            Mat.Shear(0, 0);
            gh.MultiplyTransform(Mat);
            FontStyle FS = FontStyle.Bold;
            Font ft = new Font("宋体", FontSize, FS);
            PointF point = new PointF(Convert.ToInt32((Width - (CodeStr.Length * 15)) / 2), 3);
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, Bit.Width, Bit.Height), Color.CadetBlue, Color.Red, 1.5f, true);
            gh.DrawString(CodeStr, ft, brush, point);
            gh.Save();
            Bit.Save(Response.OutputStream, ImageFormat.Png);
            gh.Dispose();
            Bit.Dispose();
        }
    }
}