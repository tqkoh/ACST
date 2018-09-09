﻿namespace ACST
{
	using System.Diagnostics.CodeAnalysis;
	using System.Windows;
	using System.Windows.Controls;
	using HtmlAgilityPack;
	using System.Collections.Generic;
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;


	/// <summary>
	/// Interaction logic for AtCoder_Sample_TesterControl.
	/// </summary>
	public partial class AtCoder_Sample_TesterControl : UserControl
	{
		List<string> testin;
		List<string> testout;
		int casenum;
		private static readonly HttpClient client = new HttpClient();
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
			HtmlDocument doc = new HtmlDocument();
			try
			{
				doc.LoadHtml(await client.GetStringAsync(loginurl));
				//doc = web.Load(loginurl);
			}
			catch
			{
				MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "atcoder login failed(0)"),
				"AtCoder Sample Tester");
				return;
			}
			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//input[@name=\"csrf_token\"]");
			if (nodes == null)
			{
				MessageBox.Show(
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
			MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "username:{0}, password:{1}, csrf_token:{2}",senddata["username"],senddata["password"],senddata["csrf_token"]),
				"AtCoder Sample Tester");


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
			logstatus.Content = usernameBox.Text;
		}

		private void Button_signout_Click(object sender, RoutedEventArgs e){
			
		}

		public void SubmitURLAsync(string url, string source, string langid = "3003"){
			
		}

		public async Task SubmitAsync(string contestid, string problemid, string source, string langid = "3003")
		{
			string submiturl = string.Format(System.Globalization.CultureInfo.CurrentUICulture, "https://beta.atcoder.jp/contests/{0}/submit",contestid);


			//get csrf_token from loginpage

			//HtmlWeb web = new HtmlWeb();
			HtmlDocument doc = new HtmlDocument();
			try
			{
				doc.LoadHtml(await client.GetStringAsync(submiturl));
				//doc = web.Load(loginurl);
			}
			catch
			{
				MessageBox.Show(
				string.Format(System.Globalization.CultureInfo.CurrentUICulture, "atcoder submit failed(0)"),
				"AtCoder Sample Tester");
				return;
			}
			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//input[@name=\"csrf_token\"]");
			if (nodes == null)
			{
				MessageBox.Show(
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

		public async Task SubmitURLAsync(string contesturl, string source, int langid=3003){
			Dictionary<string, string> senddata = new Dictionary<string, string>(10);
		}

		private void Button_submittest_Click(object sender, RoutedEventArgs e)
		{
			string source = "#include \"bits/stdc++.h\"\nusing namespace std;\nint main(){\n  int a,b;cin>>a>>b;\n  cout<<--a*--b;\n}";
			SubmitAsync("abc106", "abc106_a", source);
		}
	}
}