using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CreateAnimationFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var source = @"C:\Work\Samples\AndroidSamples\trunk\App1\App1\Resources\drawable\01_kiss_1";
            textBoxSource.Text = source;
            textBoxDestination.Text = @"C:\Work\Samples\AndroidSamples\trunk\App1\App1\Resources\anim";
            textBoxFrames.Text = "15";
            textBoxFileName.Text = source.Substring(source.LastIndexOf("\\") + 1);

            InitializeListView();

            FillListView(textBoxSource.Text);
        }

        private void InitializeListView()
        {
            listView1.View = View.Details;
            listView1.CheckBoxes = true;

            listView1.Columns.Add("", 30);
            listView1.Columns.Add("Name", 200);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.SelectedPath = textBoxSource.Text;
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxSource.Text = dialog.SelectedPath;
                textBoxFileName.Text = textBoxSource.Text.Substring(textBoxSource.Text.LastIndexOf("\\") + 1);
                textBoxXML.Text = string.Empty;
                FillListView(textBoxSource.Text);
            }
        }

        private void FillListView(string folder)
        {
            
            listView1.Items.Clear();

            var files = Directory.GetFiles(folder);

            foreach (var file in files)
            {
                var f = new FileInfo(file);

                var item = new ListViewItem("");
                item.Checked = true;
                item.SubItems.Add(f.Name);

                listView1.Items.Add(item);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.SelectedPath = textBoxDestination.Text;
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxDestination.Text = dialog.SelectedPath;
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            var duration = (int)(1000 / int.Parse(textBoxFrames.Text));

            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            sb.AppendLine(@"<animation-list xmlns:android=""http://schemas.android.com/apk/res/android"" android:oneshot=""true"">");

            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    var name = item.SubItems[1].Text;
                    sb.AppendLine(string.Format(@"<item android:drawable=""@drawable/{0}"" android:duration=""{1}"" />", name.Substring(0, name.IndexOf(".")), duration));
                }
            }

            sb.AppendLine(@"</animation-list>");

            textBoxXML.Text = sb.ToString();

            // save to file
            File.WriteAllText(Path.Combine(textBoxDestination.Text, textBoxFileName.Text + ".xml"), sb.ToString());
        }
    }
}
