using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.IO;


namespace UtilitiesCollection
{
    public partial class Form1 : Form
    {
        private String str_TextBoxBody;
        private String str_FileName;
        private FileSystemWatcher watcher;
        //@"C:\Development\DevSOA\config\results.xml"
        public Form1()
        {
            InitializeComponent();

            // Create a new FileSystemWatcher and set its properties.
            watcher = new FileSystemWatcher();

            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            // Only watch text files.
            //watcher.Path = @"C:\Development\DevSOA\config\";
            //watcher.Filter = "results.xml";

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            //watcher.EnableRaisingEvents = true;

        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            PerformOperation();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PerformOperation();            
        }

        private void PerformOperation()
        {

            if (InvokeRequired)
            {
                MethodInvoker method = new MethodInvoker(PerformOperation);
                Invoke(method);
                return;
            }

            //str_TextBoxBody = "";
            listBox1.Items.Clear();
            textBox1.Text = "";

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(str_FileName);
            }
            catch (Exception e)
            {

            }

            listBox1.BeginUpdate();
            foreach (XmlNode node in doc.SelectNodes("//testcase"))
            {
                str_TextBoxBody = "";
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    str_TextBoxBody += attribute.Value + "; ";
                }
                listBox1.Items.Add(str_TextBoxBody);
                
            }
            listBox1.EndUpdate();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string testName;
            string failureVerbose;
            XElement test;

            string selectedItemText = listBox1.SelectedItem.ToString();
            if (selectedItemText.Contains("Failure"))
            {
                testName = selectedItemText.Split(';')[0];

                test = XElement.Load(str_FileName);
                var query = from param in test.Descendants("testcase")
                            where ((string)param.Attribute("name")).Contains(testName)
                            select new
                            {
                                failureShort   = (string)param.Element("failure").Attribute("message"),
                                failureVerbose = (string)param.Element("failure")
                                
                            };
                foreach (var item in query)
                {
                    failureVerbose = item.failureVerbose.Replace(".r)",".r)" + Environment.NewLine);
                    failureVerbose = failureVerbose.Replace(".cls)",".cls)" + Environment.NewLine);
                    textBox1.Text = item.failureShort + Environment.NewLine + Environment.NewLine + failureVerbose;
                    failureVerbose = "";
                }
            }
            else
            {
                if (selectedItemText.Contains("Error"))
                {
                    testName = selectedItemText.Split(';')[0];

                    test = XElement.Load(str_FileName);
                    var query2 = from param in test.Descendants("testcase")
                                where ((string)param.Attribute("name")).Contains(testName)
                                select new
                                {
                                    failureVerbose = (string)param.Element("error")
                                };
                    foreach (var item in query2)
                    {
                        failureVerbose = item.failureVerbose.Replace(".r)",".r)" + Environment.NewLine);
                        failureVerbose = failureVerbose.Replace(".cls)",".cls)" + Environment.NewLine);
                        textBox1.Text = failureVerbose;
                        failureVerbose = "";
                    }
                }
                else
                    {
                    textBox1.Text = "";
}
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //
            // Draw the background of the ListBox control for each item.
            // Create a new Brush and initialize to a Black colored brush
            // by default.
            //
            e.DrawBackground();
            Brush myBrush = Brushes.Black;
            //
            // Determine the color of the brush to draw each item based on 
            // the index of the item to draw.
            //
            if (listBox1.Items[e.Index].ToString().Contains("Failure"))
            {
                myBrush = Brushes.Blue;
            }
            else
            { 
                if(listBox1.Items[e.Index].ToString().Contains("Error"))
                {
                    myBrush = Brushes.Red;
                }
                else 
                {
                    myBrush = Brushes.Green;
                }
            }           
            //
            // Draw the current item text based on the current 
            // Font and the custom brush settings.
            //
            e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(),
                e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            //
            // If the ListBox has focus, draw a focus rectangle 
            // around the selected item.
            //
            e.DrawFocusRectangle();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "XML Files (*.xml)|*.xml|Text Files (.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;

            DialogResult dialogResult1 = openFileDialog1.ShowDialog();
            if (dialogResult1 == DialogResult.OK)
            {
                str_FileName = openFileDialog1.FileName;
                textBox2.Text = str_FileName;
                watcher.Path = Path.GetDirectoryName(str_FileName);
                watcher.Filter = Path.GetFileName(str_FileName);
                watcher.EnableRaisingEvents = true;
                PerformOperation();
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            str_FileName = textBox2.Text;
            watcher.Path = Path.GetDirectoryName(str_FileName);
            watcher.Filter = Path.GetFileName(str_FileName);
            watcher.EnableRaisingEvents = true;
            PerformOperation();
        }
    }
}
