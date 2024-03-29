﻿using System.IO;


namespace ACST
{
	using System.Diagnostics.CodeAnalysis;
	using System.Windows.Controls;
	using HtmlAgilityPack;
	using System.Collections.Generic;
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using System.Windows;
	using System.Windows.Documents;


	/// <summary>
	/// Interaction logic for AtCoder_Sample_TesterControl.
	/// </summary>
	public partial class AtCoder_Sample_TesterControl : System.Windows.Controls.UserControl
	{
		List<string> testin;
		List<string> testout;
		List<string> results;
		int casenum;
		private static readonly HttpClient client = new HttpClient();
		/// <summary>
		/// Initializes a new instance of the <see cref="AtCoder_Sample_TesterControl"/> class.
		/// </summary>
		public AtCoder_Sample_TesterControl()
		{
			InitializeComponent();
			testin = new List<string>();
			testout = new List<string>();
			results = new List<string>();
			casenum = 0;

			try
			{
				using (StreamReader r = new StreamReader(@"ACSTuser.dat"))
				{
					usernameBox.Text = r.ReadLine();
					passwordBox.Password = r.ReadLine();
				}
				Button_signin_ClickTask(null,null);
			}catch{
				System.Windows.MessageBox.Show(
					string.Format(System.Globalization.CultureInfo.CurrentUICulture, "First use? Login atcoder on the textbox below"),
					"AtCoder Sample Tester");
			}
			try{
				using (StreamReader c = new StreamReader(@"ACSTconfig.dat"))
				{
					string lin;
					lin = c.ReadLine();
					sourcebeginpos.Text = lin;
					lin = c.ReadLine();
					sourceendpos.Text = lin;
					lin = c.ReadLine();
					cppfile.Text = lin;
					lin = c.ReadLine();
					exefile.Text = lin;
				}
			}catch{ }
		}

		/// <summary>
		/// Handles click on the button by displaying a message box.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event args.</param>
		[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
		private void CheckOnly_Click(object sender, RoutedEventArgs e)
		{
			HtmlWeb web = new HtmlWeb();
			HtmlAgilityPack.HtmlDocument doc;
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
			if(nodes==null){
				htmltest.Text = "That page is not a task page";
				return;
			}
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

			//kkari
			//htmltest.Text += "sample0: "+Test(testin[0], testout[0], 0);
			results.Clear();
			outputResult.SelectAll();
			outputResult.Selection.Text = "";

			for (int i=0;i<testin.Count; ++i){
				TextRange defRange = new TextRange(outputResult.Document.ContentEnd, outputResult.Document.ContentEnd);
				defRange.Text = "sample" + (i + 1).ToString() + ": ";
				defRange.ApplyPropertyValue(TextElement.ForegroundProperty, System.Windows.Media.Brushes.Black);
				defRange.ApplyPropertyValue(TextElement.BackgroundProperty, System.Windows.Media.Brushes.White);
				defRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Regular);
				TextRange resultRange = new TextRange(outputResult.Document.ContentEnd, outputResult.Document.ContentEnd);
				resultRange.Text = Test(testin[i], testout[i], i);
				results.Add(resultRange.Text);
				resultRange.ApplyPropertyValue(TextElement.ForegroundProperty, System.Windows.Media.Brushes.White);
				if(resultRange.Text==" AC ")resultRange.ApplyPropertyValue(TextElement.BackgroundProperty, System.Windows.Media.Brushes.LimeGreen);
				else if(resultRange.Text == " WA " || resultRange.Text == " TLE ") resultRange.ApplyPropertyValue(TextElement.BackgroundProperty, System.Windows.Media.Brushes.Orange);
				else if (resultRange.Text == " FE ") resultRange.ApplyPropertyValue(TextElement.BackgroundProperty, System.Windows.Media.Brushes.Red);
				resultRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
				outputResult.AppendText("\n");
			}
		}

		private void SubmitOnly_Click(object sender, RoutedEventArgs e)
		{
			string source = "";//"#include \"bits/stdc++.h\"\nusing namespace std;\nint main(){\n  int a,b;cin>>a>>b;\n  cout<<--a*--b;\n}";
			FileStream fs;
			StreamReader sr;

			try
			{
				fs = new FileStream(cppfile.Text,//kari
						FileMode.Open, FileAccess.Read);
				sr = new StreamReader(fs);

			}
			catch
			{
				System.Windows.MessageBox.Show(
					string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Source file not found. Go to AtCoder to sumbit directly or check the file path on the config below."),
					"AtCoder Sample Tester");
				return;
			}
			
			if (sourcebeginpos.Text == "")//all
			{
				source = sr.ReadToEnd();
			}
			else//begin=>end
			{
				Int64 beginp = 0, endp = int.MaxValue;
				try
				{
					beginp = Int64.Parse(sourcebeginpos.Text) - 1;
					endp = Int64.Parse(sourceendpos.Text) - 1;
				}
				catch
				{
					DialogResult res = System.Windows.Forms.MessageBox.Show("Invalid input.(not a number) Do you want to submit the whole source code?",
						"AtCoder Sample Tester", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (res == DialogResult.Yes)
					{
						source = sr.ReadToEnd();
						fs.Close(); sr.Close();
						SubmitURLAsync(problempage.Text, source);
					}
					return;
				}
				int pos = 0;
				for (; pos < beginp && !sr.EndOfStream; ++pos) sr.ReadLine();
				for (; pos < endp && !sr.EndOfStream; ++pos) source += sr.ReadLine() + '\n';
			}

			fs.Close(); sr.Close();

			SubmitURLAsync(problempage.Text, source);
		}

		private void CheckAndSubmit_Click(object sender, RoutedEventArgs e){
			CheckOnly_Click(sender, e);
			bool ok=true;
			for(int i=0;i<results.Count;++i){
				if (results[i] != " AC ") ok = false;
			}
			if (ok) SubmitOnly_Click(sender, e);
		}

		private void Button_copyinput_Click(object sender, RoutedEventArgs e)
		{
			int idx = 0;
			int.TryParse(copyinputarg1.Text, out idx);
			if(idx<casenum/2) System.Windows.Clipboard.SetDataObject(testin[idx], true);
			else System.Windows.MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "index is too big"),
				"AtCoder Sample Tester");
		}

		private void Button_copyoutput_Click(object sender, RoutedEventArgs e)
		{
			int idx = 0;
			int.TryParse(copyinputarg2.Text, out idx);
			if (idx < casenum) System.Windows.Clipboard.SetDataObject(testout[idx], true);
			else System.Windows.MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "index is too big"),
				"AtCoder Sample Tester");
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		//private static async Task Login1(){
			
		//}

		private void Button_signin_Click(object sender, RoutedEventArgs e){
			Button_signin_ClickTask(sender, e);
		}

		public async Task Button_signin_ClickTask(object sender, RoutedEventArgs e){
			string loginurl = "https://beta.atcoder.jp/login";
			string acurl = "https://beta.atcoder.jp";

			//System.Net.WebClient wc = new System.Net.WebClient();
			

			//wc.Headers.Add("Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.");
			//wc.Headers.Add("User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)");
			//wc.Headers.Add("Accept-Encoding: gzip, deflate, br");
			//wc.Headers.Add("Accept-Language: ja,en-US;q=0.9,en;q=0.8,und;q=0.7");
			//wc.Headers.Add("Cache-Control: max-age=0");
			//wc.Headers.Add("Upgrade-Insecure-Requests: 1");

			//wc.Headers.Add("User-Agent: Other");

			//NameValueCollectionの作成
			//System.Collections.Specialized.NameValueCollection senddata =
			//	new System.Collections.Specialized.NameValueCollection();


			//send login data

			Dictionary<string,string> senddata = new Dictionary<string, string>(10);
			//get csrf_token from loginpage

			//HtmlWeb web = new HtmlWeb();
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			try
			{
				doc.LoadHtml(await client.GetStringAsync(loginurl));
				//doc = web.Load(loginurl);
			}
			catch
			{
				System.Windows.MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "atcoder login failed(0)"),
				"AtCoder Sample Tester");
				return;
			}
			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//input[@name=\"csrf_token\"]");
			if (nodes == null)
			{
				System.Windows.MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "atcoder login failed(1)"),
				"AtCoder Sample Tester");
				return;
			}

			string csrf_token = nodes[0].GetAttributeValue("value", "");
			//append senddata
			senddata.Add("username", usernameBox.Text);
			senddata.Add("password", passwordBox.Password);
			senddata.Add("csrf_token", csrf_token);
			var payload = new FormUrlEncodedContent(senddata);
			//System.Windows.MessageBox.Show(
			//	string.Format(System.Globalization.CultureInfo.CurrentUICulture, "username:{0}, password:{1}, csrf_token:{2}",senddata["username"],senddata["password"],senddata["csrf_token"]),
			//	"AtCoder Sample Tester");


			System.Net.Http.HttpResponseMessage resData;
			//System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> resData;
			//send data
			try
			{
				resData = await Task.Run(() => client.PostAsync(loginurl, payload));//await
				//resData = wc.UploadValues(loginurl, senddata);
			}
			catch (Exception ex)
			{
				logstatus.Content = ex.Message;
				//string str = wc.DownloadString(loginurl);
				//logstatus.Content = str;
				return;
			}



			//wc.Dispose();

			//
			//string resText = System.Text.Encoding.UTF8.GetString(resData);
			//logstatus.Content = ;
			string responseString = await Task.Run(() => client.GetStringAsync(loginurl));
			//htmltest.Text = responseString;
			logstatus.Content = "logged in with "+usernameBox.Text;

			using (StreamWriter w = new StreamWriter(@"ACSTuser.dat"))
			{
				await w.WriteLineAsync(usernameBox.Text);
				await w.WriteLineAsync(passwordBox.Password);
			}


		}

		private void Button_save_Click(object sender, RoutedEventArgs e){
			using (StreamWriter w = new StreamWriter(@"ACSTconfig.dat",false))
			{
				w.WriteLine(sourcebeginpos.Text);
				w.WriteLine(sourceendpos.Text);
				w.WriteLine(cppfile.Text);
				w.WriteLine(exefile.Text);
			}
		}

		public void SubmitURLAsync(string url, string source, string langid = "3003"){
			string[] elem = url.Split('/');
			string contestid, problemid;
			if (elem.Length < 4) return;
			if (elem[elem.Length-1]==""){
				problemid = elem[elem.Length - 2];
				contestid = elem[elem.Length - 4];
			}else{
				problemid = elem[elem.Length - 1];
				contestid = elem[elem.Length - 3];
			}
			SubmitAsync(contestid, problemid, source);
		}

		public async Task SubmitAsync(string contestid, string problemid, string source, string langid = "3003")
		{
			string submiturl = string.Format(System.Globalization.CultureInfo.CurrentUICulture, "https://beta.atcoder.jp/contests/{0}/submit",contestid);


			//get csrf_token from loginpage

			//HtmlWeb web = new HtmlWeb();
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			try
			{
				doc.LoadHtml(await client.GetStringAsync(submiturl));
				//doc = web.Load(loginurl);
			}
			catch
			{
				System.Windows.MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "atcoder submit failed(0)"),
				"AtCoder Sample Tester");
				return;
			}
			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//input[@name=\"csrf_token\"]");
			if (nodes == null)
			{
				System.Windows.MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "atcoder submit failed(1)"),
				"AtCoder Sample Tester");
				return;
			}

			string csrf_token = nodes[0].GetAttributeValue("value", "");

			Dictionary<string, string> senddata = new Dictionary<string, string>(4);
			senddata["data.TaskScreenName"] = problemid;
			senddata["data.LanguageId"] = langid;
			senddata["sourceCode"] = source;
			senddata["csrf_token"] = csrf_token;
			var payload = new FormUrlEncodedContent(senddata);

			System.Net.Http.HttpResponseMessage resData;
			//System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> resData;
			//send data
			try
			{
				resData = await Task.Run(() => client.PostAsync(submiturl, payload));//await
																					//resData = wc.UploadValues(loginurl, senddata);
			}
			catch (Exception ex)
			{
				logstatus.Content = ex.Message;
				//string str = wc.DownloadString(loginurl);
				//logstatus.Content = str;
				return;
			}
		}

		

		public string Test(string inp,string outp,int num){
			string command = exefile.Text;

			//DirectoryUtils.SafeCreateDirectory("ACST");
			//File.WriteAllText("ACST\\in" + num + ".txt", inp);

			System.Diagnostics.ProcessStartInfo psInfo = new System.Diagnostics.ProcessStartInfo();

			psInfo.FileName = command;
			//psInfo.Arguments = @"0< ""ACST\in" + num + @".txt"""; ;//kari
			//psInfo.CreateNoWindow = true;
			psInfo.UseShellExecute = false;
			psInfo.RedirectStandardInput = true;

			psInfo.RedirectStandardOutput = true;

			System.Diagnostics.Process p;
			try
			{
				p = System.Diagnostics.Process.Start(psInfo);
			}catch{
				System.Windows.MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, ".exe file not found. Check file path below.\ncd="+ System.IO.Directory.GetCurrentDirectory()),
				"AtCoder Sample Tester");
				return " FE ";
			}
			inp = inp.Replace("\r\n", "\n");
			string[] arr = inp.Split('\n');
			foreach(string line in arr){
				p.StandardInput.WriteLine(line);
			}
			p.StandardInput.Close();
			p.WaitForExit(3000);
			if (!p.HasExited) {
				p.Kill();
				p.Close();
				return " TLE ";
			}
			
			string output = p.StandardOutput.ReadToEnd();

			output = output.Replace("\r\r\n", "\r\n");
			p.Close();
			if (outp == output) return " AC ";
			else return " WA ";
			//return "AC";
		}
	}
}