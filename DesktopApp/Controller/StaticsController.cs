using DesktopApp.Databases;
using DesktopApp.Structure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp.Controller
{
    public class StaticsController
    {
        private MainWindow mainWindow;
        private Timer timer;
        private int steps;
        private long elapsedMilis;
        private Boolean cheating;
        private Boolean showedCandidates;
        private Boolean showedBadValues;
        private Boolean timerEnabled;
        private int points;
        private MySqlDB db;
        private LoggedInUser user;

        

        public StaticsController(MainWindow mw)
        {
            mainWindow = mw;
            cheating = showedCandidates = showedBadValues = false;
            timerEnabled = true;
            steps = 0;
            elapsedMilis = 0;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += TimerOnTick;
            db = new MySqlDB();
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            elapsedMilis += 1000;
            var timespan = TimeSpan.FromMilliseconds(elapsedMilis);

            string formatedTime = string.Format("{0:D2} : {1:D2} : {2:D2}", timespan.Hours, timespan.Minutes, timespan.Seconds);

            mainWindow.UpdateTimerLabelContent(formatedTime);
        }

        public void GameEnded(Problem map)
        {
            switch (map.Difficulty) {
                case MapEnum.Difficulty.Easy:
                    points = 20;
                    break;

                case MapEnum.Difficulty.Medium:
                    points = 40;
                    break;

                case MapEnum.Difficulty.Hard:
                    points = 60;
                    break;

                case MapEnum.Difficulty.Expert:
                    points = 80;
                    break;
            }

            if (showedCandidates) points /= 2;

            if (cheating) points = 0;
        }

        public void CreateNewStat()
        {
            if (elapsedMilis > 0 || steps > 0)
            {
                if (db.OpenConnection())
                {
                    using (var com = new MySqlCommand("INSERT INTO solutions (UserId, Steps, Candidates, Time, Points, Cheating) VALUES(@userId, @steps, @candidates, @time, @points, @cheating)", db.Connection))
                    {
                        try
                        {
                            string cand = ShowedCandidates ? "1" : "0";
                            string cheat = Cheating ? "1" : "0";
                            com.Parameters.Add(new MySqlParameter("@userId", User.Id));
                            com.Parameters.Add(new MySqlParameter("@steps", Steps));
                            com.Parameters.Add(new MySqlParameter("@candidates", cand));
                            com.Parameters.Add(new MySqlParameter("@time", elapsedMilis));
                            com.Parameters.Add(new MySqlParameter("@points", Points));
                            com.Parameters.Add(new MySqlParameter("@cheating", cheat));
                            var reader = com.ExecuteReader();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show("Error happend: Could not insert to statics. (" + exception.Message + ")");
                        }
                        finally
                        {
                            db.CloseConnection();
                        }
                    }
                }
                GetGlobalStats();
            }

            timer.Stop();
            elapsedMilis = 0;
            mainWindow.UpdateTimerLabelContent("00 : 00 : 00");
            Steps = 0;
            Cheating = false;
        }


        public void GetGlobalStats()
        {
            string content = "";
            if (db.OpenConnection())
            {
                using (var com = new MySqlCommand("SELECT SUM(`Steps`) as SumSteps, SUM(`Candidates`) as SumCandidates, SUM(`Cheating`) as SumCheating, SUM(`Time`) as SumTime, SUM(`Points`) as SumPoints, COUNT(`UserId`) as SumGames FROM peterbartha.solutions WHERE UserId=@uid;", db.Connection))
                {
                    try
                    {
                        com.Parameters.Add(new MySqlParameter("@uid", User.Id));
                        var reader = com.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string sumSteps = reader["SumSteps"].ToString();
                                string SumCandidates = reader["SumCandidates"].ToString();
                                string SumCheating = reader["SumCheating"].ToString();
                                string SumTime = reader["SumTime"].ToString();
                                string SumPoints = reader["SumPoints"].ToString();
                                string SumGames = reader["SumGames"].ToString();

                                int value;
                                int.TryParse(SumTime, out value);

                                var timespan = TimeSpan.FromMilliseconds(value);
                                string formatedTime = string.Format("{0:D2} : {1:D2} : {2:D2}", timespan.Hours, timespan.Minutes, timespan.Seconds);

                                content = SumGames + "\r\n" + sumSteps + "\r\n" + SumCandidates + "\r\n" + formatedTime + "\r\n" + SumPoints + "\r\n" + SumCheating;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Error happend: Could not read group list. (" + exception.Message + ")");
                    }
                    finally
                    {
                        db.CloseConnection();
                    }
                }
            }
            mainWindow.SetUpStatPanelContent(content);
        }

        /**
         * Getter/Setter
         **/
        public Timer Timer
        {
            get { return timer; }
        }

        public int Steps
        {
            get { return steps; }
            set {
                steps = value;
                mainWindow.UpdateStepsCounterContent(value);
            }
        }

        public Boolean Cheating
        {
            get { return cheating; }
            set { 
                cheating = value;
                if (value)
                {
                    timer.Stop();
                    mainWindow.UpdateTimerLabelContent("00 : 00 : 00");
                }
            }
        }

        public Boolean ShowedCandidates
        {
            get { return showedCandidates; }
            set { showedCandidates = value; }
        }

        public Boolean ShowedBadValues
        {
            get { return showedBadValues; }
            set { showedBadValues = value; }
        }

        public Boolean TimerEnabled
        {
            get { return timerEnabled; }
            set { timerEnabled = value; }
        }
        public int Points
        {
            get { return points; }
            set { points = value; }
        }
        public LoggedInUser User
        {
            get { return user; }
            set { user = value; }
        }
        
    }
}
