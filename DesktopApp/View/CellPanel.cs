using DesktopApp.Controller;
using DesktopApp.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DesktopApp.View
{
    public class CellPanel : Panel
    {
        private TableController tableController;
        private Canvas candidateCanvas, resultCanvas, editCanvas;
        private TextBox field;
        private TextBlock number;
        private Border resultBorder;
        private List<TextBlock> candidates;
        private Cell cell;
        public readonly IList<string> backgrounds = new List<string> { "#b3d7e9", "#f4dda7", "#e4bdd4", "#b3e6dd", "#f5c0a6", "#eeeeee", "#e9adad", "#c8c8c8", "#e3efb9" }.AsReadOnly();

        public CellPanel(Cell c, TableController tableCtrl)
        {
            tableController = tableCtrl;
            cell = c;
            InitializeCellPanel();
            BuildCandidateView();   // This is the default view
            BuildResultView();
            BuildEditView();
            Refresh();
            this.MouseLeftButtonUp += new MouseButtonEventHandler(LeftClickOnPanel);
        }

        private void InitializeCellPanel() {
            this.Height = 60;
            this.Width = 60;
            BrushConverter bc = new BrushConverter();
            this.Background = (Brush)bc.ConvertFrom(backgrounds[tableController.Table.Boxes.IndexOf(cell.Box)]);
        }

        private void BuildCandidateView()
        {
            Border frame = new Border();
            frame.Width = frame.Height = 60;
            BrushConverter bc = new BrushConverter();
            frame.BorderBrush = (Brush)bc.ConvertFrom("#91aa9d");
            frame.BorderThickness = new Thickness(1, 1, 1, 1);

            candidateCanvas = new Canvas();
            candidateCanvas.Width = candidateCanvas.Height = 60;
            candidateCanvas.Margin = new Thickness(0);

            candidates = new List<TextBlock>();

            for (int i = 0; i < 9; i++)
            {
                Border border = new Border();
                border.Width = border.Height = 18;
                border.BorderBrush = null;

                Thickness margin = border.Margin;
                if (i % 3 == 0) margin.Left = 3;
                else margin.Left = 3 + (i % 3) * 18;
                if (Math.Floor(i / 3.0) == 0) margin.Top = 3;
                else margin.Top = 3 + Math.Floor(i / 3.0) * 18;
                border.Margin = margin;

                TextBlock candidate = new TextBlock();
                candidate.Text = Convert.ToString(i + 1);
                candidate.Foreground = (Brush)bc.ConvertFrom("#3e606f");
                candidate.FontFamily = new FontFamily("Freestyle Script Regular");
                candidate.FontSize = 20;
                candidate.HorizontalAlignment = HorizontalAlignment.Center;
                candidate.VerticalAlignment = VerticalAlignment.Center;
                candidate.Padding = new Thickness(0);
                candidate.Visibility = Visibility.Hidden;

                border.Child = candidate;
                candidates.Add(candidate);
                candidateCanvas.Children.Add(border);
            }

            candidateCanvas.Children.Add(frame);
            candidateCanvas.Visibility = Visibility.Hidden;
            this.Children.Add(candidateCanvas);
        }

        private void BuildResultView()
        {
            resultCanvas = new Canvas();
            resultCanvas.Width = resultCanvas.Height = 60;
            resultCanvas.Margin = new Thickness(-60, 0, 0, 0);

            resultBorder = new Border();
            resultBorder.Width = resultBorder.Height = 60;
            BrushConverter bc = new BrushConverter();
            resultBorder.BorderBrush = (Brush)bc.ConvertFrom("#91aa9d");
            resultBorder.BorderThickness = new Thickness(1, 1, 1, 1);

            number = new TextBlock();
            number.Text = cell.Value > 0 && cell.Value < 10 ? Convert.ToString(cell.Value) : "";
            number.Foreground = cell.IsDefault ? (Brush)bc.ConvertFrom("#2e4853") : (Brush)bc.ConvertFrom("#3e606f");
            number.FontFamily = cell.IsDefault ? new FontFamily("Calibri") : new FontFamily("Freestyle Script Regular");
            number.FontSize = cell.IsDefault ? 32 : 40;
            number.HorizontalAlignment = HorizontalAlignment.Center;
            number.VerticalAlignment = VerticalAlignment.Center;
            number.Padding = cell.IsDefault ? new Thickness(0) : new Thickness(0, 2, 0, 0);

            resultBorder.Child = number;
            resultCanvas.Children.Add(resultBorder);
            resultCanvas.Visibility = Visibility.Hidden;
            this.Children.Add(resultCanvas);
        }

        private void BuildEditView()
        {
            editCanvas = new Canvas();
            editCanvas.Width = resultCanvas.Height = 60;
            editCanvas.Margin = new Thickness(-60, 0, 0, 0);

            Border border = new Border();
            border.Width = border.Height = 60;
            BrushConverter bc = new BrushConverter();
            border.BorderBrush = (Brush)bc.ConvertFrom("#91aa9d");
            border.BorderThickness = new Thickness(1, 1, 1, 1);

            field = new TextBox();
            field.KeyDown += new KeyEventHandler(OnKeyDown);
            field.LostFocus += new RoutedEventHandler(FieldLostFocus);
            field.Text = cell.Value > 0 ? Convert.ToString(cell.Value) : "";
            field.Background = field.BorderBrush = null;
            field.BorderThickness = new Thickness(0);
            field.Foreground = (Brush)bc.ConvertFrom("#3e606f");
            field.FontFamily = new FontFamily("Freestyle Script Regular");
            field.FontSize = 40;
            field.HorizontalAlignment = HorizontalAlignment.Center;
            field.VerticalAlignment = VerticalAlignment.Center;
            field.Padding = new Thickness(0, 2, 0, 0);

            border.Child = field;
            editCanvas.Children.Add(border);
            editCanvas.Visibility = Visibility.Hidden;
            this.Children.Add(editCanvas);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape || e.Key == Key.Space)
            {
                if (!cell.IsDefault) FieldChanged();
            }
        }

        private void LeftClickOnPanel(object sender, MouseButtonEventArgs e)
        {
            if (!cell.IsDefault)
            {
                resultCanvas.Visibility = Visibility.Hidden;
                editCanvas.Visibility = Visibility.Visible;
                field.Focus();
                field.SelectAll();
                candidateCanvas.Visibility = Visibility.Hidden;
            }
        }

        private void FieldLostFocus(object sender, RoutedEventArgs e)
        {
            if (!cell.IsDefault) FieldChanged();
        }

        private void FieldChanged()
        {
            if (!tableController.Stats.Cheating && !tableController.Stats.Timer.Enabled && tableController.Stats.TimerEnabled)
                tableController.Stats.Timer.Start();

            if (!tableController.Stats.Cheating) tableController.Stats.Steps++;

            int value;
            if (int.TryParse(field.Text, out value))
            {
                EditCellValue(value);
                editCanvas.Visibility = Visibility.Hidden;
                Refresh();
            }
            else if (field.Text == "")
            {
                EditCellValue(0);
                number.Text = Convert.ToString("");
                //if (cell.Value > 0 && cell.Value <= 9) EditCellValue(cell.Value);
                //else EditCellValue(0);
                editCanvas.Visibility = Visibility.Hidden;
                Refresh();
            }
            else
            {
                string text = cell.Value == 0 ? "" : cell.Value.ToString();
                field.Text = Convert.ToString(text);
                editCanvas.Visibility = Visibility.Hidden;
                Refresh();
            }
        }

        private void EditCellValue(int value)
        {
            if (value >= 0 && value <= 9)
            {
                cell.Value = value;
                number.Text = value > 0 ? Convert.ToString(cell.Value) : "";
                tableController.MakeCandidatesForTableCells();

                if (tableController.CountEmptyCells() == 0) tableController.CheckGameState();
                CheckValue();
            }
        }

        public void ChangeCellValue(int value)
        {
            cell.Value = value;
            number.Text = Convert.ToString(cell.Value);
        }

        private void CheckCandidates()
        {
            HideAllCandidates();
            foreach (var candidate in cell.Candidates)
            {
                candidates[candidate - 1].Visibility = Visibility.Visible;
            }
            //if (candidates.Count == 0) number.Text = Convert.ToString(cell.Value);
        }

        public void CheckValue()
        {  
            var bc = new BrushConverter();

            if (!CheckCellValidity() && tableController.ShowBadValues)
            {
                resultBorder.BorderBrush = Brushes.Red;
            }
            else
            {
                resultBorder.BorderBrush = (Brush)bc.ConvertFrom("#91aa9d");
            }
        }

        private Boolean CheckCellValidity()
        {
            return (cell.Value == tableController.SolvedTable.Cells.ElementAt(cell.Y).ElementAt(cell.X).Value || cell.Value == 0);
        }

        private void HideAllCandidates()
        {
            foreach (var candidate in candidates)
            {
                candidate.Visibility = Visibility.Hidden;
            }
        }

        public void SetCellBackgroundColor(Brush brush)
        {
            candidateCanvas.Background = resultCanvas.Background = brush;
        }

        private void SwitchViewToCandidates()
        {
            if (candidateCanvas.Visibility == Visibility.Hidden)
            {
                candidateCanvas.Visibility = Visibility.Visible;
                resultCanvas.Visibility = Visibility.Hidden;
            }
        }

        private void SwitchViewToResult()
        {
            if (resultCanvas.Visibility == Visibility.Hidden)
            {
                resultCanvas.Visibility = Visibility.Visible;
                candidateCanvas.Visibility = Visibility.Hidden;
            }
        }

        public void Refresh()
        {
            if (cell.Value > 0) SwitchViewToResult();
            else
            {
                if (tableController.ShowCandidates) SwitchViewToCandidates();
                else SwitchViewToResult();
                CheckCandidates();
            }
        }

        public void RefreshView()
        {
            if (cell.Value > 0) SwitchViewToResult();
            else
            {
                if (tableController.ShowCandidates) SwitchViewToCandidates();
                else
                {
                    resultCanvas.Visibility = Visibility.Visible;
                    candidateCanvas.Visibility = Visibility.Hidden;
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double sumX = 0.0;
            double maxY = 0.0;
            foreach (UIElement child in this.Children)
            {
                child.Measure(new Size(Math.Max(availableSize.Width - sumX, 0.0), availableSize.Height));
                sumX += child.DesiredSize.Width;
                maxY = Math.Max(maxY, child.DesiredSize.Height);
            }
            return new Size(sumX, maxY);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0.0;
            for (int i = 0; i < this.Children.Count - 1; i++)
            {
                UIElement child = this.Children[i];
                child.Arrange(new Rect(x, 0.0, child.DesiredSize.Width, child.DesiredSize.Height));
                x += child.DesiredSize.Width;
            }
            if (this.Children.Count > 0)
            {
                UIElement lastChild = this.Children[this.Children.Count - 1];
                lastChild.Arrange(new Rect(x, 0.0, Math.Max(finalSize.Width - x, 0.0), lastChild.DesiredSize.Height));
            }
            return finalSize;
        }
    }
}
