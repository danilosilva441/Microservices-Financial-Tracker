// src/Models/DailyMetric.js
class DailyMetric {
    constructor(id, tenantId, date, totalRevenue) {
        this.id = id;
        this.tenantId = tenantId;
        this.date = date;
        this.totalRevenue = totalRevenue;
    }
}

module.exports = DailyMetric;