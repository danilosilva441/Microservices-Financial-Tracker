// src/routes/dashboardRoutes.js
const express = require('express');
const router = express.Router();
const dashboardController = require('../Controllers/DashboardController');

// Rota: GET /api/analysis/dashboard-data
router.get('/dashboard-data', dashboardController.getDashboardData.bind(dashboardController));

module.exports = router;