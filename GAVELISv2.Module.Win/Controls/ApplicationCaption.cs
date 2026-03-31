using System;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
namespace GAVELISv2.Module.Win.Controls
{
    public class ApplicationCaption : ApplicationCaption8_1 {
        private Image _RLLogo;
        public ApplicationCaption() {
            InitializeComponent();
            ShowLogo(false);
            TabStop = false;
        }
        public Image RLLogo {
            get { return _RLLogo; }
            set {
                if (_RLLogo == value) {return;
                }
                _RLLogo = value;
                this.Refresh();}
        }
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (RLLogo == null) {return;
            }
            e.Graphics.DrawImage(RLLogo, this.Width - RLLogo.Width - deltaX, (
            this.Height - RLLogo.Height) / 2, RLLogo.Width, RLLogo.Height);
        }
        protected override void DrawCaptions(DevExpress.Utils.Drawing.
        GraphicsCache cache, Rectangle r, int textLeft) {
            if (RLLogo == null) {
                base.DrawCaptions(cache, r, textLeft);
                return;
            }
            using (SolidBrush foreBrush = new SolidBrush(this.ForeColor)) {
                int textTop = (this.Height - (Font.Height + (Text2 != string.
                Empty ? Font2.Height + deltaY : 0))) / 2 - 1;
                r = new Rectangle(textLeft, textTop, this.Width - textLeft - 
                RLLogo.Width - deltaX, Font.Height);
                cache.Graphics.TextRenderingHint = System.Drawing.Text.
                TextRenderingHint.AntiAlias;
                if (r.Width > 0) {cache.Graphics.DrawString(Text, this.Font, 
                    foreBrush, r, TextStringFormat);
                }
                r = new Rectangle(textLeft + deltaText2, textTop + Font.Height + 
                deltaY, this.Width - textLeft - RLLogo.Width - deltaX - 
                deltaText2, Font2.Height);
                if (r.Width > 0) {cache.Graphics.DrawString(Text2, this.Font2, 
                    foreBrush, r, TextStringFormat);
                }
            }
        }
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // ApplicationCaption
            // 
            this.ResumeLayout(false);
        }
    }
}
