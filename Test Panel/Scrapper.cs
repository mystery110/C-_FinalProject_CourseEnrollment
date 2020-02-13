using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; // For using stream
using System.Net; // For using WebClient
using ktf;  // For using Kuto 

namespace Test_Panel
{
    class Scrapper
    {
        public List<Dictionary<string, string>> GetCourseList()
        {
            // http://course-query.acad.ncku.edu.tw/qry/index.php?lang=en
            string URL = "http://course-query.acad.ncku.edu.tw/qry/index.php?lang=zh_tw";
            string HTML = LoadHTML(URL);

            // Import and using Kuto 
            Kuto kuto = new Kuto(HTML);
            kuto = kuto.Extract("<div class='dept'>", "<div id='result_set'"); // Get the part that we want it 

            List<Dictionary<string, string>> courses = new List<Dictionary<string, string>>();
            while (kuto.Contains("class='dept'>"))
            {
                Dictionary<string, string> courseInfo = new Dictionary<string, string>();
                string name = kuto.Extract(">", "</a>").ToString().Trim();
                string link = kuto.Extract("href='", "'>").ToString().Trim();

                courseInfo.Add("name", name);
                courseInfo.Add("link", "http://course-query.acad.ncku.edu.tw/qry/" + link);
                kuto = kuto.Extract("class='dept'>", "");   // Remove the current item

                courses.Add(courseInfo);
            }
            return courses;
        }

        public List<Dictionary<string, string>> GetClassList(string link)
        {
            string HTML = LoadHTML(link);

            // Import and using Kuto 
            Kuto kuto = new Kuto(HTML);
            kuto = kuto.Extract("<tbody>", " </tbody>"); // Get the part that we want it

            List<Dictionary<string, string>> classes = new List<Dictionary<string, string>>();
            while (kuto.Contains("<TR class="))
            {
                Dictionary<string, string> classInfo = new Dictionary<string, string>();
                kuto = kuto.Extract("<TD", "");
                string dept = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("<TD", "");
                string code = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("<TD", "");
                string serial = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("<TD", "");
                kuto = kuto.Extract("<TD", "");
                kuto = kuto.Extract("<TD", "");
                string classNo = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("href=", "");
                string year = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("<TD", "");
                string category = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("<TD", "");
                kuto = kuto.Extract("<TD", "");
                string english = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("href=", "");
                string course = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("<TD", "");
                string type = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("<TD", "");
                string credit = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("<TD", "");
                string instructor = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("<TD", "");
                kuto = kuto.Extract("<TD", "");
                kuto = kuto.Extract("<TD", "");
                string schedule = kuto.Extract("<BR>", "<").ToString().Trim();
                kuto = kuto.Extract("href=", "");
                string classroom = kuto.Extract(">", "<").ToString().Trim();
                kuto = kuto.Extract("</TR>", "");

                classInfo.Add("dept", dept);
                classInfo.Add("code", code);
                classInfo.Add("serial", serial);
                classInfo.Add("classNo", classNo);
                classInfo.Add("year", year);
                classInfo.Add("category", category);
                classInfo.Add("english", english);
                classInfo.Add("course", course);
                classInfo.Add("type", type);
                classInfo.Add("credit", credit);
                classInfo.Add("instructor", instructor);
                classInfo.Add("schedule", schedule);
                classInfo.Add("classroom", classroom);

                classes.Add(classInfo);
            }
            return classes;
        }

        private string LoadHTML(string URL)
        {
            string HTML = "";
            WebClient webClient = new WebClient(); // for sending and receiving data from URL
            Stream stream = webClient.OpenRead(URL); // open a readable stream for the data downloaded
            StreamReader reader = new StreamReader(stream, Encoding.UTF8); // stream to be read
            HTML = reader.ReadToEnd(); // read all character from the stream
            stream.Close();
            reader.Close();
            return HTML;
        }
    }
}
