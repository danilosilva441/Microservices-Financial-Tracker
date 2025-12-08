namespace BillingService.Enums
{
    public enum ScheduleType
    {
        Diarista = 1,       // Ex: Seg-Sex, Horário Comercial
        Plantonista12x36 = 2, // 12h trabalho, 36h descanso
        Plantonista6x1 = 3,   // 6 dias trabalho, 1 folga
        Flexivel = 4          // Horários variáveis
    }
}