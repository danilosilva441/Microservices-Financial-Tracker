const express = require('express');
const cors = require('cors');
const cron = require('node-cron');
const { getAuthToken } = require('./auth.js');
const { fetchOperacoes, updateProjecao } = require('./services/billingService.js');

// 1. Inicializa o Express
const app = express();
const port = 3000;

// 2. Configura o CORS (permite requisições do frontend)
app.use(cors({ origin: 'http://localhost:5173' }));

// 3. Rota para o Healthcheck do Docker
app.get('/health', (req, res) => {
    res.status(200).send('OK');
});

// --- LÓGICA DO JOB AGENDADO (CRON) ---
async function runProjectionJob() {
  console.log('Analysis Service: Iniciando job de projeção...');
  const token = await getAuthToken();
  if (!token) return;

  const operacoes = await fetchOperacoes(token);
  if (operacoes && operacoes.length > 0) {
    console.log(`Analysis Service: ${operacoes.length} operações encontradas. Atualizando projeções...`);
    for (const op of operacoes) {
      const faturamentos = op.faturamentos?.$values || [];
      const projecaoCalculada = faturamentos
        .filter(f => f.isAtivo)
        .reduce((total, fat) => total + fat.valor, 0);

      if (op.projecaoFaturamento !== projecaoCalculada) {
        await updateProjecao(op.id, projecaoCalculada, token);
      }
    }
    console.log('Analysis Service: Job de projeção concluído.');
  } else {
    console.log('Analysis Service: Nenhuma operação encontrada.');
  }
}

// Agenda o job para rodar a cada minuto
cron.schedule('* * * * *', runProjectionJob);

// Roda o job uma vez na inicialização
setTimeout(() => {
    console.log("Executando job de projeção inicial...");
    runProjectionJob().catch(err => console.error("Erro no job inicial:", err));
}, 15000); // 15 segundos para dar tempo aos outros serviços

// 4. Inicia o servidor Express para escutar as requisições
app.listen(port, () => {
  console.log(`Analysis Service rodando e escutando na porta ${port}`);
});