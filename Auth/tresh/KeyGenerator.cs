using System;
using System.Security.Cryptography;

public class KeyGenerator
{
    public static string GenerateSymmetricKey(int keyLength)
    {
        // Створюємо об'єкт RNGCryptoServiceProvider для генерації випадкових байтів
        using (var rng = new RNGCryptoServiceProvider())
        {
            // Масив байтів для збереження згенерованого ключа
            byte[] keyBytes = new byte[keyLength / 8];
            
            // Заповнюємо масив випадковими байтами
            rng.GetBytes(keyBytes);

            // Повертаємо згенерований ключ у вигляді рядка шістнадцяткових символів
            return BitConverter.ToString(keyBytes).Replace("-", "");
        }
    }
}

// Приклад використання для генерації ключа довжиною 256 бітів (32 байти)


