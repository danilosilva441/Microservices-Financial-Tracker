// utils/dateFormatter.js
export const formatarDataParaAPI = (dataString) => {
    if (!dataString) return null;

    // Se já estiver no formato ISO (com T), retorna como está
    if (dataString.includes('T')) {
        return dataString;
    }

    // Converte "YYYY-MM-DD" para ISO com hora zero
    return new Date(dataString + 'T00:00:00').toISOString();
};

export const formatarDatasDoFormulario = (formData) => {
    const dadosFormatados = { ...formData };

    // Formata dataInicio (obrigatória)
    dadosFormatados.dataInicio = formatarDataParaAPI(formData.dataInicio);

    // Se dataFim for string vazia, remove o campo ou envia null
    if (formData.dataFim === '' || !formData.dataFim) {
        dadosFormatados.dataFim = null; // Envia como null
    } else {
        dadosFormatados.dataFim = formatarDataParaAPI(formData.dataFim);
    }

    return dadosFormatados;
};