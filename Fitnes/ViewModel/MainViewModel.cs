using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using Fitnes.Model;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace Fitnes.ViewModel
{
    public class MainViewModel:INotifyPropertyChanged
    {
        private List<UserAppData> users;
        public List<UserAppData> Users { get { return users; } 
            set
            {
                users = value;
                OnPropertyChanged("Users");
            }
        }
        private int firstDay;
        public int FirstDay
        {
            get { return firstDay; }
            set
            {
                firstDay = value;
                OnPropertyChanged("FirstDay");
            }
        }
        private int lastDay;
        public int LastDay
        {
            get { return lastDay; }
            set
            {
                lastDay = value;
                OnPropertyChanged("LastDay");
            }
        }
        private RelayCommand createJSON;
        public RelayCommand CreateJSON
        {
            get
            {
                return createJSON ??
                    (createJSON = new RelayCommand(obj =>
                      {
                          
                          if (chosenUser == null)
                              System.Windows.MessageBox.Show("Не выбрано ни одно поле!");
                          else
                          {
                              UserOutJson user = new UserOutJson {User=chosenUser.User, Average = chosenUser.Average, Rank = chosenUser.Rank, Status = chosenUser.Status, BestResult = chosenUser.BestResult, WorstResult = chosenUser.WorstResult}; 
                              string result = JsonConvert.SerializeObject(user);
                              FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

                              DialogResult dialogResult = folderBrowser.ShowDialog();

                              if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                              {
                                  File.WriteAllText(folderBrowser.SelectedPath + @"\myJson.json", result);
                                  System.Windows.MessageBox.Show("Файл успешно записан.");
                              }
                          }
                      }));
            }
        }
        private RelayCommand showSome;
        public RelayCommand ShowSome
        {
            get
            {
                return showSome ??
                    (showSome = new RelayCommand(obj =>
                      {
                          Users = DataFromJson.GetData(firstDay, lastDay);
                      }));
            }
        }
        public ObservableDataSource<Point> UserTable { get; set; }
        private UserAppData chosenUser;
        public UserAppData ChosenUser
        {
            get { return chosenUser; }
            set
            {
                chosenUser = value;
                UserTable.Collection.Clear();
                int g = 0;
                foreach (int i in ChosenUser.StepsPerDay)
                {
                    g++;
                    UserTable.Collection.Add(new Point(g, i));
                }
            }
        }
        public MainViewModel()
        {
            Users = DataFromJson.GetData(1,30);
            UserTable = new ObservableDataSource<Point>();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}