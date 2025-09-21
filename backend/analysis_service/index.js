const express = require('express');
const { projetarManualmente } = require('./Controllers/analysisController');

// Inicia o agendador de tarefas
require('./jobs/projectionJob');

const app = express();
const PORT = process.env.PORT || 3000;

app.use(express.json());

// Define a rota para a projeção manual
app.post('/projetar', projetarManualmente);

app.listen(PORT, () => {
    console.log(`Analysis Service rodando na porta ${PORT}`);
});