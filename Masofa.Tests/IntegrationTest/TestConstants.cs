namespace Masofa.Tests.IntegrationTest
{
    public static class TestConstants
    {
        public const string BASE_URL = "http://185.100.234.107:30020";

        // Valid credentials
        public const string TEST_USERNAME = "TestAdmin";
        public const string TEST_PASSWORD = "PasswordTestUser";
        public const string NEW_TEST_PASSWORD = "ChangePasswordTestUser";
        public static readonly Guid TEST_USER_GUID = Guid.Parse("019957da-203d-7452-991f-0cbaefd7b7aa");

        // Invalid credentials
        public const string WRONG_USERNAME = "InvalidUserName";
        public const string WRONG_PASSWORD = "InvalidPassword";


        // Данные для Bid в тестах
        public static readonly Guid EXISTING_BID_ID = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        public static readonly Guid EXISTING_BID_TYPE_ID = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        public static readonly Guid EXISTING_FOREMAN_ID = Guid.Parse("019957da-203d-7452-991f-0cbaefd7b7aa");
        public static readonly Guid EXISTING_WORKER_ID = Guid.Parse("0199ddec-28dd-7730-b8b5-66ec980edda2");
        public static readonly Guid EXISTING_CROP_ID = Guid.Parse("01993db2-e370-74da-b4d1-d7c05519c0a5");
        public static readonly Guid EXISTING_BID_TEMPLATE_ID = Guid.Parse("0199581b-c355-780a-8244-75ad3343b605");
        public const double TEST_LAT = 41.0; // Пример координат
        public const double TEST_LNG = 69.0; // Пример координат
        public static readonly DateTime TEST_FIELD_PLANTING_DATE = DateTime.UtcNow.AddDays(0);
        public static readonly DateTime TEST_DEADLINE_DATE = DateTime.UtcNow.AddDays(-7);
        public static readonly DateTime? TEST_START_DATE = DateTime.UtcNow.AddDays(1);
        public static readonly DateTime? TEST_END_DATE = DateTime.UtcNow.AddDays(5);
        public const string TEST_COMMENT = "Demo";
        public const string TEST_DESCRIPTION = "Demo";
        public const string TEST_CUSTOMER = "Demo Customer";
    }
}