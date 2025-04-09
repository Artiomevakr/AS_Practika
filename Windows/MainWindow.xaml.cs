using MedLab.Windows.HelpWindows;
using MedLab.Windows.LaboratoryAssistantWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Mail_LIB;
using MedLab.Windows.AdministratorWindows;
using MedLab.Windows.LaboratoryAssistantResearcherWindows;
using MedLab.Windows.BoogalterWindows;
using MedLab.Windows.UserWindows;

namespace MedLab
{
    public class IncorrectUser
    {
        public IncorrectUser(string userName, int incorrectInput) 
        {
            this.userName = userName;
            this.incorrectInput = incorrectInput;
        }
        public string userName { get; set; }
        public int incorrectInput { get; set; }
    }
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        ASEntities As = new ASEntities();
        private List<IncorrectUser> incorrectUsers = new List<IncorrectUser>();
        //РАСКРЫТЬ ПАРОЛЬ

        /// <summary>
        /// Обработчик нажатия кнопки для отображения/скрытия пароля
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Аргументы события</param>
        /// <remarks>
        /// <para>Функционал:</para>
        /// <list type="bullet">
        /// <item>Переключает видимость пароля между текстовым и скрытым режимами</item>
        /// <item>Работает с двумя парами элементов управления паролем:</item>
        ///   <list type="bullet">
        ///     <item>PasswordAvt (скрытый) и VisiblePassword (видимый) - для авторизации</item>
        ///     <item>RegPassword (скрытый) и RegVisiblePassword (видимый) - для регистрации</item>
        ///   </list>
        /// <item>Сохраняет значение пароля при переключении режимов отображения</item>
        /// </list>
        /// <para>Особенности:</para>
        /// <list type="bullet">
        /// <item>Использует механизм Visibility для переключения между элементами</item>
        /// <item>Обеспечивает синхронизацию значений между скрытым и видимым полями</item>
        /// <item>Работает независимо для каждой пары полей ввода пароля</item>
        /// </list>
        /// </remarks>
        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordAvt.Visibility == Visibility.Visible)
            {
                string pw = PasswordAvt.Password;
                PasswordAvt.Visibility = Visibility.Hidden;
                VisiblePassword.Visibility = Visibility.Visible;
                VisiblePassword.Text = pw;
            }
            else
            {
                string pw = VisiblePassword.Text;
                VisiblePassword.Visibility = Visibility.Hidden;
                PasswordAvt.Visibility = Visibility.Visible;
                PasswordAvt.Password = pw;
            }
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

        private void AvtClear_Click(object sender, RoutedEventArgs e)
        {
            string err = Cryptographer.HashPassword(PasswordAvt.Password);
            AvtLogin.Text = string.Empty;
            PasswordAvt.Password = string.Empty;
            VisiblePassword.Text = string.Empty;
        }

        //РЕГИСТРАЦИЯ
        private void RegSave_Click(object sender, RoutedEventArgs e)
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
                }
            }
        }

        
        private void RegClear_Click(object sender, RoutedEventArgs e)
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
        /////////////////////

        public static History newHistory = new History();
        public static User userLogin;
        private bool isCorrect = false;
        private ASEntities ASEntitiesForLoginUser;


        /// <summary>
        /// Выполняет аутентификацию пользователя и авторизацию в системе в зависимости от типа пользователя.
        /// </summary>
        /// <remarks>
        /// <para>Основной функционал:</para>
        /// <list type="bullet">
        /// <item>Проверяет существование логина в базе данных</item>
        /// <item>Сравнивает введённый пароль с хэшем в базе данных</item>
        /// <item>Проверяет временные ограничения для лаборантов (кварцевание)</item>
        /// <item>Перенаправляет пользователя на соответствующую форму в зависимости от типа учётной записи</item>
        /// <item>Записывает историю входа в систему</item>
        /// </list>
        /// <para>Обрабатываемые роли пользователей:</para>
        /// <list type="bullet">
        /// <item>Лаборант</item>
        /// <item>Лаборант-исследователь</item>
        /// <item>Администратор</item>
        /// <item>Клиент</item>
        /// <item>Бухгалтер</item>
        /// </list>
        /// </remarks>
        /// <exception cref="Exception">Может выбрасывать исключения при работе с базой данных (только для администратора)</exception>
        public void LoginUser() 
        {
            ASEntitiesForLoginUser = new ASEntities();
            if (ASEntitiesForLoginUser.Users.Any(x => x.Login == AvtLogin.Text))
            {
                History lastExitUser = null;
                userLogin = ASEntitiesForLoginUser.Users.FirstOrDefault(x => x.Login == AvtLogin.Text);
                if (userLogin.Histories.Where(x => x.IdUser == userLogin.Id).Any()) 
                {
                    lastExitUser = userLogin.Histories.Where(x => x.IdUser == userLogin.Id).Last();
                }
                if (Cryptographer.VerifyPassword(PasswordAvt.Password, userLogin.Password))
                {
                    switch (userLogin.UserType.Type)
                    {
                        case "Лаборант":
                            if (lastExitUser != null && lastExitUser.ReasonsForExit.Reason == "Время вышло"
                                && TimeSpan.FromMinutes(15) > DateTime.Now - lastExitUser.DateEnd)
                            {
                                MessageBox.Show("Время кварцевания еще не вышло!");
                                isCorrect = true;
                            }
                            else
                            {
                                newHistory.IdUser = userLogin.Id;
                                newHistory.DateStart = DateTime.Now;
                                newHistory.Ip = HelperIP.GetIp();
                                MainLabAssistant mainLabAssistant = new MainLabAssistant(userLogin, newHistory, this);
                                mainLabAssistant.Show();
                                this.Hide();
                                isCorrect = true;
                            }
                            break;
                        case "Лаборант-исследователь":
                            if (lastExitUser != null && lastExitUser.ReasonsForExit.Reason == "Время вышло"
                                && TimeSpan.FromMinutes(15) > DateTime.Now - lastExitUser.DateEnd)
                            {
                                MessageBox.Show("Время кварцевания еще не вышло!");
                                isCorrect = true;
                            }
                            else
                            {
                                newHistory.IdUser = userLogin.Id;
                                newHistory.DateStart = DateTime.Now;
                                newHistory.Ip = HelperIP.GetIp();
                                MainLaboratoryAssistantResearcher mainLabAssistantRes = new MainLaboratoryAssistantResearcher(this, newHistory, userLogin);
                                mainLabAssistantRes.Show();
                                this.Hide();
                                isCorrect = true;
                            }
                            break;
                        case "Администратор":
                            try
                            {
                                newHistory.IdUser = userLogin.Id;
                                newHistory.DateStart = DateTime.Now;
                                newHistory.Ip = HelperIP.GetIp();
                                MainAdministranotWindows mainAdministranotWindows = new MainAdministranotWindows(this, newHistory, userLogin);
                                mainAdministranotWindows.Show();
                                this.Hide();
                                isCorrect = true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                return;
                            }
                            isCorrect = true;
                            break;
                        case "Клиент":
                            newHistory.IdUser = userLogin.Id;
                            newHistory.DateStart = DateTime.Now;
                            newHistory.Ip = HelperIP.GetIp();
                            MainUserWindows mainUserWindows = new MainUserWindows(this, userLogin, newHistory);
                            mainUserWindows.Show();
                            this.Hide();
                            isCorrect = true;
                            break;
                        case "Бухгалтер":
                            newHistory.IdUser = userLogin.Id;
                            newHistory.DateStart = DateTime.Now;
                            newHistory.Ip = HelperIP.GetIp();
                            MainBoogalterWindows mainBoogalterWindows = new MainBoogalterWindows(this, userLogin, newHistory);
                            mainBoogalterWindows.Show();
                            this.Hide();
                            isCorrect = true;
                            break;
                        default:
                            MessageBox.Show("Ошибка БД");
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Пароль не верен!");
                    isCorrect = false;
                }
            }
            else
            {
                MessageBox.Show("Логин не найден!");
                isCorrect = false;
            }
        }

        //АВТОРИЗАЦИЯ

        /// <summary>
        /// Обработчик нажатия кнопки входа в систему
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Аргументы события</param>
        /// <remarks>
        /// <para>Основные функции:</para>
        /// <list type="bullet">
        /// <item>Проверяет заполнение полей логина и пароля</item>
        /// <item>Вызывает метод аутентификации LoginUser()</item>
        /// <item>Обрабатывает некорректные попытки входа:</item>
        ///   <list type="bullet">
        ///     <item>Ведет учет количества неудачных попыток для каждого пользователя</item>
        ///     <item>При 3+ неудачных попытках показывает окно CAPTCHA</item>
        ///   </list>
        /// </list>
        /// <para>Особенности работы:</para>
        /// <list type="bullet">
        /// <item>Использует коллекцию incorrectUsers для отслеживания неудачных попыток</item>
        /// <item>Скрывает текущее окно при показе CAPTCHA</item>
        /// <item>Обновляет счетчик неудачных попыток после каждой проверки</item>
        /// </list>
        /// </remarks>
        private void AvtEnter_Click(object sender, EventArgs e)
        {   
            if (!string.IsNullOrEmpty(AvtLogin.Text) && !string.IsNullOrEmpty(PasswordAvt.Password))
            {
                LoginUser();
                if (!isCorrect)
                {
                    if (!incorrectUsers.Any(x=> x.userName == AvtLogin.Text))
                    {
                        incorrectUsers.Add(new IncorrectUser(AvtLogin.Text, 0));
                    }
                    var incorrectUser = incorrectUsers.Where(x => x.userName == AvtLogin.Text).FirstOrDefault();
                    if (incorrectUser.incorrectInput > 1)
                    {
                        InputCaptchaWindow inputCaptchaWindow = new InputCaptchaWindow(this, userLogin);
                        inputCaptchaWindow.Show();
                        this.Hide();
                    }
                    int NumberOfIncorrectInputs = incorrectUser.incorrectInput;
                    string userName = incorrectUser.userName;
                    incorrectUsers.Remove(incorrectUser);
                    incorrectUsers.Add(new IncorrectUser(userName, incorrectUser.incorrectInput + 1));
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
        }
    }
}
