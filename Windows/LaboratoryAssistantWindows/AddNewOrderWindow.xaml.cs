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

namespace MedLab.Windows.LaboratoryAssistantWindows
{
    public partial class AddNewOrderWindow : Window
    {
        ASEntities As = new ASEntities();
        // возможность выбора сразу нескольких анализов в заказ
        User selectedUser;
        Order newOrder;
        public AddNewOrderWindow(User users_)
        {
            InitializeComponent();
            try
            {
                ListService.ItemsSource = As.Services.ToList();
                selectedUser = users_;
                newOrder = new Order()
                {
                    DateOfCreation = DateTime.Now,
                    IdStatusOrder = 2,
                    IdUser = selectedUser.Id
                };
                As.Orders.Add(newOrder);
                As.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }    
        }

        private void AddNewService_Click(object sender, RoutedEventArgs e)
        {
            var selectedService = ListService.SelectedItem as Service;
            AddBarcode addBarcode = new AddBarcode(newOrder, selectedService);
            addBarcode.Show();
        }

        private void ListService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sel = ListService.SelectedItem as Service;
        }
    }
}
