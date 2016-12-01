using System;
using System.Drawing;
using System.Windows.Forms;
using WixSharp;
using WixSharp.UI.Forms;

namespace MyProduct
{
    public partial class UserNameDialog : ManagedForm, IManagedDialog
    {
        public UserNameDialog()
        {
            InitializeComponent();
        }

        void dialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");

            name.Text = Defaults.UserName;
            password.Text = MsiRuntime.Session["PASSWORD"];

            localDomain.Checked = true;

            ResetLayout();
        }

        void ResetLayout()
        {
            // The form controls are properly anchored and will be correctly resized on parent form 
            // resizing. However the initial sizing by WinForm runtime doesn't do a good job with DPI 
            // other than 96. Thus manual resizing is the only reliable option apart from going WPF.  
            float ratio = (float) banner.Image.Width / (float) banner.Image.Height;
            topPanel.Height = (int) (banner.Width / ratio);

            var upShift = (int) (next.Height * 2.3) - bottomPanel.Height;
            bottomPanel.Top -= upShift;
            bottomPanel.Height += upShift;

            middlePanel.Top = topPanel.Bottom + 10;
            middlePanel.Height = (bottomPanel.Top - 10) - middlePanel.Top;
        }

        void back_Click(object sender, EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, EventArgs e)
        {
            MsiRuntime.Session["PASSWORD"] = password.Text;
            MsiRuntime.Session["DOMAIN"] = domain.Text;
            MsiRuntime.Data["test"] = "test value";
            Shell.GoNext();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

        void DomainType_CheckedChanged(object sender, EventArgs e)
        {
            if (localDomain.Checked)
                domain.Text = Environment.MachineName;
            else if (networkDomain.Checked)
                domain.Text = Environment.UserDomainName;

            UpdateEnabledStates();
        }

        void password_TextChanged(object sender, EventArgs e)
        {
            UpdateEnabledStates();
        }

        void UpdateEnabledStates()
        {
            domain.Enabled = networkDomain.Checked;
            next.Enabled = password.Text.IsNotEmpty();
        }
    }
}