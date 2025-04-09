using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mail_LIB
{
    public static class LoginHelper
    {
        public static bool СheckMail(string email)
        {
            bool isCorrect = false;
            string EmailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(email))
            {
                errors.Add("Поле 'email' пустое.");
                isCorrect = false;
            }
            else if (email.Length > 50)
            {
                errors.Add($"Длина адреса превышает допустимое значение (50 символов): {email}");
                isCorrect = false;
            }

            Regex regex = new Regex(EmailRegex); 
            if (!regex.IsMatch(email))
            {
                errors.Add($"Адрес '{email}' имеет неверный формат.");
                isCorrect = false;
            }

            if (errors.Count > 0)
            {
                string errorMessage = "Список ошибок:\n";
                foreach (var error in errors)
                {
                    errorMessage += $"• {error}\n";
                }

                MessageBox.Show(errorMessage, "Ошибка валидации email"); // Убедитесь, что у вас есть доступ к MessageBox
            }
            else
            {
                isCorrect = true;
            }

            return isCorrect;
        }
        public static bool Check_password(string password)
        {
            bool isCorrect = false;
            List<string> errors = new List<string>();

            // Проверка минимальной длины пароля
            if (password.Length < 8)
            {
                errors.Add("Пароль должен содержать минимум 8 символов.");
                isCorrect = false;
            }

            // Проверка наличия хотя бы одной заглавной буквы
            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                errors.Add("Пароль должен содержать хотя бы одну заглавную букву.");
                isCorrect = false;
            }

            // Проверка наличия хотя бы одной строчной буквы
            if (!Regex.IsMatch(password, "[a-z]"))
            {
                errors.Add("Пароль должен содержать хотя бы одну строчную букву.");
                isCorrect = false;
            }

            // Проверка наличия хотя бы одной цифры
            if (!Regex.IsMatch(password, @"\d"))
            {
                errors.Add("Пароль должен содержать хотя бы одну цифру.");
                isCorrect = false;
            }

            // Проверка наличия хотя бы одного специального символа
            if (!Regex.IsMatch(password, @"[^a-zA-Z\d]"))
            {
                errors.Add("Пароль должен содержать хотя бы один специальный символ.");
                isCorrect = false;
            }

            // Вывод результата проверки
            if (errors.Count > 0)
            {
                string errorMessage = "Ошибки в пароле:\n";
                foreach (var error in errors)
                {
                    errorMessage += $"• {error}\n";
                }
                MessageBox.Show(errorMessage, "Ошибка валидации пароля");
            }
            else
            {
                isCorrect = true;
            }
            return isCorrect;
        }

        public static bool Check_login(string login)
        {
            bool isCorrect = false;
            List<string> errors = new List<string>();

            // Проверка минимальной длины логина
            if (login.Length < 6)
            {
                errors.Add("Логин должен содержать минимум 6 символов.");
                isCorrect = false;
            }

            // Проверка на допустимые символы (латинские буквы и цифры)
            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9]+$"))
            {
                errors.Add("Логин может содержать только латинские буквы и цифры.");
                isCorrect = false;
            }

            // Вывод результата проверки
            if (errors.Count > 0)
            {
                string errorMessage = "Ошибки в логине:\n";
                foreach (var error in errors)
                {
                    errorMessage += $"• {error}\n";
                }
                MessageBox.Show(errorMessage, "Ошибка валидации логина");
            }
            else
            {
                isCorrect = true;
            }
            return isCorrect;
        }
    }
}
