const cron = require('node-cron');
const http = require('http');
const { getAuthToken } = require('./auth.js');
const { fetchOperacoes, updateProjecao } = require('./services/billingService.js');

console.log('Analysis Service: Tarefa de análise agendada.');

// A função principal do job, agora separada para ser reutilizável
async function runProjectionJob() {
  console.log('Analysis Service: Iniciando job de análise...');

  const token = await getAuthToken();
  if (token) {
    const operacoes = await fetchOperacoes(token);

    if (operacoes && operacoes.length > 0) {
      console.log(`Analysis Service: ${operacoes.length} operações encontradas. Calculando e salvando projeções...`);

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
}

// Agenda o job para rodar a cada minuto
cron.schedule('* * * * *', runProjectionJob);

// --- CORREÇÃO AQUI: Adiciona try...catch ao job inicial ---
// Roda o job uma vez na inicialização para popular os dados imediatamente
setTimeout(async () => {
    try {
        console.log("Executando job inicial...");
        await runProjectionJob();
    } catch (error) {
        console.error("Erro durante a execução do job inicial:", error);
        // Mesmo com erro, o serviço principal continua rodando.
    }
}, 10000); // Aumentado para 10 segundos para dar mais tempo aos outros serviços

// Servidor para healthcheck
http.createServer((req, res) => {
  res.writeHead(200, {'Content-Type': 'text/plain'});
  res.end('Analysis Service is running\n');
}).listen(3000);

console.log('Analysis Service rodando na porta 3000');