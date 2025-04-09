using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

public class Cryptographer
{

    public static string HashPassword(string password)
    {
        // Создаем случайную соль длиной 16 байтов
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Хэшируем пароль с использованием соли и итераций
        const int iterations = 10000; // Рекомендуемое количество итераций
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        byte[] hash = pbkdf2.GetBytes(20); // Длина хэша в байтах

        // Объединяем соль и хэш для хранения
        byte[] hashBytes = new byte[36]; // Размер соли + размер хэша
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }


    public static bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            // Декодируем Base64 строку обратно в массив байтов
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Извлекаем соль и хэш из массива байтов
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            byte[] storedHash = new byte[20];
            Array.Copy(hashBytes, 16, storedHash, 0, 20);

            // Повторяем процесс хэширования с той же солью и числом итераций
            const int iterations = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] computedHash = pbkdf2.GetBytes(20);

            // Сравниваем полученный хэш с хранимым
            for (int i = 0; i < 20; i++)
            {
                if (computedHash[i] != storedHash[i])
                    return false;
            }

            return true;
        }
        catch (Exception)
        {
            MessageBox.Show("Ошибка проверки пароля!");
            return false;
        }
        
    }
}
