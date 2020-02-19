using Fitnes.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GalaSoft.MvvmLight;
using Fitnes.Model;
using System.ComponentModel;
using System.Windows;

namespace Fitnes.ViewModel
{
    public class DataFromJson
    {
        public static List<UserAppData> GetData(int firstFile,int lastFile)
        {
            if (firstFile <= 0&&firstFile>29)
                firstFile = 1;
            if (lastFile <= 1&&lastFile>30)
                lastFile = 30;
            List<UserFromJson> Users = new List<UserFromJson>();
            List<UserAppData> userOuts = new List<UserAppData>();
            int _total = 0;
            int _rank = 1;
            FileInfo AppFile = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            List<string> all = Directory.GetFiles(AppFile.DirectoryName+@"/Source").ToList();
            int amount = all.Count;
            try
            {
                for (int i = firstFile; i <= lastFile; i++)
                {
                    string s = AppFile.DirectoryName + @"/Source\\" + "day" + i + ".json";

                    using (StreamReader stream = new StreamReader(s))
                    {
                        var json = JsonConvert.DeserializeObject<UserFromJson[]>(stream.ReadToEnd());
                        foreach (UserFromJson user in json.ToList())
                        {
                            if (Users.Exists(x => x.User == user.User))
                                Users.Where(x => x.User == user.User)
                                .FirstOrDefault().Steps += user.Steps;
                            else
                                Users.Add(user);
                            if (userOuts.Exists(x => x.User == user.User))
                            {
                                UserAppData userOut = userOuts.Where(x => x.User == user.User).FirstOrDefault();
                                if (user.Steps > userOut.BestResult)
                                    userOut.BestResult = user.Steps;
                                if (user.Steps < userOut.WorstResult)
                                    userOut.WorstResult = user.Steps;
                            }
                            else
                                userOuts.Add(new UserAppData { User = user.User, Average = 0, BestResult = user.Steps, WorstResult = user.Steps, Rank = 0, Status = user.Status });
                        }
                    }

                }
                Users = Users.OrderByDescending(x => x.Steps).ToList();

                foreach (UserFromJson user in Users)
                {
                    userOuts.Where(x => x.User == user.User).FirstOrDefault().Rank = _rank;
                    _rank++;
                }
                for (int i = firstFile; i <= lastFile; i++)
                {
                    string s = AppFile.DirectoryName + @"/Source\\" + "day" + i + ".json";
                    using (StreamReader stream = new StreamReader(s))
                    {
                        var json = JsonConvert.DeserializeObject<UserFromJson[]>(stream.ReadToEnd());
                        foreach (UserFromJson user in json.ToList())
                            userOuts.Where(x => x.User == user.User).FirstOrDefault().StepsPerDay.Add(user.Steps);
                    }
                }
                foreach (UserAppData userOut in userOuts)
                {
                    UserFromJson user = Users.Where(x => x.User == userOut.User).FirstOrDefault();
                    _total += user.Steps;
                    userOut.Average = user.Steps / userOut.StepsPerDay.Count;
                }
                return userOuts;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

        }
    }
}