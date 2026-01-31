import api from './api';

export default {
    // Busca o "Super JSON" do Dashboard
    // Pode receber filtros opcionais (startDate, endDate, unidadeId)
    async getDashboardData(filters = {}) {
        // Converte objeto de filtros em query string: ?unidadeId=...&startDate=...
        const params = new URLSearchParams(filters).toString();
        const response = await api.get(`/api/analysis/dashboard-data?${params}`);
        return response.data; // Retorna { success: true, data: { ... } }
    }
};