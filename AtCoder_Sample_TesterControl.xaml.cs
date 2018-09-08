namespace ACST
{
	using System.Diagnostics.CodeAnalysis;
	using System.Windows;
	using System.Windows.Controls;
	using HtmlAgilityPack;
	using System.Collections.Generic;


	/// <summary>
	/// Interaction logic for AtCoder_Sample_TesterControl.
	/// </summary>
	public partial class AtCoder_Sample_TesterControl : UserControl
	{
		List<string> testin;
		List<string> testout;
		int casenum;
		/// <summary>
		/// Initializes a new instance of the <see cref="AtCoder_Sample_TesterControl"/> class.
		/// </summary>
		public AtCoder_Sample_TesterControl()
		{
			this.InitializeComponent();
			testin = new List<string>();
			testout = new List<string>();
			casenum = 0;
		}

		/// <summary>
		/// Handles click on the button by displaying a message box.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event args.</param>
		[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
		private void button1_Click(object sender, RoutedEventArgs e)
		{
			HtmlWeb web = new HtmlWeb();
			HtmlDocument doc;
			try
			{
				doc = web.Load(problempage.Text);
			}
			catch
			{
				htmltest.Text = "That page doesn't exist";
				return;
			}



			var nodes = doc.DocumentNode.SelectNodes("//span[@class=\"lang-ja\"]/div/section/pre");
			testin.Clear();
			testout.Clear();
			htmltest.Text = "";
			casenum = nodes.Count;
			for (int i = 0; i < casenum / 2; ++i){
				testin.Add(nodes[2 * i].InnerText);
				testout.Add(nodes[2 * i + 1].InnerText);
				htmltest.Text += "in[" + i.ToString() + "]:\n";
				htmltest.Text += nodes[2 * i].InnerText;
				htmltest.Text += "out[" + i.ToString() + "]:\n";
				htmltest.Text += nodes[2 * i + 1].InnerText + "\n";
			}
		}

		private void button_copyinput_Click(object sender, RoutedEventArgs e)
		{
			int idx = 0;
			int.TryParse(copyinputarg1.Text, out idx);
			if(idx<casenum/2) Clipboard.SetDataObject(testin[idx], true);
			else MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "index is too big"),
				"AtCoder Sample Tester");
		}

		private void button_copyoutput_Click(object sender, RoutedEventArgs e)
		{
			int idx = 0;
			int.TryParse(copyinputarg2.Text, out idx);
			if (idx < casenum) Clipboard.SetDataObject(testout[idx], true);
			else MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "index is too big"),
				"AtCoder Sample Tester");
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}
	}
}