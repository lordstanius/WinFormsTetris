using System;
using System.Windows.Forms;

namespace Game
{
	public partial class Main : Form
	{
		public Main()
		{
			InitializeComponent();

			radioButton1.Tag = 2;
			radioButton2.Tag = 0;
			radioButton3.Tag = 1;
		}

		private void btnStartStop_Click(object sender, System.EventArgs e)
		{
			if (btnStartStop.Text == "Start")
			{
				btnStartStop.Text = "Stop";
				if (tetris.IsRunning)
					tetris.Start();
				else
					tetris.NewGame();
				groupBox1.Enabled = false;
				tetris.Focus();
			}
			else
			{
				btnStartStop.Text = "Start";
				groupBox1.Enabled = true;
				tetris.Stop();
			}
		}

		private void OnSetSelect(object sender, System.EventArgs e)
		{
			tetris.Set = Convert.ToInt32(((RadioButton)sender).Tag);
		}
	}
}
