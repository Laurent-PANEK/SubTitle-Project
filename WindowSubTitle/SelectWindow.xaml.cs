using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowSubTitle
{
    /// <summary>
    /// Logique d'interaction pour SelectFile.xaml
    /// </summary>
    public partial class SelectWindow : Window
    {
        public string pathFile;
        public string pathSubtitle;
        public SelectWindow(string pathF, string pathS)
        {
            InitializeComponent();
            pathFile = pathF;
            pathSubtitle = pathS;
        }

        private void Select_file_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".mp4"; // Default file extension
            dlg.Filter = "Fichier MP4 (.mp4)|*.mp4|Fichier MKV (.mkv)|*.mkv|Fichier AVI (.avi)|*.avi|Fichier WMV (.wmv)|*.wmv|Fichier FLV (.flv)|*.flv"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                pathFile = dlg.FileName;
                PathVideoFile.Text = pathFile;
            }
        }

        private void Select_subtitle_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".srt"; // Default file extension
            dlg.Filter = "Fichier Srt (.srt)|*.srt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                pathSubtitle = dlg.FileName;
                PathSubtitleFile.Text = pathSubtitle;
            }
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidPath(pathFile) && IsValidPath(pathSubtitle))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Les medias choisit ne sont pas valide !", "Erreur Fichier", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidPath(string path)
        {
            Regex r = new Regex(@"[A-Z]:(\\.+)*\\(.)+\.(.){3}");

            if (path != null)
            {
                if (r.IsMatch(path))
                {
                    if (File.Exists(path))
                        return true;
                }
            }
            return false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidPath(pathFile) && IsValidPath(pathSubtitle))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Les medias choisit ne sont pas valide !", "Erreur Fichier", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
