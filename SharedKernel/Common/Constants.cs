namespace SharedKernel.Common
{
    public static class Constants
    {
        public static class Claims
        {
            // O nome exato da claim que você está usando para identificar o Tenant
            public const string TenantId = "tenantId"; 
            
            // Outras claims comuns (verifique qual seu AuthService usa)
            public const string UserId = "id"; // ou "sub"
            public const string Email = "email";
            public const string Role = "role";
            public const string UserName = "unique_name";
        }

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Manager = "Manager";
            public const string Employee = "Employee";
            public const string Dev = "Dev";
            public const string Gerente = "Gerente";
            public const string Supervisor = "Supervisor";
            public const string Lider = "Lider";
            public const string Operador = "Operador";
            public const string User = "User";
        }

        public static class Policies
        {
            public const string AdminOnly = "AdminOnly";
        }
        
        public static class Headers
        {
            public const string TenantId = "X-Tenant-ID";
        }
    }
}