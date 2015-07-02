using DesktopApp.Structure;
using DesktopApp.Databases;
using DesktopApp.MapEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;
namespace DesktopApp.Controller
{
    public class MapController
    {
        private MainWindow mainWindow;
        private StaticsController statCtrl;
        private TableController tableController;
        private SolverController solverCtrl;
        private List<Problem> maps;
        private Problem map;
        private MySqlDB db;


        public MapController(TableController tableCtrl, StaticsController sctrl, SolverController solver, MainWindow mw)
        {
            mainWindow = mw;
            solverCtrl = solver;
            statCtrl = sctrl;
            tableController = tableCtrl;

            maps = new List<Problem>();
            db = new MySqlDB();

            LoadMapsFromDB();

            GenerateMap(maps.ElementAt(0));
        }

        public void LoadMapsFromDB()
        {
            if (db.OpenConnection())
            {
                using (var com = new MySqlCommand("SELECT * FROM Maps", db.Connection))
                {
                    try
                    {
                        var reader = com.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var problem = new Problem();

                                problem.Id = int.Parse(reader["Id"].ToString());
                                problem.AuthorId = int.Parse(reader["AuthorId"].ToString());
                                problem.Name = reader["Name"].ToString();
                                problem.IsPublic = reader["IsPublic"].ToString() == "1";
                                problem.Map = reader["Map"].ToString();
                                problem.Box = reader["Boxes"].ToString();
                                problem.Difficulty = (Difficulty)System.Enum.Parse(typeof(Difficulty), reader["Difficulty"].ToString());

                                maps.Add(problem);
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
        }

        private List<List<Cell>> ParseMap(String map)
        {
            List<List<Cell>> table = new List<List<Cell>>();

            for (int y = 0; y < 9; y++)
            {
                List<Cell> row = new List<Cell>();
                for (int x = 0; x < 9; x++)
                {
                    int value;
                    if (int.TryParse(Convert.ToString(map[x + y * 9]), out value))
                    {
                        Cell cell = new Cell(x, y, value);
                        cell.IsDefault = (value > 0);
                        row.Add(cell);
                    }
                }
                table.Add(row);
            }
            return table;
        }

        private List<Box> ParseBox(String box)
        {
            List<Box> boxes = new List<Box>();

            for (int i = 0; i < 9; i++)
            {
                Box tempBox = new Box();
                boxes.Add(tempBox);
            }

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    int boxID;
                    if (int.TryParse(Convert.ToString(box[x + y * 9]), out boxID))
                    {
                        Cell cell = tableController.Table.Cells.ElementAt(y).ElementAt(x);
                        boxes[boxID - 1].Cells.Add(cell);
                        cell.Box = boxes[boxID - 1];
                    }
                }
            }
            return boxes;
        }

        public void GenerateMap(Problem problem)
        {
            map = problem;
            tableController.Table.Cells = ParseMap(problem.Map);
            tableController.Table.Boxes = ParseBox(problem.Box);
            tableController.RenderTable();
            tableController.SolveTableBeforeGame(solverCtrl);
            mainWindow.SetActualMapName(problem.Name);
            statCtrl.CreateNewStat();
            mainWindow.TheEnd.Visibility = Visibility.Hidden;
        }

        public Problem Map
        {
            get { return map; }
            set { map = value; }
        }
        public List<Problem> Problems
        {
            get { return maps; }
            set { maps = value; }
        }
    }
}
