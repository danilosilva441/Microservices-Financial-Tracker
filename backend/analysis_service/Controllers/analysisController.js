const { calcularProjecao } = require('../Services/analysisService'); // <--- NOME ATUALIZADO

function projetarManualmente(req, res) {
    const { metaMensal, faturamentos } = req.body;
    if (!metaMensal || !faturamentos || !Array.isArray(faturamentos)) {
        return res.status(400).json({ error: 'Dados incompletos.' });
    }
    const { valorAcumuladoAtual, projecaoFinal, percentualProjetado } = calcularProjecao(metaMensal, faturamentos);
    let mensagem = `Projeção de ${percentualProjetado.toFixed(2)}% da meta.`;
    if (percentualProjetado >= 100) {
        mensagem += " Excelente! A meta provavelmente será atingida ou superada.";
    } else {
        mensagem += " Atenção! A meta provavelmente não será atingida neste ritmo.";
    }
    const response = {
        metaMensal,
        valorAcumuladoAtual,
        projecaoFinal: parseFloat(projecaoFinal.toFixed(2)),
        percentualProjetado: parseFloat(percentualProjetado.toFixed(2)),
        mensagem
    };
    res.status(200).json(response);
}

module.exports = { projetarManualmente };