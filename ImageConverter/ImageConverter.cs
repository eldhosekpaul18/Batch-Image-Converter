using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageConverter
{
    public partial class ImageConverter : Form
    {
        const string FILE_TYPES = ".bmp,.jpg,.jpeg,.png,.icon";
        public enum Format
        {
            Jpeg,
            Png,
            Bmp,
            Emf,
            Wmf,
            Gif,
            Tiff,
            Exif,
            Icon
        }
        private int _sucessCount = 0;
        public ImageConverter()
        {
            InitializeComponent();
            cmbOutputFormat.DataSource = Enum.GetValues(typeof(Format));
        }

        private void btnBrowseInput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtInputFolder.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtOutputFolder.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtInputFolder.Text))
            {
                MessageBox.Show("Please select input folder");
                txtInputFolder.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtOutputFolder.Text))
            {
                MessageBox.Show("Please select output folder");
                txtOutputFolder.Focus();
                return;
            }

            SearchOption searchOption = chkSearchSubDirectory.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var filePaths = Directory.GetFiles(txtInputFolder.Text, "*", searchOption)
                .Where(file => FILE_TYPES.Split(',').Any(file.ToLower().EndsWith)).ToList();

            if (filePaths.Count() == 0)
            {
                MessageBox.Show("No images found in the directory.");
                return;
            }
            _sucessCount = 0;
            foreach (var filepath in filePaths)
            {
                ConvertImage(filepath);
            }
            
            MessageBox.Show(string.Format("Converted {0} out of {1} images",_sucessCount,filePaths.Count()));
        }

        private void ConvertImage(string filepath)
        {
            try
            {
                using (Bitmap bmp = new Bitmap(filepath))
                {
                    ImageFormat format = GetImageFormat();
                    string path = CreateDirIfNotExist(filepath);
                    var filename = Path.GetFileName(filepath);
                    var outfilename = Path.Combine(path, Path.GetFileNameWithoutExtension(filepath)) + "." + format.ToString();

                    if (File.Exists(outfilename))
                    {
                        outfilename = Path.Combine(path, Path.GetFileNameWithoutExtension(filepath)) + "_" + DateTime.Now.Ticks.ToString() + "." + format.ToString();
                    }

                    bmp.Save(outfilename, format);
                    _sucessCount++;
                    txtOutPut.AppendText(string.Format("Converted {0} to {1}", filename, outfilename));
                    txtOutPut.AppendText(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                txtOutPut.AppendText(string.Format("Error converting {0} : {1}", filepath, ex.Message));
                txtOutPut.AppendText(Environment.NewLine);
            }
        }

        private string CreateDirIfNotExist(string filePath)
        {
            string dirPath = txtOutputFolder.Text;
           
            if (chkKeepStruct.Checked)
            {
                filePath = filePath.Replace(txtInputFolder.Text, txtOutputFolder.Text);
                dirPath = Directory.GetParent(filePath).FullName;
            }
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        public ImageFormat GetImageFormat()
        {
            Format format;
            Enum.TryParse<Format>(cmbOutputFormat.SelectedValue.ToString(), out format);

            if (format.Equals(Format.Jpeg))
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            if (format.Equals(Format.Bmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (format.Equals(Format.Png))
                return System.Drawing.Imaging.ImageFormat.Png;
            if (format.Equals(Format.Emf))
                return System.Drawing.Imaging.ImageFormat.Emf;
            if (format.Equals(Format.Exif))
                return System.Drawing.Imaging.ImageFormat.Exif;
            if (format.Equals(Format.Gif))
                return System.Drawing.Imaging.ImageFormat.Gif;
            if (format.Equals(Format.Icon))
                return System.Drawing.Imaging.ImageFormat.Icon;
            if (format.Equals(Format.Tiff))
                return System.Drawing.Imaging.ImageFormat.Tiff;
            else
                return System.Drawing.Imaging.ImageFormat.Wmf;
        }
    }

}
