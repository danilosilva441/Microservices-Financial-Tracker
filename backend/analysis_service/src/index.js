const express = require('express');
const cors = require('cors');
const cron = require('node-cron');
const { getAuthToken } = require('./auth.js');

// Importa TODAS as funções necessárias do billingService
const { 
  fetchOperacoes, 
  updateProjecao,
  calcularDashboardData // Importante para as rotas
} = require('./services/billingService.js'); 

// Importa o router do dashboard
const dashboardRoutes = require('./routes/dashboardRoutes.js');

// 1. Inicializa o Express
const app = express();
const port = 3000;

// 2. Configura o CORS (permite o seu api-gateway local)
app.use(cors({ origin: 'http://localhost:8080' })); 

// 3. Rota para o Healthcheck do Docker
app.get('/health', (req, res) => {
  res.status(200).send('OK');
});

// 4. Diz ao Express para usar as novas rotas do dashboard
// (O Nginx remove /api/analysis/, então o Express vê /dashboard-data)
app.use('/api/analysis', dashboardRoutes);

// --- LÓGICA DO JOB AGENDADO (CRON) ---
async function runProjectionJob() {
  console.log('Analysis Service: Iniciando job de projeção...');
  const token = await getAuthToken();
  if (!token) {
      console.log('Analysis Service: Não foi possível obter token, job interrompido.');
      return;
  }

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
    console.log('Analysis Service: Nenhuma operação encontrada para processar.');
  }
}

// Agenda o job para rodar a cada minuto
cron.schedule('* * * * *', runProjectionJob);

// Roda o job uma vez na inicialização
setTimeout(() => {
  console.log("Executando job de projeção inicial...");
  runProjectionJob().catch(err => console.error("Erro no job inicial:", err));
}, 15000); // 15 segundos

// 5. Inicia o servidor Express
app.listen(port, '::', () => {
  console.log(`Analysis Service a rodar e a escutar na porta ${port}`);
});