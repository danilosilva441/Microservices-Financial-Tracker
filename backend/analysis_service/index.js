const express = require('express');
const app = express();
const PORT = process.env.PORT || 3000;

// Middleware para permitir que o Express entenda JSON no corpo das requisições
app.use(express.json());

// Rota principal para um teste de saúde (opcional, mas bom para testar)
app.get('/', (req, res) => {
    res.send('Analysis Service is running!');
});

// O endpoint principal do nosso serviço de análise
app.post('/projetar', (req, res) => {
    // 1. Extrai os dados do corpo da requisição
    const { valorMeta, valorAcumuladoAtual, diaAtualDoMes } = req.body;

    // 2. Validação simples dos dados de entrada
    if (!valorMeta || !valorAcumuladoAtual || !diaAtualDoMes) {
        return res.status(400).json({ error: 'Dados de entrada incompletos. São necessários: valorMeta, valorAcumuladoAtual, diaAtualDoMes.' });
    }

    if (diaAtualDoMes <= 0 || valorMeta <= 0) {
        return res.status(400).json({ error: 'Valores de entrada devem ser positivos.' });
    }

    // 3. Lógica da projeção
    const hoje = new Date();
    // Pega o total de dias no mês atual
    const totalDeDiasNoMes = new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0).getDate();

    const mediaDiaria = valorAcumuladoAtual / diaAtualDoMes;
    const projecaoFinal = mediaDiaria * totalDeDiasNoMes;
    const percentualProjetado = (projecaoFinal / valorMeta) * 100;

    let mensagem = `Projeção de ${percentualProjetado.toFixed(2)}% da meta.`;
    if (percentualProjetado >= 100) {
        mensagem += " Excelente! A meta provavelmente será atingida ou superada.";
    } else {
        mensagem += " Atenção! A meta provavelmente não será atingida neste ritmo.";
    }

    // 4. Monta o objeto de resposta
    const response = {
        valorMeta,
        valorAcumuladoAtual,
        projecaoFinal: parseFloat(projecaoFinal.toFixed(2)),
        percentualProjetado: parseFloat(percentualProjetado.toFixed(2)),
        mensagem
    };

    // 5. Envia a resposta
    res.status(200).json(response);
});

// Inicia o servidor para escutar na porta definida
app.listen(PORT, () => {
    console.log(`Analysis Service rodando na porta ${PORT}`);
});