// Caminho: backend/analysis_service/src/index.js
const express = require('express');
const cors = require('cors');
// O 'cron' e a lógica de projeção v1.0 foram removidos.

// Importa o router do dashboard v2.0
const dashboardRoutes = require('./routes/dashboardRoutes.js');

// 1. Inicializa o Express
const app = express();
const port = process.env.PORT || 3000;

// 2. Configura o CORS (permite qualquer origem)
app.use(cors()); 

// 3. Rota para o Healthcheck do Docker
app.get('/health', (req, res) => {
    res.status(200).send('OK');
});

// 4. Diz ao Express para usar as novas rotas do dashboard
// (O Nginx roteia /api/analysis/* para cá)
app.use('/api/analysis', dashboardRoutes);

// 5. Inicia o servidor Express
app.listen(port, '::', () => {
    console.log(`Analysis Service (v2.0) a rodar e a escutar na porta ${port}`);
    
    // O job (cron) v1.0 foi removido.
    // O script create-system-user.sh (do Dockerfile) ainda roda na inicialização.
});