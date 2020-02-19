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
    public class MainViewModel
    {
        private List<UserAppData> users;
        public List<UserAppData> Users { get { return users; } 
            set
            {
                users = value;
                OnPropertyChanged("Users");
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
                          
                          if (_chosenUser == null)
                              System.Windows.MessageBox.Show("Не выбрано ни одно поле!");
                          else
                          {
                              UserOutJson user = new UserOutJson {User=_chosenUser.User, Average = _chosenUser.Average, Rank = _chosenUser.Rank, Status = _chosenUser.Status, BestResult = _chosenUser.BestResult, WorstResult = _chosenUser.WorstResult}; 
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
        public ObservableDataSource<Point> UserTable { get; set; }
        private UserAppData _chosenUser;
        public UserAppData ChosenUser
        {
            get { return _chosenUser; }
            set
            {
                _chosenUser = value;
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
            Users = DataFromJson.GetData(1,30);//Изменяет отображение данных по дням(первый день и последний)
            UserTable = new ObservableDataSource<Point>();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}