using Mail_LIB;
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
    public partial class AddNewUserByAssistant : Window
    {
        public AddNewUserByAssistant()
        {
            InitializeComponent();
        }

        ASEntities As = new ASEntities();
        private void RegClearAs_Click(object sender, RoutedEventArgs e)
        {
            RegSurname.Text = string.Empty;
            RegName.Text = string.Empty;
            RegPatronymic.Text = string.Empty;
            RegDateOfBirth.Text = string.Empty;
            RegPassportSeries.Text = string.Empty;
            RegPassportNumber.Text = string.Empty;
            RegEmail.Text = string.Empty;
            RegIncurancePolicyNumber.Text = string.Empty;
            RegPassword.Password = string.Empty;
            RegVisiblePassword.Text = string.Empty;
        }

        private void RegSaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(RegSurname.Text) || string.IsNullOrEmpty(RegName.Text) || string.IsNullOrEmpty(RegPatronymic.Text)
                || string.IsNullOrEmpty(RegDateOfBirth.Text)
                   || string.IsNullOrEmpty(RegPassportSeries.Text) || string.IsNullOrEmpty(RegPassportNumber.Text) || string.IsNullOrEmpty(RegEmail.Text)
                   || string.IsNullOrEmpty(RegIncurancePolicyNumber.Text) || string.IsNullOrEmpty(RegLogin.Text) || string.IsNullOrEmpty(RegPassword.Password) || string.IsNullOrEmpty(RegNumberTel.Text))
                {
                    MessageBox.Show("Вы ввели не все данные!");
                }
                else
                {
                    string password = Cryptographer.HashPassword(RegPassword.Password);
                    if (LoginHelper.СheckMail(RegEmail.Text) && LoginHelper.Check_login(RegLogin.Text) && LoginHelper.Check_password(RegPassword.Password))
                    {
                        User newUser = new User()
                        {
                            Surname = RegSurname.Text,
                            Name = RegName.Text,
                            Patronymic = RegPatronymic.Text,
                            DateOfBirth = Convert.ToDateTime(RegDateOfBirth.Text),
                            PassportSeries = RegPassportSeries.Text,
                            PassportNumber = RegPassportNumber.Text,
                            Email = RegEmail.Text,
                            PhoneNumber = RegNumberTel.Text,
                            IdUserType = 1,
                            InsurancePolicyNumber = RegIncurancePolicyNumber.Text,
                            Login = RegLogin.Text,
                            Password = password
                        };
                        As.Users.Add(newUser);
                        As.SaveChanges();
                        MessageBox.Show("Успешно");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
    }
}
