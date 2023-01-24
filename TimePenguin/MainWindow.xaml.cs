using System;
using System.Timers;
using System.Windows;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace TimePenguin
{
    public partial class MainWindow : Window
    {
        #region Variables
        public double timeric = 0;

        public string description = "";

        public System.Timers.Timer timer1;

        public System.Timers.Timer timer2;

        public System.Timers.Timer timer3;

        KeyboardHook hook = new KeyboardHook();
        KeyboardHook hook1 = new KeyboardHook();
        KeyboardHook hook2 = new KeyboardHook();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            #region Warmup
            Timerr_Copy.Visibility = Visibility.Hidden;
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(ModifierKeys.Control, Keys.F6);
            hook1.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook1_KeyPressed);
            hook1.RegisterHotKey(ModifierKeys.Control, Keys.F7);
            hook2.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook2_KeyPressed);
            hook2.RegisterHotKey(ModifierKeys.Control, Keys.F8);
            double width = SystemParameters.PrimaryScreenWidth;
            this.Top = 0;
            this.Left = width - 200;
            System.Timers.Timer timer = new System.Timers.Timer(3000);
            timer.AutoReset = true;
            timer.Elapsed += timer_elapsed;
            timer.Start();
            this.ShowInTaskbar = false;
            #endregion

            #region Hide from ALT+TAB
            Window w = new Window();
            w.Title = "TimePenguin";
            w.Name = "TimePenguin";
            w.Top = -100;
            w.Left = -100;
            w.Width = 1;
            w.Height = 1;

            w.WindowStyle = WindowStyle.ToolWindow; 
            w.ShowInTaskbar = false;
            w.Show();
            this.Owner = w;
            w.Hide();
            #endregion
        }

        #region KeyPress
        private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            try
            {
                string output = ShowDialog("Укажите время (в минутах) до события. (0-999)", "TimePenguin");
                string output2 = ShowDialog("Укажите описание события. (20 символов)", "TimePenguin");
                if (output2.Length > 20 || output2.Length < 1)
                {
                    System.Windows.MessageBox.Show("Текст должен быть от 1 до 20 символов.", "TimePenguin", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                description = output2;
                if (Convert.ToInt64(output) > 0 && Convert.ToInt64(output) < 999)
                {
                    System.Timers.Timer timer = new System.Timers.Timer(60000);
                    timer2 = timer;
                    timeric = Convert.ToInt64(output);
                    Timerr_Copy.Visibility = Visibility.Visible;
                    Dispatcher.Invoke(() => Timerr_Copy.Text = description + " через " + timeric + " минут(у, ы)");
                    timeric = timeric - 1;
                    timer.AutoReset = true;
                    timer.Elapsed += timer_elapsed2;
                    timer.Start();
                }
                else
                {
                    System.Windows.MessageBox.Show("Невозможно создать событие т.к. число меньше нуля или больше 999.", "TimePenguin", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Получена ошибка, скорее всего вы задали строку вместо числа когда выбирали время.", "TimePenguin", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private void hook1_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            description = "";
            Timerr_Copy.Visibility = Visibility.Hidden;
            timeric = 0;
        }
        public void hook2_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            Environment.Exit(1);
        }
        #endregion

        #region Timers
        private void timer_elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => this.IsTabStop = true);
            Process[] processes = Process.GetProcesses();
            string prcname = "";
            string prc2name = "";
            foreach (Process clsProcess in processes)
            {
                if (GetForegroundWindow() == clsProcess.MainWindowHandle)
                {
                    prcname = clsProcess.MainWindowTitle;
                    prc2name = clsProcess.Id.ToString();
                }
            }
            if (prcname == "ARK: Survival Evolved")
            {
                if (description != "")
                {
                    Dispatcher.Invoke(() => Timerr_Copy.Visibility = Visibility.Visible);
                }
                Dispatcher.Invoke(() => Timerr.Visibility = Visibility.Visible);
                Dispatcher.Invoke(() => Timerr.Text = DateTime.Now.ToShortTimeString().ToString());
            }
            else
            {
                Dispatcher.Invoke(() => Timerr.Visibility = Visibility.Hidden);
                Dispatcher.Invoke(() => Timerr_Copy.Visibility = Visibility.Hidden);
            }
        }
        private void timer_elapsed2(object sender, ElapsedEventArgs e)
        {
            if (timeric == 0)
            {
                timer2.Stop();
                Dispatcher.Invoke(() => Timerr_Copy.Text = description + " таймер истёк!");
                System.Timers.Timer timer = new System.Timers.Timer(30000);
                timer3 = timer;
                timer.AutoReset = false;
                timer.Elapsed += timer_elapsed3;
                timer.Start();
                return;
            }
            Dispatcher.Invoke(() => Timerr_Copy.Text = description + " через " + timeric + " минут(у, ы)");
            timeric = timeric - 1;
        }

        private void timer_elapsed3(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => description = "");
            Dispatcher.Invoke(() => Timerr_Copy.Visibility = Visibility.Hidden);
            timer3.Stop();
            return;
        }
        #endregion

        #region Functions
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        public static string ShowDialog(string text, string caption)
        {
            System.Windows.Forms.Form prompt = new System.Windows.Forms.Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            };
            System.Windows.Forms.Label textLabel = new System.Windows.Forms.Label() { Left = 50, Top = 20, Text = text, Width = 400 };
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 50, Top = 50, Width = 400 };
            System.Windows.Forms.Button confirmation = new System.Windows.Forms.Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = System.Windows.Forms.DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == System.Windows.Forms.DialogResult.OK ? textBox.Text : "";
        }
        #endregion
    }
}
