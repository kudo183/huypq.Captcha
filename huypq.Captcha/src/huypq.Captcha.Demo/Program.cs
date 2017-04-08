using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace huypq.Captcha.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int id;
            SimpleCaptcha.Instance.GenerateCaptcha(out id);
            var b1 = SimpleCaptcha.Instance.GenerateCaptcha(out id);
            var t1 = SimpleCaptcha.Instance.VerifyCaptcha(id, System.Text.Encoding.UTF8.GetString(b1));
            SimpleCaptcha.Instance.SetCaptchaLifeTime(10);
            SimpleCaptcha.Instance.GenerateCaptcha(out id);
            SimpleCaptcha.Instance.GenerateCaptcha(out id);
            var b2 = SimpleCaptcha.Instance.GenerateCaptcha(out id);
            System.Threading.Thread.Sleep(15 * 1000);
            var t2 = SimpleCaptcha.Instance.VerifyCaptcha(id, System.Text.Encoding.UTF8.GetString(b2));
        }
    }
}
