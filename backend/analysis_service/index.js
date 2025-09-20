const express = require('express');
const app = express();
const PORT = process.env.PORT || 3000;

app.use(express.json());

app.post('/projetar', (req, res) => {
    // 1. Nova estrutura de dados de entrada
    const { metaMensal, faturamentos } = req.body;

    // 2. Validação dos dados
    if (!metaMensal || !faturamentos || !Array.isArray(faturamentos)) {
        return res.status(400).json({ error: 'Dados incompletos. São necessários: metaMensal e uma lista de faturamentos.' });
    }

    if (faturamentos.length === 0) {
        return res.status(200).json({
            metaMensal,
            valorAcumuladoAtual: 0,
            projecaoFinal: 0,
            percentualProjetado: 0,
            mensagem: "Nenhum faturamento registrado para análise."
        });
    }

    // 3. Lógica de projeção aprimorada
    const hoje = new Date();
    const diaAtualDoMes = hoje.getDate();
    const totalDeDiasNoMes = new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0).getDate();

    // Calcula o total faturado somando os valores da lista
    const valorAcumuladoAtual = faturamentos.reduce((total, fat) => total + fat.valor, 0);

    const mediaDiaria = valorAcumuladoAtual / diaAtualDoMes;
    const projecaoFinal = mediaDiaria * totalDeDiasNoMes;
    const percentualProjetado = (projecaoFinal / metaMensal) * 100;

    let mensagem = `Projeção de ${percentualProjetado.toFixed(2)}% da meta.`;
    if (percentualProjetado >= 100) {
        mensagem += " Excelente! A meta provavelmente será atingida ou superada.";
    } else {
        mensagem += " Atenção! A meta provavelmente não será atingida neste ritmo.";
    }

    // 4. Monta o objeto de resposta
    const response = {
        metaMensal,
        valorAcumuladoAtual,
        projecaoFinal: parseFloat(projecaoFinal.toFixed(2)),
        percentualProjetado: parseFloat(percentualProjetado.toFixed(2)),
        mensagem
    };

    res.status(200).json(response);
});

app.listen(PORT, () => {
    console.log(`Analysis Service rodando na porta ${PORT}`);
});