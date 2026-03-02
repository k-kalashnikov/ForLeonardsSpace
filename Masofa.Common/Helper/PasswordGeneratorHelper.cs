namespace Masofa.Common.Helper
{
    public static class PasswordGeneratorHelper
    {
        private static readonly Random Random = new Random();
        // Буквы: строчные и заглавные
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "123456789";
        private const string SpecialChars = "!@#$%^&*()_-+=<>?";

        // Общий алфавит (можно расширить)
        private const string AllChars = Lowercase + Uppercase + Digits;

        /// <summary>
        /// Генерирует пароль по заданным правилам Identity.
        /// Длина >= 6, содержит минимум 1 строчную и 1 заглавную букву.
        /// </summary>
        /// <param name="length">Длина пароля (минимум 6)</param>
        /// <returns>Сгенерированный пароль</returns>
        public static string GeneratePassword(int length = 8)
        {
            if (length < 6)
                throw new ArgumentException("Длина должна быть не менее 6 символов.", nameof(length));

            var password = new char[length];
            int position;

            // Шаг 1: гарантируем наличие строчной буквы
            position = Random.Next(0, length);
            password[position] = Lowercase[Random.Next(Lowercase.Length)];

            // Шаг 2: гарантируем наличие заглавной буквы
            do
            {
                position = Random.Next(0, length);
            } while (password[position] != '\0'); // чтобы не перезаписать предыдущую

            password[position] = Uppercase[Random.Next(Uppercase.Length)];

            // Шаг 3: заполняем оставшиеся позиции любыми символами (буквы, цифры)
            for (int i = 0; i < length; i++)
            {
                if (password[i] == '\0') // свободная позиция
                {
                    password[i] = AllChars[Random.Next(AllChars.Length)];
                }
            }

            return new string(password);
        }
    }
}
