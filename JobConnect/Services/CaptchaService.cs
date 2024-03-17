using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using ViewModel;

namespace Services
{
    public interface ICaptchaService
    {
        string GetImage(string clientGuid);
        bool Validate(Captcha request);
    }
    public class CaptchaService : ICaptchaService
    {
        private readonly ConcurrentDictionary<string, string> _dictionary;
        private readonly Brush[] _whiteBrushes;
        private readonly Brush[] _darkBrushes;

        private const string CaptchaCharacters = "۱۲۳۴۵۶۷۸۹۰";
        private const string FontName = "Tahoma";
        private int CaptchaLength = 5;

        public CaptchaService()
        {
            _dictionary = new ConcurrentDictionary<string, string>();

            _whiteBrushes = new[]
            {
                Brushes.White,
                Brushes.AliceBlue,
                Brushes.AntiqueWhite,
                Brushes.Azure,
                Brushes.Beige,
                Brushes.Cornsilk,
                Brushes.FloralWhite,
                Brushes.WhiteSmoke,
                Brushes.SeaShell,
                Brushes.OldLace,
                Brushes.MintCream,
                Brushes.LightYellow,
                Brushes.LightCyan
            };
            _darkBrushes = new[]
            {
                Brushes.Gray,
                Brushes.DarkBlue,
                Brushes.MediumBlue,
                Brushes.Navy,
            };
        }

        public string? GetImage(string clientGuid)
        {
            if (string.IsNullOrEmpty(clientGuid))
                return null;

            if (_dictionary.ContainsKey(clientGuid))
                _dictionary.TryRemove(clientGuid, out _);

            var rand = new Random();
            var value = new string(Enumerable.Repeat(CaptchaCharacters, CaptchaLength)
                .Select(s => s[rand.Next(s.Length)]).ToArray());

            _dictionary.TryAdd(clientGuid, value);

            var width = 100;
            var height = 40;

            using var memoryStream = new MemoryStream();
            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);

            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillRectangle(_whiteBrushes[rand.Next(_whiteBrushes.Length)], new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            var pen = new Pen(Color.Gray);
            int i;
            for (i = 1; i < 15; i++)
            {
                pen.Color = Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
                var x = rand.Next(0, width);
                var r = rand.Next(0, width / 3);
                var y = rand.Next(0, height);
                graphics.DrawEllipse(pen, x - r, y - r, r, r);
            }
            graphics.DrawString(value, new Font(FontName, 18), _darkBrushes[rand.Next(_darkBrushes.Length)], 2, 3);
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            var base64String = Convert.ToBase64String(memoryStream.ToArray());

#if DEBUG
            base64String += "?" + value;
#endif

            return base64String;
        }

        public bool Validate(Captcha request)
        {
            if (request == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(request.CaptchaCode) || string.IsNullOrEmpty(request.ClientGuid))
            {
                return false;
            }

            _dictionary.TryRemove(request.ClientGuid, out var lastValue);

            if (string.IsNullOrEmpty(lastValue))
            {
                return false;
            }

            return lastValue == request.CaptchaCode;
        }
    }
}
