using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Devart.Data.SQLite;
using System.Diagnostics;

namespace MR2CSV
{
    public partial class MR2CSV : Form
    {

        String csvPath, path;
        Process p = new Process();
        String currentDirectory = Directory.GetCurrentDirectory().ToString();


        public MR2CSV()
        {
            InitializeComponent();
        }

        private void MR2CSV_Load(object sender, EventArgs e)
        {
            checkBox1.Enabled = false; checkBox2.Enabled = false; button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            dbPath.ShowDialog();
            textBox1.Text = dbPath.FileName.ToString();
            path = textBox1.Text.ToString();
            checkBox1.Enabled = true; checkBox2.Enabled = true; button2.Enabled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SQLiteConnection dbConnection = new SQLiteConnection();
            SQLiteDataReader reader;
            SQLiteCommand cmd = dbConnection.CreateCommand();

            dbConnection.ConnectionString = @"Data Source=" + path + ";FailIfMissing=False;";

            try
            {

                dbConnection.Open();
                cmd.CommandText = "SELECT manga_name, author, last_read FROM Favorites";
                reader = cmd.ExecuteReader();

                using (StreamWriter sw = File.CreateText("mangalist.csv"))
                {
                    try
                    {

                        if (checkBox1.Checked && checkBox2.Checked)
                        {
                            sw.WriteLine(String.Format("{0};{1};{2}", "Manga name", "Author's name", "Last chapter read") + "\n");
                            while (reader.Read())
                            {
                                byte[] bytes = Encoding.Default.GetBytes(String.Format("{0};{1};{2}", reader.GetString("manga_name"), reader.GetString("author"), reader.GetString("last_read")));
                                string mangainf = Encoding.UTF8.GetString(bytes);
                                sw.WriteLine(mangainf);
                            }
                        }

                        if (checkBox1.Checked)
                        {
                            sw.WriteLine(String.Format("{0};{1}", "Manga name", "Author's name") + "\n");
                            while (reader.Read())
                            {
                                byte[] bytes = Encoding.Default.GetBytes(String.Format("{0};{1}", reader.GetString("manga_name"), reader.GetString("author")));
                                string mangainf = Encoding.UTF8.GetString(bytes);
                                sw.WriteLine(mangainf);
                            }
                        }

                        if (checkBox2.Checked)
                        {
                            sw.WriteLine(String.Format("{0};{1}", "Manga name", "Last chapter read") + "\n");
                            while (reader.Read())
                            {
                                byte[] bytes = Encoding.Default.GetBytes(String.Format("{0};{1}", reader.GetString("manga_name"), reader.GetString("last_read")));
                                string mangainf = Encoding.UTF8.GetString(bytes);
                                sw.WriteLine(mangainf);
                            }
                        }

                        if (!checkBox1.Checked && !checkBox2.Checked)
                        {

                            sw.WriteLine(String.Format("{0}", "Manga name") + "\n");
                            while (reader.Read())
                            {
                                byte[] bytes = Encoding.Default.GetBytes(String.Format("{0}", reader.GetString("manga_name")));
                                string mangainf = Encoding.UTF8.GetString(bytes);
                                sw.WriteLine(mangainf);
                            }

                        }

                    }
                    catch (Exception ex) { MessageBox.Show("Oops... contact me in reddit and explain wtf you did.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    finally
                    {
                        MessageBox.Show("It's done bro.", "Yeee!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }

            }
            catch (Exception ex) { MessageBox.Show("Oops... contact me in reddit and explain wtf you did.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error); }


            dbConnection.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            getDB();
            Decompress();
            ExtractTar();
            cpdlFiles();
        }

        void getDB()
        {

            p.StartInfo.FileName = currentDirectory + "\\adb.exe";
            p.StartInfo.Arguments = "backup -noapk com.notabasement.mangarock.android.lotus";
            p.Start();
            p.WaitForExit();

        }

        void Decompress()
        {

            p.StartInfo.FileName = "java";
            p.StartInfo.Arguments = "-jar " + currentDirectory + "\\abe.jar unpack backup.ab manga.tar";
            p.Start();
            p.WaitForExit();

        }

        void ExtractTar()
        {

            if (!Directory.Exists(currentDirectory + "\\mangatar"))
            {
                Directory.CreateDirectory(currentDirectory + "\\mangatar");
            }

            p.StartInfo.FileName = currentDirectory + "\\tar.exe";
            p.StartInfo.Arguments = "xvf manga.tar -C mangatar";
            p.Start();
            p.WaitForExit();
        }

        void cpdlFiles()
        {

            File.Copy(currentDirectory + "\\mangatar\\apps\\com.notabasement.mangarock.android.lotus\\db\\mangarock.db", currentDirectory + "\\mangarock.db");
if (Directory.Exists(currentDirectory + "\\mangatar"))
            {
                Directory.Delete(currentDirectory + "\\mangatar", true);
            }
            File.Delete(currentDirectory + "\\backup.ab");
            File.Delete(currentDirectory + "\\manga.tar");
        }

    }

}
