using iText.Layout.Element;
using System;
using System.Collections.Generic;
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

namespace MedLab.Windows.BoogalterWindows
{
    /// <summary>
    /// Логика взаимодействия для MainBoogalterWindows.xaml
    /// </summary>
    public partial class MainBoogalterWindows : Window
    {
        private MainWindow _mainWindow;
        private User _users;
        private History _newHistory;
        private bool checkClosed = false;
        ASEntities As = new ASEntities();

        public MainBoogalterWindows(MainWindow main_, User users_, History history_)
        {
            InitializeComponent();
            try
            {
                _mainWindow = main_;
                _users = users_;
                _newHistory = history_;
                ListAccount.ItemsSource = As.Orders.ToList();
                InsuranceCompanyCB.ItemsSource = As.InsuranceCompanies.Select(x => x.Name).ToList();
                FIOUser.Text = $"{_users.Surname} {_users.Name} {_users.Patronymic}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void NewHistory(int reason)
        {
            _newHistory.DateEnd = DateTime.Now;
            _newHistory.IdReason = reason;
            As.Histories.Add(_newHistory);
            As.SaveChanges();
        }

        private void ExitMainForBuh_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Show();
            checkClosed = true;
            NewHistory(1);
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (!checkClosed)
                {
                    NewHistory(1);
                }
                _mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "AS");
            }

        }

        private DateTime? dateStart = null;
        private DateTime? dateEnd = null;
        private int companyId = 0;

        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(StartDate.Text))
            {
                if (DateTime.TryParse(StartDate.Text, out var parsedStartDate))
                {
                    dateStart = parsedStartDate;
                }
                else
                {
                    MessageBox.Show("Некорректный формат начальной даты");
                    dateStart = null;
                }
            }
            else
            {
                dateStart = null;
            }
            ApplyFilters();
        }

        private void EndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EndDate.Text))
            {
                if (DateTime.TryParse(EndDate.Text, out var parsedEndDate))
                {
                    dateEnd = parsedEndDate;
                }
                else
                {
                    MessageBox.Show("Некорректный формат конечной даты");
                    dateEnd = null;
                }
            }
            else
            {
                dateEnd = null;
            }

            ApplyFilters();
        }

        private void InsuranceCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InsuranceCompanyCB.SelectedItem != null)
            {
                string nameCompany = InsuranceCompanyCB.SelectedItem.ToString();
                var company = As.InsuranceCompanies.FirstOrDefault(x => x.Name == nameCompany);
                if (company != null)
                {
                    companyId = company.Id;
                }
                else
                {
                    companyId = 0;
                }
            }
            else
            {
                companyId = 0;
            }

            ApplyFilters();
        }
        private IQueryable<Order> query;
        private void ApplyFilters()
        {
            query = As.Orders;

            if (companyId > 0)
            {
                query = query.Where(x => x.User.InsuranceCompany1.Id == companyId);
            }
           
            if (dateStart.HasValue && dateEnd.HasValue)
            {
                query = query.Where(d => d.DateOfCreation >= dateStart.Value && d.DateOfCreation <= dateEnd.Value);
            }

            else if (dateStart.HasValue)
            {
                query = query.Where(d => d.DateOfCreation >= dateStart.Value);
            }

            else if (dateEnd.HasValue)
            {
                query = query.Where(d => d.DateOfCreation <= dateEnd.Value);
            }

            ListAccount.ItemsSource = query.ToList();
        }

        public string FormingInvoiceNumber() 
        {
            string year = DateTime.Now.Year.ToString();
            int lastSerialNumber = As.AccountForInsuranceCompanies.OrderByDescending(x => x.Id).FirstOrDefault().Id;
            return $"ACC-{year}-{++lastSerialNumber}";
        }
        private void AddNewAccount_Click(object sender, RoutedEventArgs e)
        {
            if (companyId != 0 && dateEnd.HasValue && dateStart.HasValue)
            {
                decimal amount = 0;
                foreach (var item in query)
                {
                    amount += item.FullPrice;
                }
                AccountForInsuranceCompany accountForInsuranceCompanies_ = new AccountForInsuranceCompany()
                {
                    IdUser = _users.Id,
                    InvoiceNumber = FormingInvoiceNumber(),
                    InvoiceDate = dateStart,
                    Amount = amount,
                    IdInsuranceCompany = companyId,
                    IdStatusAccount = 2,
                    EndDate = dateEnd,
                };
                As.AccountForInsuranceCompanies.Add(accountForInsuranceCompanies_);
                As.SaveChanges();
                MessageBox.Show("Успешно!", "Уведомление от программы AS", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Страховая компания или временной промежуток не выбраны!", "AS", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
        }
    }
}
