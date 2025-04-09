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

namespace MedLab.Windows.UserWindows
{
    public partial class MainUserWindows : Window
    {
        ASEntities As = new ASEntities();
        User _user;
        MainWindow _mainWindow;
        History _newHistory;
        public MainUserWindows(MainWindow mainWindow, User users_, History history_)
        {
            InitializeComponent();
            try
            {
                _user = users_;
                _mainWindow = mainWindow;
                _newHistory = history_;
                List<string> orderSearchBy = new List<string>()
                {
                    "Дата",
                    "Статус"
                };
                OrderSearchBy.ItemsSource = orderSearchBy;
                List<string> serviceSearchBy = new List<string>()
                {   
                    "Название",
                    "Статус",
                    "Начальная норма",
                    "Конечная норма",
                    "Результат"
                };
                ServiceSearchBy.ItemsSource = serviceSearchBy;
                FIOUser.Text = $"{_user.Surname} {_user.Name} {_user.Patronymic}";
                if (As.Orders.Where(x => x.IdUser == _user.Id).Count() != 0)
                {
                    OrderList.ItemsSource = As.Orders.Where(x => x.IdUser == _user.Id).ToList();
                }
                else
                {
                    MessageBox.Show("Вы у нас в первый раз! Заказов нет.", "AS", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OrderSearchText_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(orderSearchBy))
            {
                try
                {
                    OrderList.ItemsSource = null;
                    switch (orderSearchBy)
                    {
                        case "Статус":
                            OrderList.ItemsSource = As.Orders.Where(x => x.IdUser == _user.Id && x.StatusOrder.Status.ToString().StartsWith(OrderSearchText.Text)).ToList();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private string orderSearchBy;
        private void OrderSearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrderSearchBy.SelectedItem != null)
            {
                orderSearchBy = OrderSearchBy.SelectedItem.ToString();
                if (orderSearchBy == "Дата")
                {
                    OrderSearchText.Visibility = Visibility.Collapsed;
                    DateGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    OrderSearchText.Visibility = Visibility.Visible;
                    DateGrid.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool checkClosed = false;
        private void StartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(orderSearchBy))
                {
                    OrderList.ItemsSource = null;
                    switch (orderSearchBy)
                    {
                        case "Дата":
                            DateTime? startDate = null;
                            DateTime? endDate = null;

                            if (!string.IsNullOrWhiteSpace(StartDate.Text))
                                startDate = DateTime.Parse(StartDate.Text);

                            if (!string.IsNullOrWhiteSpace(EndDate.Text))
                                endDate = DateTime.Parse(EndDate.Text);

                            IQueryable<Order> query = As.Orders.Where(x => x.IdUser == _user.Id);

                            if (startDate.HasValue && endDate.HasValue)
                            {
                                query = query.Where(x => x.DateOfCreation >= startDate.Value && x.DateOfCreation <= endDate.Value);
                            }
                            else if (startDate.HasValue)
                            {
                                query = query.Where(x => x.DateOfCreation >= startDate.Value);
                            }
                            else if (endDate.HasValue)
                            {
                                query = query.Where(x => x.DateOfCreation <= endDate.Value);
                            }

                            OrderList.ItemsSource = query.ToList();
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

        public void NewHistory(int reason)
        {
            _newHistory.DateEnd = DateTime.Now;
            _newHistory.IdReason = reason;
            As.Histories.Add(_newHistory);
            As.SaveChanges();
        }

        private void ExitMainForUser_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Show();
            checkClosed = true;
            NewHistory(1);
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!checkClosed)
            {
                NewHistory(1);
            }
            _mainWindow.Show();
        }
        private Order selectedOrder;
        private void OrderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrderList.SelectedItem != null)
            {
                selectedOrder = OrderList.SelectedItem as Order;
                if (selectedOrder != null)
                {
                    ServiceList.ItemsSource = As.OrderAndServices.Where(x => x.IdOrder == selectedOrder.Id).ToList();
                }
            }
        }
        private string serviceSearchBy;
        private void SericeSearchText_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(serviceSearchBy))
            {      
                try
                {
                    ServiceList.ItemsSource = null;
                    switch (serviceSearchBy)
                    {
                        case "Статус":
                            ServiceList.ItemsSource = As.OrderAndServices.Where(x => x.IdOrder == selectedOrder.Id && x.StatusServicesInOrder.Status.StartsWith(SericeSearchText.Text)).ToList();
                            break;
                        case "Название":
                            ServiceList.ItemsSource = As.OrderAndServices.Where(x => x.IdOrder == selectedOrder.Id && x.Service.Service1.StartsWith(SericeSearchText.Text)).ToList();
                            break;
                        case "Начальная норма":
                            ServiceList.ItemsSource = As.OrderAndServices.Where(x => x.IdOrder == selectedOrder.Id && x.Service.InitialAVGDeviation.StartsWith(SericeSearchText.Text)).ToList();
                            break;
                        case "Конечная норма":
                            ServiceList.ItemsSource = As.OrderAndServices.Where(x => x.IdOrder == selectedOrder.Id && x.Service.FinalAVGDeviation.StartsWith(SericeSearchText.Text)).ToList();
                            break;
                        case "Результат":
                            ServiceList.ItemsSource = As.OrderAndServices.Where(x => x.IdOrder == selectedOrder.Id && x.Result.StartsWith(SericeSearchText.Text)).ToList();
                            break;
                        default:
                            MessageBox.Show("Ошибка");
                            break;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void ServiceSearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ServiceSearchBy.SelectedItem != null)
            {
                serviceSearchBy = ServiceSearchBy.SelectedItem.ToString();
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!checkClosed)
            {
                NewHistory(1);
            }
            _mainWindow.Show();
        }
    }
}
