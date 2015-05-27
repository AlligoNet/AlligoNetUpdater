using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace AlligoUpdater
{
    public partial class Form1 : Form
    {
        private string downloadServer = "REDACTED";
        private string softetherInstallPath = "REDACTED";

        public Form1(string[] args)
        {
            InitializeComponent();

            /* Kill Main Client */
            Process[] activeProcesses = Process.GetProcesses();
            foreach (Process currProc in activeProcesses)
            {
                string procName = currProc.ProcessName.ToLower();
                if (procName.CompareTo("AlligoClient") == 0)
                {
                    currProc.Kill();
                    break;
                }
            }

            if(args.Length > 0)
            {
                if(args[0].Contains("install"))
                {
                    InstallSoftEther();
                }
            }
            else
            {
                UpdateClient();
            }

        }

        private void InstallSoftEther()
        {
            label1.Text = "Downloading Required Software (SoftEther VPN)";
            //Download Installer
            WebClient downloader = new WebClient();

            downloader.DownloadFileCompleted += SoftEtherDownloaded;
            downloader.DownloadProgressChanged += SoftEtherProgressChanged;
            downloader.DownloadFileAsync(new System.Uri(softetherInstallPath), "SoftEtherInstall.exe");
        }

        private void UpdateClient()
        {
            label1.Text = "Downloading Alligo Client Update";
            //Remove Previous Backup
            if( File.Exists("AlligoClient_old.exe"))
            {
                File.Delete("AlligoClient_old.exe");
            }

            //Create new backup
            File.Move("AlligoClient.exe", "AlligoClient_old.exe");

            //Download the update
            WebClient downloader = new WebClient();

            downloader.DownloadFileCompleted += UpdateDownloaded;
            downloader.DownloadProgressChanged += UpdateProgressChanged;
            downloader.DownloadFileAsync(new System.Uri(downloadServer), "AlligoClient.exe");
        }

        // Runs as file is downloading
        void SoftEtherProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        // Runs when file download is complete
        private void SoftEtherDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            //Run Installer
            Process.Start("SoftEtherInstall.exe");

            // Kill Self
            Application.Exit();
        }

        // Runs as file is downloading
        void UpdateProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        // Runs when file download is complete
        private void UpdateDownloaded(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Update Completed!");

            // Reopen Client
            Process.Start("AlligoClient.exe");

            // Kill Self
            Application.Exit();
        }
    }
}
