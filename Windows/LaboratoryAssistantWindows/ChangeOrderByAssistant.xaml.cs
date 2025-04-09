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
    public partial class ChangeOrderByAssistant : Window
    {
        ASEntities As = new ASEntities();
        private Order _selectedOrder;
        public ChangeOrderByAssistant(Order selectedOrder)
        {
            InitializeComponent();
            try
            {
                ListNewServicesInOrder.ItemsSource = As.Services.ToList();
                _selectedOrder = selectedOrder;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddNewServicesInOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListNewServicesInOrder.SelectedItem != null)
                {
                    var selectedService = ListNewServicesInOrder.SelectedItem as Service;
                    if (selectedService != null)
                    {
                        OrderAndService newServiceInOrder = new OrderAndService
                        {
                            IdOrder = _selectedOrder.Id,
                            ServiceCode = selectedService.Code,
                            IdStatus = 1
                        };
                        As.OrderAndServices.Add(newServiceInOrder);
                        As.SaveChanges();
                        MessageBox.Show("Успешно!");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }
    }
}
