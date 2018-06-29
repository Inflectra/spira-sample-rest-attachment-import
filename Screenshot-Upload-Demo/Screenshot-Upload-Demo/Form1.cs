using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Screenshot_Upload_Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.txtFilename.Text = this.openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        /// <summary>
        /// Uploads the file
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            //Create the full URL
            string url = this.txtUrl.Text.Trim() + "/Services/v5_0/RestService.svc/projects/{project_id}/documents/file?filename={filename}";
            string projectId = this.txtProject.Text.Trim();
            string filename = Path.GetFileName(this.txtFilename.Text);
            url = url.Replace("{project_id}", projectId);
            url = url.Replace("{filename}", filename);

            //Create the HTTP Post
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = "POST";

            //Add the authentication
            string base64Credentials = GetEncodedCredentials();
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            //Read the file and turn into JSON byte array
            byte[] contents = File.ReadAllBytes(this.txtFilename.Text);
            string json = "[";
            bool first = true;
            foreach (byte item in contents)
            {
                if (!first)
                {
                    json += ",";
                }
                else
                {
                    first = false;
                }
                json += item.ToString();
            }
            json += "]";
            
            //Add the filename body
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                //writer.Write("[0,0,0,0,0,0,0,0,0,0,0,0,0]");
                writer.Write(json);
            }

            //Fire the query and get the response
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string result = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            MessageBox.Show("Done!");
        }

        /// <summary>
        /// Gets the base64-encoded login/password
        /// </summary>
        /// <returns></returns>
        private string GetEncodedCredentials()
        {
            string mergedCredentials = string.Format("{0}:{1}", this.txtLogin.Text.Trim(), this.txtPassword.Text.Trim());
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }

        private void txtUrl_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
