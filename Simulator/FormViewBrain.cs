namespace Simulator
{
    public partial class FormViewBrain : Form
    {
        public FormViewBrain()
        {
            InitializeComponent();
        }
        public FormViewBrain(string text)
        {
            InitializeComponent();

            textBoxText.Text = text;
        }
    }
}
