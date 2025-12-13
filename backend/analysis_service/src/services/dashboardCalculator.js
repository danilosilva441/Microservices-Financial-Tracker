// src/Services/DashboardCalculator.js

class DashboardCalculator {
    
    getEmptyDashboardData() {
        return {
            kpis: {
                receitaTotal: 0, despesaTotal: 0, lucroTotal: 0, metaTotal: 0, percentualMetaTotal: 0
            },
            desempenho: { todas: [] },
            graficos: {
                barChartData: { labels: [], datasets: [] },
                pieChartData: { labels: [], datasets: [] }
            }
        };
    }

    calcularDashboardLucro(unidades, dadosPorUnidade) {
        if (!unidades || unidades.length === 0) {
            return this.getEmptyDashboardData();
        }

        let receitaTotal = 0;
        let despesaTotal = 0;
        let metaTotal = 0;

        const desempenhoUnidades = unidades.map(unidade => {
            const dados = dadosPorUnidade[unidade.id];
            if (!dados) return null;

            // 1. Calcula a Receita (Apenas de fechamentos APROVADOS)
            const receitaUnidade = dados.fechamentos
                .filter(f => f.status === "Aprovado")
                .reduce((total, f) => total + f.valorTotalParciais, 0);

            // 2. Calcula a Despesa
            const despesaUnidade = dados.despesas
                .reduce((total, d) => total + d.amount, 0);

            // 3. Calcula o Lucro
            const lucroUnidade = receitaUnidade - despesaUnidade;

            receitaTotal += receitaUnidade;
            despesaTotal += despesaUnidade;
            metaTotal += unidade.metaMensal;

            return {
                id: unidade.id,
                nome: unidade.nome,
                metaMensal: unidade.metaMensal,
                receita: receitaUnidade,
                despesa: despesaUnidade,
                lucro: lucroUnidade,
                percentualMeta: unidade.metaMensal > 0 ? (receitaUnidade / unidade.metaMensal) * 100 : 0
            };
        }).filter(u => u != null);

        const lucroTotal = receitaTotal - despesaTotal;
        const percentualMetaTotal = metaTotal > 0 ? (receitaTotal / metaTotal) * 100 : 0;

        // Gráficos v2.0
        const barChartData = {
            labels: desempenhoUnidades.map(u => u.nome),
            datasets: [
                { label: 'Receita (Aprovada)', backgroundColor: '#34d399', data: desempenhoUnidades.map(u => u.receita) },
                { label: 'Despesa', backgroundColor: '#f87171', data: desempenhoUnidades.map(u => u.despesa) },
                { label: 'Lucro', backgroundColor: '#38bdf8', data: desempenhoUnidades.map(u => u.lucro) }
            ]
        };

        const pieChartData = {
            labels: desempenhoUnidades.map(u => u.nome),
            datasets: [{
                backgroundColor: ['#4ade80', '#38bdf8', '#f87171', '#fbbf24', '#a78bfa'],
                data: desempenhoUnidades.map(u => u.lucro > 0 ? u.lucro : 0)
            }]
        };

        return {
            kpis: { receitaTotal, despesaTotal, lucroTotal, metaTotal, percentualMetaTotal },
            desempenho: { todas: desempenhoUnidades.sort((a, b) => b.lucro - a.lucro) },
            graficos: { barChartData, pieChartData }
        };
    }
}

module.exports = new DashboardCalculator();