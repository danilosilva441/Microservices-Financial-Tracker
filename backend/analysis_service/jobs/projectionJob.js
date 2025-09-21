const cron = require('node-cron');
const { rodarAnaliseAutomatica } = require('../Services/analysisService'); // <--- NOME ATUALIZADO

// Roda a cada minuto para testes
cron.schedule('* * * * *', rodarAnaliseAutomatica);
console.log('Tarefa de análise automática agendada.');