using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
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
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer _timer = null;
        public MainWindow()
        {
            InitializeComponent();

            //_timer = new DispatcherTimer();
            //_timer.Interval = new TimeSpan(0, 0, 5);
            //_timer.Tick += _timer_Tick;
            //_timer.Start();
            Load();
        }
        private void _timer_Tick(object sender, EventArgs e)
        {
            Load();
        }
        public void Load()
        {
            //grid.ItemsSource = Process.GetProcesses();//.Select(p => new ProcessInfo(p));
            grid.ItemsSource = Process.GetProcesses();
            int[] arr = new int[] { 1, 2, 5 };
            cbIntervals.ItemsSource = arr;
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, (int)cbIntervals.SelectedItem);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }
        private void btnKill_Click(object sender, RoutedEventArgs e)
        {
            var processKill = Process.GetProcesses().Where(p => p.ProcessName == grid.SelectedItem);
            foreach (var item in processKill)
            {
                item.Kill();
            }
        }
        private void btnNewProcess_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(tbArea.Text);
        }
    }

    class ProcessInfo
    {
        public int Id { get; set; }
        public TimeSpan TotalProcessorTime { get; set; }
        public ProcessPriorityClass PriorityClass { get; set; }
        public string UserName { get; set; }
        public bool State { get; set; }

        public ProcessInfo(Process pr)
        {
            Id = pr.Id;
            try { TotalProcessorTime = pr.TotalProcessorTime; } catch { }
            try { PriorityClass = pr.PriorityClass; } catch { }
            UserName = GetProcessOwner(pr.Id);
            try { State = pr.HasExited; } catch { }
        }
        public string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[0];
                }
            }
            return "NO OWNER";
        }
    }
}
