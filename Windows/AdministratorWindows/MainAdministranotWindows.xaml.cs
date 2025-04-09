using Mail_LIB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MedLab.Windows.AdministratorWindows
{
    /// <summary>
    /// Логика взаимодействия для MainAdministranotWindows.xaml
    /// </summary>
    public partial class MainAdministranotWindows : Window
    {
        ASEntities As = new ASEntities();
        MainWindow _mainWindow;
        History _newHistory; 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="newHistory"></param>
        /// <param name="users_"></param>
        public MainAdministranotWindows(MainWindow main, History newHistory, User users_)
        {
            InitializeComponent();
            try
            {
                _mainWindow = main;
                _newHistory = newHistory;
                FIOUser.Text = $"{users_.Surname} {users_.Name} {users_.Patronymic}";
                List<string> searchByHistory = new List<string>() 
                {
                    "Логин",
                    "IP-Адрес",
                    "Дата",
                    "Причина выхода"
                };
                foreach (string search in searchByHistory)
                {
                    HistorySearchBy.Items.Add(search);
                }
                List<string> fullSearch = new List<string>()
                {
                    "Фамилия",
                    "Имя",
                    "Отчество",
                    "Услуга"
                };
                foreach (var item in fullSearch)
                {
                    FullSearchBy.Items.Add(item);
                }

                HistoryList.ItemsSource = As.Histories.ToList();

                foreach (var item in As.UserTypes.ToList())
                {
                    Specialization.Items.Add(item.Type);
                }

                FullList.ItemsSource = As.OrderAndServices.Where(x => x.IdStatus == 3).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private int selectedSpec = -1;
        private string historySearchBy;
        public bool checkClosed = false;
        private void HistorySearchText_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(historySearchBy))
            {
                HistoryList.ItemsSource = null;
                switch (historySearchBy)
                {
                    case "Логин":
                        HistoryList.ItemsSource = As.Histories.Where(x => x.User.Login.StartsWith(HistorySearchText.Text)).ToList();
                        break;
                    case "IP-Адрес": 
                        HistoryList.ItemsSource = As.Histories.Where(x => x.Ip.StartsWith(HistorySearchText.Text)).ToList(); 
                        break;
                    case "Причина выхода":
                        HistoryList.ItemsSource = As.Histories.Where(x => x.ReasonsForExit.Reason.StartsWith(HistorySearchText.Text)).ToList();
                        break;
                }

            }
        }

        public void NewHistory(int reason)
        {
            _newHistory.DateEnd = DateTime.Now;
            _newHistory.IdReason = reason;
            As.Histories.Add(_newHistory);
            As.SaveChanges();
        }
        private void HistorySearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HistorySearchBy.SelectedItem != null)
            {
                historySearchBy = HistorySearchBy.SelectedItem.ToString();
                if (historySearchBy == "Дата")
                {
                    HistorySearchText.Visibility = Visibility.Collapsed;
                    DateGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    HistorySearchText.Visibility = Visibility.Visible;
                    DateGrid.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!checkClosed)
            {
                NewHistory(1);
            }
            _mainWindow.Show();
        }

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(historySearchBy))
                {
                    HistoryList.ItemsSource = null;
                    switch (historySearchBy)
                    {
                        case "Дата":

                            DateTime? startDate = null;
                            DateTime? endDate = null;

                            if (!string.IsNullOrWhiteSpace(StartDate.Text))
                                startDate = DateTime.Parse(StartDate.Text);

                            if (!string.IsNullOrWhiteSpace(EndDate.Text))
                                endDate = DateTime.Parse(EndDate.Text);

                            IQueryable<History> query = As.Histories;

                            if (startDate.HasValue && endDate.HasValue)
                            {
                                query = query.Where(x => x.DateStart >= startDate.Value && x.DateEnd <= endDate.Value);
                            }
                            else if (startDate.HasValue)
                            {
                                query = query.Where(x => x.DateStart >= startDate.Value);
                            }
                            else if (endDate.HasValue)
                            {
                                query = query.Where(x => x.DateEnd <= endDate.Value);
                            }

                            HistoryList.ItemsSource = query.ToList();
                            break;
                    }
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Неверный формат даты. Пожалуйста, проверьте ввод.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitMainForLabAdmin_Click(object sender, RoutedEventArgs e)
        {
            NewHistory(1);
            _mainWindow.Visibility = Visibility.Visible;
            this.Close();
        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (RegPassword.Visibility == Visibility.Visible)
            {
                string pw = RegPassword.Password;
                RegPassword.Visibility = Visibility.Hidden;
                RegVisiblePassword.Visibility = Visibility.Visible;
                RegVisiblePassword.Text = pw;
            }
            else
            {
                string pw = RegVisiblePassword.Text;
                RegVisiblePassword.Visibility = Visibility.Hidden;
                RegPassword.Visibility = Visibility.Visible;
                RegPassword.Password = pw;
            }
        }

        private void RegClear_Click(object sender, RoutedEventArgs e)
        {
            RegSurname.Text = string.Empty;
            RegName.Text = string.Empty;
            RegPatronymic.Text = string.Empty;
            RegEmail.Text = string.Empty;
            RegPassword.Password = string.Empty;
            RegVisiblePassword.Text = string.Empty;
        }

        private void RegSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(RegSurname.Text) || string.IsNullOrEmpty(RegName.Text) || string.IsNullOrEmpty(RegPatronymic.Text)
                || string.IsNullOrEmpty(RegEmail.Text) || string.IsNullOrEmpty(RegLogin.Text) || string.IsNullOrEmpty(RegPassword.Password)
                || string.IsNullOrEmpty(RegNumberTel.Text))
                {
                    MessageBox.Show("Вы ввели не все данные!");
                }
                else
                {
                    string password = Cryptographer.HashPassword(RegPassword.Password);
                    if (LoginHelper.СheckMail(RegEmail.Text) && LoginHelper.Check_login(RegLogin.Text) && LoginHelper.Check_password(RegPassword.Password))
                    {
                        if (selectedSpec == -1)
                        {
                            MessageBox.Show("Специальность не выбрана!");
                            return;
                        }
                        User newUser = new User()
                        {
                            Surname = RegSurname.Text,
                            Name = RegName.Text,
                            Patronymic = RegPatronymic.Text,
                            Email = RegEmail.Text,
                            PhoneNumber = RegNumberTel.Text,
                            IdUserType = selectedSpec,
                            Login = RegLogin.Text,
                            Password = password
                        };
                        As.Users.Add(newUser);
                        As.SaveChanges();
                        MessageBox.Show("Успешно!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        private void Specialization_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Specialization.SelectedItem != null)
            {
                var spec = Specialization.SelectedItem.ToString();
                if (spec != null) 
                {
                    selectedSpec = As.UserTypes.Where(x => x.Type == spec).First().Id;
                }
            }
        }

        /////////////////
        private string fullSearchBy;
        private void FullSearchText_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(fullSearchBy))
            {
                FullList.ItemsSource = null;
                switch (fullSearchBy)
                {
                    case "Фамилия":
                        FullList.ItemsSource = As.OrderAndServices.Where(x => x.Order.User.Surname.StartsWith(FullSearchText.Text) && x.IdStatus == 3).ToList();
                        break;
                    case "Имя":
                        FullList.ItemsSource = As.OrderAndServices.Where(x => x.Order.User.Name.StartsWith(FullSearchText.Text) && x.IdStatus == 3).ToList();
                        break;
                    case "Отчество":
                        FullList.ItemsSource = As.OrderAndServices.Where(x => x.Order.User.Patronymic.StartsWith(FullSearchText.Text) && x.IdStatus == 3).ToList();
                        break;
                    case "Услуга":
                        FullList.ItemsSource = As.OrderAndServices.Where(x => x.Service.Service1.StartsWith(FullSearchText.Text) && x.IdStatus == 3).ToList();
                        break;

                }

            }
        }

        private void FullSearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FullSearchBy.SelectedItem != null)
            {
                fullSearchBy = FullSearchBy.SelectedItem.ToString();
            }
        }
    }
}
