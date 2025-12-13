// src/Config/database.js
const { Pool } = require('pg');

const pool = new Pool({
    connectionString: process.env.DATABASE_URL,
});

// Evento para logar conexão
pool.on('connect', () => {
    // console.log('Base de Dados conectada com sucesso!');
});

module.exports = {
    query: (text, params) => pool.query(text, params),
};