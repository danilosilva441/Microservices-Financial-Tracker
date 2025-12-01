// Caminho: SharedKernel/ErrorMessages.cs
namespace SharedKernel
{
    /// <summary>
    /// Centraliza todas as mensagens de erro da aplicação (v2.0)
    /// para garantir consistência e facilitar a manutenção/tradução.
    /// </summary>
    public static class ErrorMessages
    {
        // Erros de Autenticação (AuthService)
        public const string InvalidCredentials = "Credenciais inválidas.";
        public const string EmailInUse = "Um usuario com este email já existe.";
        public const string UserNotFound = "Usuario não encontrado.";
        public const string RoleNotFound = "Perfil (Role) não encontrado no banco de dados.";
        public const string TenantProvisionFailed = "Erro ao provisionar a nova empresa (Tenant).";
        public const string AccountLocked = "Conta temporariamente bloqueada por excesso de tentativas.";
        public const string PasswordExpired = "A palavra-passe expirou. Por favor, redefina-a.";
        public const string InvalidToken = "Token inválido ou expirado.";
        
        // Erros de Hierarquia (AuthService / UserService)
        public const string InvalidRoleCreation = "Permissão negada. Só é possível criar perfis 'Supervisor', 'Lider' ou 'Operador'.";
        public const string ManagerTenantRequired = "Ação falhou. O Gerente não tem um TenantId associado.";
        public const string InsufficientPermissions = "Permissões insuficientes para realizar esta ação.";
        public const string CannotModifyOwnRole = "Não pode modificar o seu próprio perfil.";
        public const string CannotDeleteOwnAccount = "Não pode eliminar a sua própria conta.";
        public const string UserNoPermission = "Usuário sem permissão para esta ação.";
        
        // Erros de Negócio (BillingService)
        public const string UnidadeNotFound = "Unidade não encontrada ou não pertence a este Tenant.";
        public const string CategoriaNotFound = "Categoria de despesa não encontrada.";
        public const string DespesaNotFound = "Despesa não encontrada.";
        public const string FechamentoNotFound = "Fechamento não encontrado.";
        public const string FechamentoJaExiste = "Já existe um fechamento submetido para esta data.";
        public const string DespesaExceedsBudget = "A despesa excede o orçamento definido para esta categoria.";
        public const string InvalidDateRange = "Intervalo de datas inválido.";
        public const string DataFuturaNaoPermitida = "Não são permtidas datas futuras para esta operação.";
        public const string NoAlteredStatus = "Não é permitido alterar o status de 'Aprovado' para pendente.";
        public const string FaturamentoNotFound = "Faturamento não encontrado.";
        public const string FaturamentoParcialNotFound = "Faturamento parcial não encontrado.";
        public const string FaturamentoJaExiste = "Já existe um faturamento registrado para este período.";
        public const string OverlappingFaturamento = "Já existe um faturamento registrado com sobreposição de horário.";
        public const string AcessoNegado  = "Acesso negado ao recurso solicitado.";

        
        // Erros de Validação
        public const string RequiredField = "O campo {0} é obrigatório.";
        public const string InvalidEmailFormat = "Formato de email inválido.";
        public const string StringLengthExceeded = "O campo {0} não pode exceder {1} caracteres.";
        public const string InvalidNumberRange = "O valor deve estar entre {0} e {1}.";
        public const string InvalidDateFormat = "Formato de data inválido. Use DD/MM/AAAA.";
        
        // Erros de Sistema/Infraestrutura
        public const string DatabaseConnectionFailed = "Erro de conexão com a base de dados.";
        public const string ExternalServiceUnavailable = "Serviço externo indisponível.";
        public const string FileStorageError = "Erro ao armazenar o ficheiro.";
        public const string EmailServiceError = "Erro ao enviar email.";
        public const string ConcurrentModification = "O recurso foi modificado por outro utilizador. Por favor, recarregue os dados.";
        
        // Erros Genéricos
        public const string GenericNotFound = "Recurso não encontrado.";
        public const string FileRequired = "O ficheiro é obrigatório.";
        public const string InvalidFileType = "Formato de ficheiro inválido.";
        public const string OperationFailed = "A operação falhou. Tente novamente.";
        public const string InvalidOperation = "Operação inválida para o estado atual do recurso.";
    }
}