using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SubTitleProject;
using System.Threading;
using System.ComponentModel;

namespace WindowSubTitle
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SubtitleFile f;
        string pathSub;
        string pathFile;
        SelectWindow s;
        BackgroundWorker workerSub;
        ManualResetEvent pause = new ManualResetEvent(true);

        public MainWindow()
        {
            InitializeComponent();
            f = new SubtitleFile();
            s = new SelectWindow(pathFile, pathSub);
            controlPanel.Opacity = 0.1;
            media.LoadedBehavior = MediaState.Stop;
            media.Volume = (double)volumeSlider.Value;
            text.Foreground = Brushes.White;
            text.FontSize = 36;
            text.TextAlignment = TextAlignment.Center;
            text.TextWrapping = TextWrapping.WrapWithOverflow;
            s.ShowDialog();
            if (pathSub != s.pathSubtitle && s.pathSubtitle != null)
                pathSub = s.pathSubtitle;
            if (pathFile != s.pathFile && s.pathFile != null)
                pathFile = s.pathFile;

            media.Source = new Uri(pathFile);
            f.ReadFile(pathSub);
        }

        private void InitializeBackgroundWorker()
        {
            workerSub.WorkerReportsProgress = true;
            workerSub.WorkerSupportsCancellation = true;

            workerSub.DoWork += new DoWorkEventHandler(workerSub_DoWork);
            workerSub.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerSub_RunWorkerCompleted);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            media.LoadedBehavior = MediaState.Play;
            pause.Set();
            workerSub = new BackgroundWorker();
            InitializeBackgroundWorker();

            if (workerSub.IsBusy != true)
            {
                workerSub.RunWorkerAsync();
            }

        }


        /*TODO Enlever le décalage lors de la pause : 
          -Créer un second background worker (bw2) qui tourne indefiniment en attendant que le premier background worker (bw1)
         soit en pause (ManualResetEvent = reset)
         -Lorsque bw1 est en pause, bw2 doit lancer un timer pour connaitre le temps de la pause
         -A la fin de la pause, bw2 doit renvoyer la nouvelle liste de tache à bw1 ayant pris en 
         compte le timer de pause pour que bw1 affiche les bons sous-titre

            Refactoriser :
            - fct AddSubTitle et RemoveSubTitle pour avoir en parametre un timer
            - bw1 pour qu'il puissent utiliser le retour de bw2, qu'il supprime les sous-titre qui ont déjà été
            affiché et supprime les ancienne tache en attente (peut etre dans bw2)
        
         */

        private void workerSub_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int i = 1;
            List<Task<string>> allT = new List<Task<string>>();
            // Parti de code que bw2 doit renvoyer en foncion du timer de la pause
            foreach (SubTitle sub in f.allSubTitle)
            {
                pause.WaitOne(Timeout.Infinite);
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    Task<string> tAdd = AddSubTitleInUI(sub);
                    Task<string> tRm = RemoveSubTitleInUI(sub);
                    allT.Add(tAdd);
                    allT.Add(tRm);
                    i++;
                }
            }
            var dine = Task.WhenAll(allT);

            while (!dine.IsCompleted)
            {
                pause.WaitOne(Timeout.Infinite);

                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    foreach (Task<string> t in allT)
                    {
                        pause.WaitOne(Timeout.Infinite);
                        if ((worker.CancellationPending == true))
                        {
                            e.Cancel = true;
                            break;
                        }
                        else
                        {
                            pause.WaitOne(Timeout.Infinite);
                            t.Wait();
                            text.Dispatcher.Invoke(
                                System.Windows.Threading.DispatcherPriority.Normal,
                                new Action(
                                delegate ()
                                {
                                    text.Text = t.Result;
                                }
                                ));
                        }
                    }
                }


            }
        }

        private void workerSub_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
               
            }

            else if (!(e.Error == null))
            {
                MessageBox.Show("Error: " + e.Error.Message);
            }

            else
            {

            }
        }




        public async Task<string> AddSubTitleInUI(SubTitle sub)
        {
            await sub.TimeToAdd();
            return ValidLenght(sub.subtitle);

        }

        public async Task<string> RemoveSubTitleInUI(SubTitle sub)
        {
            await sub.TimeToRemove();
            return "";

        }

        public string ValidLenght(string s)
        {
            string[] split = s.Split(' ');

            if (split.Length > 8)
            {
                string new_string = "";

                double returnLine = Math.Ceiling((double)split.Length / 2);

                for (int i = 0; i < split.Length; i++)
                {
                    if ((int)returnLine - 1 == i)
                    {
                        new_string += "\n";
                    }
                    new_string += split[i] + " ";
                }

                return new_string;
            }

            return s;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            media.LoadedBehavior = MediaState.Stop;
            pause.Reset();
            if (workerSub.WorkerSupportsCancellation == true)
            {
                workerSub.CancelAsync();
            }

        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            media.LoadedBehavior = MediaState.Pause;
            pause.Reset();
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            media.Volume = (double)volumeSlider.Value;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            controlPanel.Opacity = 0.1;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            controlPanel.Opacity = 1;
        }
    }
}
