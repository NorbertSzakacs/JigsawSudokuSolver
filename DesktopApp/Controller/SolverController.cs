using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopApp.Heuristics;
using DesktopApp.Structure;

namespace DesktopApp.Controller
{
    public class SolverController
    {
        private List<Heuristic> heuristics;
        private int actualId;
        private Table table;
        private MainWindow window;

        public SolverController(Table t, MainWindow w)
        {
            this.table = t;
            this.window = w;
            InitializeHeuristics();
            Actual = 0;
        }

        private void InitializeHeuristics() {
            heuristics = new List<Heuristic>();
            heuristics.Add(new NakedSingle(table));
            heuristics.Add(new HiddenSingle(table));
            heuristics.Add(new NakedPair(table));
            heuristics.Add(new HiddenPair(table));
            heuristics.Add(new PointingPair(table));
            heuristics.Add(new BoxLineReduction(table));
            heuristics.Add(new XWing(table));
            heuristics.Add(new RandomPick(table));
        }

        public void TakeOneStep()
        {
            if (heuristics.ElementAt(Actual).Apply())
            {
                if (window.CheckGameState(true) != MainWindow.GameState.InGame) return;
                Actual = 0;
                return;
            }

            for (int i = Actual + 1; i < heuristics.Count; i++)
            {
                if (heuristics[i].Enabled)
                {
                    Actual = i;
                    return;
                }
            }
            Actual = 0;
        }

        public void AutoSolve()
        {
            int steps = 0;
            foreach (var heuristic in heuristics)
            {
                Actual = heuristics.IndexOf(heuristic);
                if (heuristic.Apply())
                {
                    if (window.CheckGameState(true) != MainWindow.GameState.InGame) return;
                    Actual = 0;
                    AutoSolve();
                    break;
                }
            }
        }

        public void GetSolvedMap()
        {
            foreach (var heuristic in heuristics)
            {
                Actual = heuristics.IndexOf(heuristic);
                if (heuristic.Apply())
                {
                    if (window.CheckGameState(false) != MainWindow.GameState.InGame) return;
                    GetSolvedMap();
                    break;
                }
            }
        }

        public int Actual
        {
            get { return actualId; }
            set { actualId = value; }
        }

        public List<Heuristic> Heuristics
        {
            get { return heuristics; }
        }
    }
}
