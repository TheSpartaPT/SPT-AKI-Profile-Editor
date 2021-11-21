﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SPT_AKI_Profile_Editor.Core
{
    public class BackupService : INotifyPropertyChanged
    {
        public BackupService()
        {
            if (!Directory.Exists(backupFolder))
            {
                DirectoryInfo dir = new(backupFolder);
                dir.Create();
            }
        }

        public List<BackupFile> BackupList
        {
            get => backupList;
            set
            {
                backupList = value;
                OnPropertyChanged("BackupList");
            }
        }

        private List<BackupFile> backupList;
        private static readonly string backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

        public void LoadBackupsList(string profile = null)
        {
            if (string.IsNullOrEmpty(profile) && !string.IsNullOrEmpty(AppData.AppSettings.DefaultProfile))
                profile = Path.GetFileNameWithoutExtension(AppData.AppSettings.DefaultProfile);
            List<BackupFile>  backups = new ();
            if (!string.IsNullOrEmpty(profile) && Directory.Exists(Path.Combine(backupFolder, profile)))
            {
                foreach (var bk in Directory.GetFiles(Path.Combine(backupFolder, profile)).Where(x => x.Contains("backup")))
                {
                    try
                    {
                        backups.Add(new BackupFile
                        {
                            Path = bk,
                            Date = DateTime.ParseExact(Path.GetFileNameWithoutExtension(bk).Remove(0, profile.Length + 8),
                            "dd-MM-yyyy-HH-mm-ss",
                            CultureInfo.InvariantCulture, DateTimeStyles.None)
                        });
                    }
                    catch (Exception ex) { Logger.Log($"Backup file ({bk}) loading error: {ex.Message}"); }
                }
            }
            BackupList = backups.OrderByDescending(x => x.Date).ToList();
        }
        public void CreateBackup(string sourcePath = null)
        {
            if (string.IsNullOrEmpty(sourcePath))
                sourcePath = Path.Combine(AppData.AppSettings.ServerPath, AppData.AppSettings.DirsList["dir_profiles"], AppData.AppSettings.DefaultProfile);
            string destFolder = Path.Combine(backupFolder,
                Path.GetFileNameWithoutExtension(sourcePath));
            if (!Directory.Exists(destFolder))
            {
                DirectoryInfo dir = new(destFolder);
                dir.Create();
            }
            string destPath = Path.Combine(destFolder, $"{Path.GetFileNameWithoutExtension(AppData.AppSettings.DefaultProfile)}-backup-{DateTime.Now:dd-MM-yyyy-HH-mm-ss}.json");
            File.Copy(sourcePath, destPath, true);
        }

        public void RestoreBackup(BackupFile file, string destPath = null)
        {
            if (string.IsNullOrEmpty(destPath))
                destPath = Path.Combine(AppData.AppSettings.ServerPath, AppData.AppSettings.DirsList["dir_profiles"], AppData.AppSettings.DefaultProfile);
            File.Copy(file.Path, destPath, true);
            File.Delete(file.Path);
        }

        public void RemoveBackup(BackupFile file)
        {
            File.Delete(file.Path);
            LoadBackupsList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}