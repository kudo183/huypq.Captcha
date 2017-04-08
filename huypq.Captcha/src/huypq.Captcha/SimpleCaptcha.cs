using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace huypq.Captcha
{
    public class SimpleCaptcha
    {
        private static readonly SimpleCaptcha _instance = new SimpleCaptcha();
        public static SimpleCaptcha Instance
        {
            get { return _instance; }
        }

        private static long CaptchaLifeTime { get; set; }//in ticks

        private SimpleCaptcha()
        {
            CaptchaLifeTime = 2 * 60 * TimeSpan.TicksPerSecond;
        }

        class CaptchaBufferItem
        {
            public int ID { get; set; }
            public string Value { get; set; }
            public long GenerateTime { get; set; }

            public bool IsAlive()
            {
                return ((DateTime.UtcNow.Ticks - GenerateTime) < CaptchaLifeTime);
            }
        }

        public enum CaptchaVerifyResult
        {
            OK,
            WrongValue,
            Timeout,
            NotFound
        }

        int _captchaID = 1;
        LinkedList<CaptchaBufferItem> _buffer = new LinkedList<CaptchaBufferItem>();

        public void SetCaptchaLifeTime(int second)
        {
            CaptchaLifeTime = second * TimeSpan.TicksPerSecond;
        }

        public byte[] GenerateCaptcha(out int captchaID)
        {
            var node = _buffer.First;
            while (node != null)
            {
                var next = node.Next;
                if (node.Value.IsAlive() == false)
                {
                    _buffer.Remove(node);
                }
                node = next;
            }

            _captchaID++;
            captchaID = _captchaID;

            string captchaValue = GenerateRandomCode();

            _buffer.AddLast(new CaptchaBufferItem()
            {
                ID = _captchaID,
                Value = captchaValue,
                GenerateTime = DateTime.UtcNow.Ticks
            });

            //var result = new byte[2];//generate image bytes from value
            var result = System.Text.Encoding.UTF8.GetBytes(captchaValue);
            return result;
        }
        
        public CaptchaVerifyResult VerifyCaptcha(int captchaId, string captchaValue)
        {
            var node = _buffer.First;
            while (node != null)
            {
                var next = node.Next;

                if (node.Value.ID == captchaId)
                {
                    if (node.Value.IsAlive() == false)
                    {
                        _buffer.Remove(node);
                        return CaptchaVerifyResult.Timeout;
                    }
                    if (node.Value.Value != captchaValue)
                    {
                        return CaptchaVerifyResult.WrongValue;
                    }
                    _buffer.Remove(node);
                    return CaptchaVerifyResult.OK;
                }
                else if (node.Value.IsAlive() == false)
                {
                    _buffer.Remove(node);
                }
                node = next;
            }
            return CaptchaVerifyResult.NotFound;
        }

        Random r = new Random();
        private string GenerateRandomCode()
        {
            string s = "";
            for (int j = 0; j < 5; j++)
            {
                int i = r.Next(3);
                int ch;
                switch (i)
                {
                    case 1:
                        ch = r.Next(0, 9);
                        s = s + ch.ToString();
                        break;
                    case 2:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    case 3:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    default:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                }
                r.NextDouble();
                r.Next(100, 1999);
            }
            return s;
        }

        //https://www.codeproject.com/Articles/169371/Captcha-Image-using-C-in-ASP-NET, wait System.Drawing for dotnet core
        private void GenerateImage()
        {
            //  Bitmap bitmap = new Bitmap
            //    (this.width, this.height, PixelFormat.Format32bppArgb);
            //  Graphics g = Graphics.FromImage(bitmap);
            //  g.SmoothingMode = SmoothingMode.AntiAlias;
            //  Rectangle rect = new Rectangle(0, 0, this.width, this.height);
            //  HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti,
            //      Color.LightGray, Color.White);
            //  g.FillRectangle(hatchBrush, rect);
            //  SizeF size;
            //  float fontSize = rect.Height + 1;
            //  Font font;

            //  do
            //  {
            //      fontSize--;
            //      font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold);
            //      size = g.MeasureString(this.text, font);
            //  } while (size.Width > rect.Width);
            //  StringFormat format = new StringFormat();
            //  format.Alignment = StringAlignment.Center;
            //  format.LineAlignment = StringAlignment.Center;
            //  GraphicsPath path = new GraphicsPath();
            //  //path.AddString(this.text, font.FontFamily, (int) font.Style, 
            //  //    font.Size, rect, format);
            //  path.AddString(this.text, font.FontFamily, (int)font.Style, 75, rect, format);
            //  float v = 4F;
            //  PointF[] points =
            //  {
            //      new PointF(this.random.Next(rect.Width) / v, this.random.Next(
            //         rect.Height) / v),
            //      new PointF(rect.Width - this.random.Next(rect.Width) / v,
            //          this.random.Next(rect.Height) / v),
            //      new PointF(this.random.Next(rect.Width) / v,
            //          rect.Height - this.random.Next(rect.Height) / v),
            //      new PointF(rect.Width - this.random.Next(rect.Width) / v,
            //          rect.Height - this.random.Next(rect.Height) / v)
            //};
            //  Matrix matrix = new Matrix();
            //  matrix.Translate(0F, 0F);
            //  path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
            //  hatchBrush = new HatchBrush(HatchStyle.Percent10, Color.Black, Color.SkyBlue);
            //  g.FillPath(hatchBrush, path);
            //  int m = Math.Max(rect.Width, rect.Height);
            //  for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            //  {
            //      int x = this.random.Next(rect.Width);
            //      int y = this.random.Next(rect.Height);
            //      int w = this.random.Next(m / 50);
            //      int h = this.random.Next(m / 50);
            //      g.FillEllipse(hatchBrush, x, y, w, h);
            //  }
            //  font.Dispose();
            //  hatchBrush.Dispose();
            //  g.Dispose();
            //  this.image = bitmap;
        }
    }
}
