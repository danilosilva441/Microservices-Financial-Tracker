// src/Services/DashboardCalculator.js

class DashboardCalculator {
    
    getEmptyDashboardData() {
        return {
            kpis: {
                receitaTotal: 0,
                despesaTotal: 0,
                lucroTotal: 0,
                metaTotal: 0,
                percentualMetaTotal: 0,
                unidadesAtivas: 0,
                mediaLucroPorUnidade: 0,
                melhorUnidade: null,
                piorUnidade: null
            },
            desempenho: {
                todas: [],
                resumo: {
                    unidadesComLucro: 0,
                    unidadesComPrejuizo: 0,
                    unidadesAcimaDaMeta: 0
                }
            },
            graficos: {
                barChartData: {
                    labels: [],
                    datasets: [],
                    options: this._getBarChartOptions()
                },
                pieChartData: {
                    labels: [],
                    datasets: [],
                    options: this._getPieChartOptions()
                },
                trendChartData: {
                    labels: [],
                    datasets: [],
                    options: this._getTrendChartOptions()
                }
            },
            period: {
                inicio: new Date().toISOString().split('T')[0],
                fim: new Date().toISOString().split('T')[0]
            }
        };
    }

    calcularDashboardLucro(unidades, dadosPorUnidade) {
        if (!unidades || !Array.isArray(unidades) || unidades.length === 0) {
            return this.getEmptyDashboardData();
        }

        let receitaTotal = 0;
        let despesaTotal = 0;
        let metaTotal = 0;
        
        const unidadesComLucro = [];
        const unidadesComPrejuizo = [];
        const unidadesAcimaDaMeta = [];

        const desempenhoUnidades = unidades.map(unidade => {
            const dados = dadosPorUnidade[unidade.id];
            
            // Se não houver dados para a unidade, usar valores zerados
            if (!dados) {
                console.warn(`DashboardCalculator: Dados não encontrados para unidade ${unidade.id}`);
                return {
                    id: unidade.id,
                    nome: unidade.nome || `Unidade ${unidade.id}`,
                    metaMensal: unidade.metaMensal || 0,
                    receita: 0,
                    despesa: 0,
                    lucro: 0,
                    percentualMeta: 0,
                    status: 'sem_dados',
                    fechamentos: 0,
                    despesas: 0
                };
            }

            // 1. Calcula a Receita (Apenas de fechamentos APROVADOS)
            const fechamentosAprovados = dados.fechamentos.filter(f => 
                f && f.status === "Aprovado" && f.valorTotalParciais
            );
            const receitaUnidade = fechamentosAprovados.reduce((total, f) => 
                total + parseFloat(f.valorTotalParciais || 0), 0
            );

            // 2. Calcula a Despesa
            const despesasValidas = dados.despesas.filter(d => d && d.amount);
            const despesaUnidade = despesasValidas.reduce((total, d) => 
                total + parseFloat(d.amount || 0), 0
            );

            // 3. Calcula o Lucro
            const lucroUnidade = receitaUnidade - despesaUnidade;

            // 4. Calcula percentual da meta
            const metaUnidade = parseFloat(unidade.metaMensal || 0);
            const percentualMeta = metaUnidade > 0 
                ? (receitaUnidade / metaUnidade) * 100 
                : (receitaUnidade > 0 ? 100 : 0);

            // Classificação da unidade
            const status = lucroUnidade > 0 
                ? 'lucro' 
                : (lucroUnidade < 0 ? 'prejuizo' : 'neutro');
            
            if (lucroUnidade > 0) unidadesComLucro.push(unidade.id);
            if (lucroUnidade < 0) unidadesComPrejuizo.push(unidade.id);
            if (percentualMeta >= 100) unidadesAcimaDaMeta.push(unidade.id);

            receitaTotal += receitaUnidade;
            despesaTotal += despesaUnidade;
            metaTotal += metaUnidade;

            return {
                id: unidade.id,
                nome: unidade.nome || `Unidade ${unidade.id}`,
                metaMensal: metaUnidade,
                receita: Math.round(receitaUnidade * 100) / 100,
                despesa: Math.round(despesaUnidade * 100) / 100,
                lucro: Math.round(lucroUnidade * 100) / 100,
                percentualMeta: Math.round(percentualMeta * 100) / 100,
                status,
                fechamentos: fechamentosAprovados.length,
                despesas: despesasValidas.length,
                margemLucro: receitaUnidade > 0 
                    ? Math.round((lucroUnidade / receitaUnidade) * 10000) / 100 
                    : 0
            };
        }).filter(u => u != null);

        const lucroTotal = Math.round((receitaTotal - despesaTotal) * 100) / 100;
        const percentualMetaTotal = metaTotal > 0 
            ? Math.round((receitaTotal / metaTotal) * 10000) / 100 
            : 0;
        
        // Encontrar melhor e pior unidade
        const desempenhoOrdenado = [...desempenhoUnidades].sort((a, b) => b.lucro - a.lucro);
        const melhorUnidade = desempenhoOrdenado[0] || null;
        const piorUnidade = desempenhoOrdenado[desempenhoOrdenado.length - 1] || null;

        // Gráficos
        const barChartData = this._generateBarChartData(desempenhoUnidades);
        const pieChartData = this._generatePieChartData(desempenhoUnidades);
        const trendChartData = this._generateTrendChartData(desempenhoUnidades);

        return {
            kpis: {
                receitaTotal: Math.round(receitaTotal * 100) / 100,
                despesaTotal: Math.round(despesaTotal * 100) / 100,
                lucroTotal,
                metaTotal: Math.round(metaTotal * 100) / 100,
                percentualMetaTotal,
                unidadesAtivas: desempenhoUnidades.length,
                mediaLucroPorUnidade: desempenhoUnidades.length > 0 
                    ? Math.round((lucroTotal / desempenhoUnidades.length) * 100) / 100 
                    : 0,
                melhorUnidade: melhorUnidade ? {
                    id: melhorUnidade.id,
                    nome: melhorUnidade.nome,
                    lucro: melhorUnidade.lucro
                } : null,
                piorUnidade: piorUnidade ? {
                    id: piorUnidade.id,
                    nome: piorUnidade.nome,
                    lucro: piorUnidade.lucro
                } : null
            },
            desempenho: {
                todas: desempenhoUnidades.sort((a, b) => b.lucro - a.lucro),
                resumo: {
                    unidadesComLucro: unidadesComLucro.length,
                    unidadesComPrejuizo: unidadesComPrejuizo.length,
                    unidadesAcimaDaMeta: unidadesAcimaDaMeta.length,
                    totalUnidades: desempenhoUnidades.length
                }
            },
            graficos: {
                barChartData,
                pieChartData,
                trendChartData
            },
            period: {
                inicio: this._getFirstDayOfMonth(),
                fim: new Date().toISOString().split('T')[0],
                mesAtual: new Date().toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' })
            },
            metadata: {
                generatedAt: new Date().toISOString(),
                version: '2.0',
                totalCalculations: desempenhoUnidades.length
            }
        };
    }

    _generateBarChartData(desempenhoUnidades) {
        const labels = desempenhoUnidades.map(u => {
            // Limitar nome para visualização
            return u.nome.length > 15 ? u.nome.substring(0, 12) + '...' : u.nome;
        });

        return {
            labels,
            datasets: [
                {
                    label: 'Receita (Aprovada)',
                    backgroundColor: '#10b981',
                    borderColor: '#059669',
                    borderWidth: 1,
                    data: desempenhoUnidades.map(u => u.receita)
                },
                {
                    label: 'Despesa',
                    backgroundColor: '#ef4444',
                    borderColor: '#dc2626',
                    borderWidth: 1,
                    data: desempenhoUnidades.map(u => u.despesa)
                },
                {
                    label: 'Lucro',
                    backgroundColor: '#3b82f6',
                    borderColor: '#2563eb',
                    borderWidth: 1,
                    data: desempenhoUnidades.map(u => u.lucro)
                }
            ]
        };
    }

    _generatePieChartData(desempenhoUnidades) {
        const unidadesComLucro = desempenhoUnidades.filter(u => u.lucro > 0);
        
        if (unidadesComLucro.length === 0) {
            return {
                labels: ['Sem lucro'],
                datasets: [{
                    data: [100],
                    backgroundColor: ['#94a3b8'],
                    hoverOffset: 4
                }]
            };
        }

        return {
            labels: unidadesComLucro.map(u => u.nome),
            datasets: [{
                data: unidadesComLucro.map(u => u.lucro),
                backgroundColor: [
                    '#10b981', '#3b82f6', '#8b5cf6', '#f59e0b', '#ef4444',
                    '#06b6d4', '#84cc16', '#f97316', '#8b5cf6', '#ec4899'
                ],
                hoverOffset: 4,
                borderWidth: 2,
                borderColor: '#ffffff'
            }]
        };
    }

    _generateTrendChartData(desempenhoUnidades) {
        // Simulação de dados históricos (em um caso real, viria do banco)
        const meses = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun'];
        
        return {
            labels: meses,
            datasets: [
                {
                    label: 'Receita Total',
                    data: meses.map(() => Math.random() * 100000),
                    borderColor: '#10b981',
                    backgroundColor: 'rgba(16, 185, 129, 0.1)',
                    fill: true,
                    tension: 0.4
                },
                {
                    label: 'Lucro Total',
                    data: meses.map(() => Math.random() * 50000),
                    borderColor: '#3b82f6',
                    backgroundColor: 'rgba(59, 130, 246, 0.1)',
                    fill: true,
                    tension: 0.4
                }
            ]
        };
    }

    _getBarChartOptions() {
        return {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'top',
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            label += new Intl.NumberFormat('pt-BR', {
                                style: 'currency',
                                currency: 'BRL'
                            }).format(context.raw);
                            return label;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return 'R$ ' + value.toLocaleString('pt-BR');
                        }
                    }
                }
            }
        };
    }

    _getPieChartOptions() {
        return {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right',
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const label = context.label || '';
                            const value = context.raw || 0;
                            const percentage = context.parsed || 0;
                            
                            return `${label}: R$ ${value.toLocaleString('pt-BR')} (${percentage.toFixed(1)}%)`;
                        }
                    }
                }
            }
        };
    }

    _getTrendChartOptions() {
        return {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'top',
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return 'R$ ' + value.toLocaleString('pt-BR');
                        }
                    }
                }
            }
        };
    }

    _getFirstDayOfMonth() {
        const date = new Date();
        return new Date(date.getFullYear(), date.getMonth(), 1)
            .toISOString()
            .split('T')[0];
    }
}

module.exports = new DashboardCalculator();