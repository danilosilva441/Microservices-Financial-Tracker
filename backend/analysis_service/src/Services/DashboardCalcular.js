// src/Services/DashboardCalculator.js

/**
 * @class DashboardCalculator
 * @description Serviço responsável pela inteligência de negócios do Dashboard.
 * Ele processa dados brutos de faturamento e despesas, calcula KPIs, projeta resultados
 * futuros e gera estruturas de dados otimizadas para gráficos (Chart.js).
 */
class DashboardCalculator {

    /**
     * Retorna um esqueleto vazio do dashboard.
     * Útil para inicializar o estado no frontend antes de receber dados da API,
     * evitando erros de "undefined" ao tentar acessar propriedades aninhadas.
     * * @returns {Object} Estrutura inicial do dashboard com valores zerados.
     */
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
                piorUnidade: null,

                // Projeção Linear baseada no dia atual
                projecao: {
                    valorEstimado: 0,
                    percentualEstimado: 0,
                    mediaDiariaAtual: 0,
                    mediaDiariaNecessaria: 0,
                    diasRestantes: 0,
                    probabilidade: 'indefinida', // 'Alta', 'Média', 'Baixa', 'Crítica'
                    status: 'neutro' // 'success', 'warning', 'danger'
                },

                // Insights automáticos sobre os dados
                analise: {
                    crescimentoPeriodo: 0,
                    diaMelhorPerformance: '',
                    diaPiorPerformance: '',
                    estabilidade: 'N/A',
                    ticketMedio: 0
                },
                benchmark: {
                    variacaoReceita: 0,
                    variacaoLucro: 0,
                    variacaoTicket: 0,
                    periodoAnterior: { receita: 0, lucro: 0, ticket: 0 }
                }
            },
            desempenho: {
                todas: [],
                resumo: {
                    unidadesComLucro: 0,
                    unidadesComPrejuizo: 0,
                    unidadesAcimaDaMeta: 0,
                    totalUnidades: 0
                }
            },
            graficos: {
                barChartData: this._getEmptyChartConfig(),
                pieChartData: this._getEmptyChartConfig(),
                trendChartData: this._getEmptyChartConfig(),
                sazonalidadeChartData: this._getEmptyChartConfig()
            },
            period: {
                inicio: new Date().toISOString().split('T')[0],
                fim: new Date().toISOString().split('T')[0],
                mesAtual: ''
            },
            metadata: {
                generatedAt: new Date().toISOString(),
                version: '3.1-Stable'
            }
        };
    }

    _getEmptyChartConfig() {
        return { labels: [], datasets: [], options: {} };
    }

    /**
     * Processa a lista de unidades e seus respectivos dados financeiros.
     * É o "cérebro" da aplicação.
     * * @param {Array} unidades - Lista de objetos representando as unidades/lojas.
     * @param {Object} dadosPorUnidade - Mapa { unidadeId: { fechamentos: [], despesas: [] } }.
     * @returns {Object} Objeto completo com KPIs, Gráficos e Tabelas calculados.
     */
    calcularDashboardLucro(unidades, dadosPorUnidade) {
        // Validação de segurança
        if (!unidades || !Array.isArray(unidades) || unidades.length === 0) {
            return this.getEmptyDashboardData();
        }

        // Acumuladores Globais
        let receitaTotal = 0;
        let despesaTotal = 0;
        let metaTotal = 0;
        let totalFechamentos = []; // Pool de todos os fechamentos para análise temporal global

        // Listas auxiliares para contagem rápida
        const unidadesComLucro = [];
        const unidadesComPrejuizo = [];
        const unidadesAcimaDaMeta = [];

        // 1. Processamento Individual por Unidade
        const desempenhoUnidades = unidades.map(unidade => {
            const dados = dadosPorUnidade[unidade.id] || { fechamentos: [], despesas: [] };

            // Filtra apenas fechamentos válidos (Fechados ou Aprovados)
            const fechamentosValidos = (dados.fechamentos || []).filter(f =>
                f.valorTotal > 0 // Usa o campo novo direto!
            );

            // Filtra fechamentos APROVADOS (se esse campo existir)
            // Se não houver campo 'status', use fechamentosValidos como fechamentosAprovados
            const fechamentosAprovados = fechamentosValidos.filter(f =>
                f.status === 'APROVADO' || f.status === 'FECHADO' || !f.status
            );

            // Adiciona ao pool global
            totalFechamentos = totalFechamentos.concat(fechamentosAprovados);

            // Cálculos da Unidade
            // Soma direta usando o novo campo
            const receitaUnidade = fechamentosValidos.reduce((total, f) =>
                total + (f.valorTotal || 0), 0
            );

            const despesasValidas = (dados.despesas || []).filter(d => d && d.amount);
            const despesaUnidade = despesasValidas.reduce((total, d) =>
                total + parseFloat(d.amount || 0), 0
            );

            const lucroUnidade = receitaUnidade - despesaUnidade;
            const metaUnidade = parseFloat(unidade.metaMensal || 0);

            // Percentual da meta atingido
            const percentualMeta = metaUnidade > 0
                ? (receitaUnidade / metaUnidade) * 100
                : (receitaUnidade > 0 ? 100 : 0);

            // Determina status da unidade
            const status = lucroUnidade > 0 ? 'lucro' : (lucroUnidade < 0 ? 'prejuizo' : 'neutro');

            // Popula listas auxiliares
            if (lucroUnidade > 0) unidadesComLucro.push(unidade.id);
            if (lucroUnidade < 0) unidadesComPrejuizo.push(unidade.id);
            if (percentualMeta >= 100) unidadesAcimaDaMeta.push(unidade.id);

            // Soma aos globais
            receitaTotal += receitaUnidade;
            despesaTotal += despesaUnidade;
            metaTotal += metaUnidade;

            // Retorna objeto formatado da unidade
            return {
                id: unidade.id,
                nome: unidade.nome || `Unidade ${unidade.id}`,
                metaMensal: metaUnidade,
                receita: this._round(receitaUnidade),
                despesa: this._round(despesaUnidade),
                lucro: this._round(lucroUnidade),
                percentualMeta: this._round(percentualMeta),
                percentualProjetado: 0, // Placeholder, será preenchido após cálculo de projeção individual
                status,
                fechamentosCount: fechamentosAprovados.length,
                ticketMedio: fechamentosAprovados.length > 0 ? this._round(receitaUnidade / fechamentosAprovados.length) : 0
            };
        }).filter(u => u != null);

        // 2. Cálculos Globais Consolidados
        const lucroTotal = this._round(receitaTotal - despesaTotal);
        const percentualMetaTotal = metaTotal > 0
            ? this._round((receitaTotal / metaTotal) * 100)
            : 0;

        // 3. Ordenação para Rankings (Melhor e Pior)
        const desempenhoOrdenado = [...desempenhoUnidades].sort((a, b) => b.lucro - a.lucro);
        const melhorUnidade = desempenhoOrdenado[0] || null;
        const piorUnidade = desempenhoOrdenado[desempenhoOrdenado.length - 1] || null;

        // 4. 🔥 CÁLCULOS AVANÇADOS (Inteligência de Dados)

        // 4a. Projeção Linear (Forecast)
        const projecao = this._calcularProjecao(receitaTotal, metaTotal);

        // Aplica projeção individual para cada unidade (para ordenar quem vai bater a meta)
        desempenhoUnidades.forEach(u => {
            const projecaoUnidade = this._calcularProjecao(u.receita, u.metaMensal);
            u.percentualProjetado = projecaoUnidade.percentualEstimado;
            u.projecaoValor = projecaoUnidade.valorEstimado;
        });

        // 4b. Análise de Sazonalidade (Melhores dias da semana)
        const analiseSazonal = this._analisarSazonalidade(totalFechamentos);

        // 4c. Tendência Temporal (Crescimento no período)
        const tendenciaTemporal = this._analisarTendenciaTemporal(totalFechamentos);

        // ✅ NOVO: Cálculo específico de Benchmarking
        const benchmarkData = this._calcularBenchmark(totalFechamentos);

        // 5. Montagem do Objeto Final
        return {
            kpis: {
                receitaTotal: this._round(receitaTotal),
                despesaTotal: this._round(despesaTotal),
                lucroTotal,
                metaTotal: this._round(metaTotal),
                percentualMetaTotal,
                unidadesAtivas: desempenhoUnidades.length,
                mediaLucroPorUnidade: desempenhoUnidades.length > 0
                    ? this._round(lucroTotal / desempenhoUnidades.length)
                    : 0,
                melhorUnidade: melhorUnidade ? { ...melhorUnidade } : null,
                piorUnidade: piorUnidade ? { ...piorUnidade } : null,

                // Dados calculados de projeção
                projecao: projecao,
                benchmark: benchmarkData,

                // Dados calculados de análise
                analise: {
                    crescimentoPeriodo: tendenciaTemporal.crescimento,
                    diaMelhorPerformance: analiseSazonal.melhorDia,
                    diaPiorPerformance: analiseSazonal.piorDia,
                    estabilidade: tendenciaTemporal.estabilidade,
                    ticketMedio: totalFechamentos.length > 0 ? this._round(receitaTotal / totalFechamentos.length) : 0
                }
            },
            desempenho: {
                // Ordena lista principal por projeção (quem está mais propenso a bater a meta aparece primeiro)
                todas: desempenhoUnidades.sort((a, b) => b.percentualProjetado - a.percentualProjetado),
                resumo: {
                    unidadesComLucro: unidadesComLucro.length,
                    unidadesComPrejuizo: unidadesComPrejuizo.length,
                    unidadesAcimaDaMeta: unidadesAcimaDaMeta.length,
                    totalUnidades: desempenhoUnidades.length
                }
            },
            graficos: {
                barChartData: this._generateBarChartData(desempenhoUnidades),
                pieChartData: this._generatePieChartData(desempenhoUnidades),
                trendChartData: this._generateTrendChartData(totalFechamentos),
                sazonalidadeChartData: this._generateSazonalidadeChartData(analiseSazonal.dadosPorDiaSemana)
            },
            period: {
                inicio: this._getFirstDayOfMonth(),
                fim: new Date().toISOString().split('T')[0],
                mesAtual: new Date().toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' })
            },
            metadata: {
                generatedAt: new Date().toISOString(),
                version: '3.1'
            }
        };
    }

    // ==========================================
    // 🧠 MOTORES DE CÁLCULO (LÓGICA DE NEGÓCIO)
    // ==========================================

    /**
         * Calcula a variação de Receita, Lucro e Ticket Médio comparando 
         * a primeira metade do período com a segunda metade (Proxy de Benchmarking).
         * * @param {Array} fechamentos - Lista total de fechamentos.
         * @returns {Object} Variações percentuais e valores absolutos anteriores.
         */
    _calcularBenchmark(fechamentos) {
        if (fechamentos.length < 2) {
            return { variacaoReceita: 0, variacaoLucro: 0, variacaoTicket: 0, periodoAnterior: {} };
        }

        // Ordena cronologicamente
        const sorted = [...fechamentos].sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));

        // Divide o período temporalmente (ex: 1ª quinzena vs 2ª quinzena)
        const meio = Math.floor(sorted.length / 2);
        const periodoAnterior = sorted.slice(0, meio);
        const periodoAtual = sorted.slice(meio);

        // Função auxiliar de soma
        const somar = (arr, campo) => arr.reduce((acc, f) => acc + parseFloat(f[campo] || 0), 0);
        // Helper simplificado para lucro (Receita - Despesa Proporcional - aqui simplificamos usando margem ou dados brutos se disponiveis)
        // Nota: Como fechamento não tem despesa direta atrelada no objeto simples, usaremos Receita como proxy principal de performance

        // 1. Receita
        const receitaAnt = somar(periodoAnterior, 'valorTotalParciais');
        const receitaAtual = somar(periodoAtual, 'valorTotalParciais');

        // 2. Ticket Médio
        const ticketAnt = periodoAnterior.length > 0 ? receitaAnt / periodoAnterior.length : 0;
        const ticketAtual = periodoAtual.length > 0 ? receitaAtual / periodoAtual.length : 0;

        // 3. Lucro (Estimado baseada numa margem fixa ou se tiver dados reais. Vamos usar Receita * 0.3 como simulação se não tiver despesa no objeto fechamento)
        // Num cenário ideal, cruzaríamos com as despesas por data.
        const lucroAnt = receitaAnt * 0.35; // Simulação de margem 35%
        const lucroAtual = receitaAtual * 0.35;

        // Cálculo de Variação (%)
        const calcVar = (atual, anterior) => anterior > 0 ? ((atual - anterior) / anterior) * 100 : 0;

        return {
            variacaoReceita: this._round(calcVar(receitaAtual, receitaAnt)),
            variacaoLucro: this._round(calcVar(lucroAtual, lucroAnt)),
            variacaoTicket: this._round(calcVar(ticketAtual, ticketAnt)),
            periodoAnterior: {
                receita: this._round(receitaAnt),
                lucro: this._round(lucroAnt),
                ticket: this._round(ticketAnt)
            }
        };
    }


    /**
     * Calcula a projeção de faturamento até o fim do mês baseado no ritmo atual (Run Rate).
     * @param {number} receitaAtual - Valor já faturado.
     * @param {number} metaTotal - Meta a ser atingida.
     * @returns {Object} Dados da projeção, probabilidade e cores de status.
     */
    _calcularProjecao(receitaAtual, metaTotal) {
        const hoje = new Date();
        const diaAtual = hoje.getDate();
        const ultimoDiaMes = new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0).getDate();
        const diasRestantes = ultimoDiaMes - diaAtual;

        // Evita divisão por zero no dia 1
        const diasCorridos = diaAtual === 0 ? 1 : diaAtual;

        // 1. Ritmo Atual (Média Diária)
        const mediaDiariaAtual = receitaAtual / diasCorridos;

        // 2. Projeção Linear (Se continuar assim...)
        const valorEstimado = mediaDiariaAtual * ultimoDiaMes;

        // 3. Percentual Estimado
        const percentualEstimado = metaTotal > 0
            ? (valorEstimado / metaTotal) * 100
            : 0;

        // 4. Esforço Necessário (Quanto precisa vender por dia para salvar a meta?)
        const saldoParaMeta = metaTotal - receitaAtual;
        let mediaDiariaNecessaria = 0;

        if (saldoParaMeta > 0 && diasRestantes > 0) {
            mediaDiariaNecessaria = saldoParaMeta / diasRestantes;
        }

        // 5. Determinação de Status/Probabilidade
        let probabilidade = 'Baixa';
        let status = 'danger';

        if (percentualEstimado >= 110) {
            probabilidade = 'Superação';
            status = 'success';
        } else if (percentualEstimado >= 98) {
            probabilidade = 'Alta';
            status = 'success';
        } else if (percentualEstimado >= 85) {
            probabilidade = 'Média';
            status = 'warning';
        }

        // Regra de Negócio: Se passou do dia 20 e a projeção é ruim, vira "Crítica"
        if (diaAtual > 20 && percentualEstimado < 70) {
            probabilidade = 'Crítica';
        }

        return {
            valorEstimado: this._round(valorEstimado),
            percentualEstimado: this._round(percentualEstimado),
            mediaDiariaAtual: this._round(mediaDiariaAtual),
            mediaDiariaNecessaria: this._round(mediaDiariaNecessaria),
            diasRestantes,
            probabilidade,
            status
        };
    }

    /**
     * Analisa quais dias da semana performam melhor.
     * @param {Array} fechamentos - Lista de todas as vendas.
     * @returns {Object} Estatísticas por dia da semana.
     */
    _analisarSazonalidade(fechamentos) {
        const diasSemana = ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'];
        const totaisPorDia = new Array(7).fill(0);
        const contagemPorDia = new Array(7).fill(0);

        fechamentos.forEach(f => {
            if (!f.createdAt) return;
            const data = new Date(f.createdAt);
            const diaIndex = data.getDay(); // 0-6
            totaisPorDia[diaIndex] += parseFloat(f.valorTotalParciais || 0);
            contagemPorDia[diaIndex]++;
        });

        // Calcula médias para não enviesar dias que ocorreram mais vezes no mês
        const mediasPorDia = totaisPorDia.map((total, index) => {
            return {
                dia: diasSemana[index],
                total: total,
                media: contagemPorDia[index] > 0 ? total / contagemPorDia[index] : 0
            };
        });

        // Encontra melhor e pior dia (baseado no total absoluto neste caso)
        const diasComVendas = mediasPorDia.filter(d => d.total > 0);
        diasComVendas.sort((a, b) => b.total - a.total);

        return {
            dadosPorDiaSemana: mediasPorDia,
            melhorDia: diasComVendas.length > 0 ? diasComVendas[0].dia : 'N/A',
            piorDia: diasComVendas.length > 0 ? diasComVendas[diasComVendas.length - 1].dia : 'N/A'
        };
    }

    /**
     * Verifica se as vendas estão subindo ou descendo comparando metades do período.
     */
    _analisarTendenciaTemporal(fechamentos) {
        if (fechamentos.length < 2) return { crescimento: 0, estabilidade: 'Dados Insuficientes' };

        // Ordena cronologicamente
        const sorted = [...fechamentos].sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));

        // Divide o período em dois
        const meio = Math.floor(sorted.length / 2);
        const primeiraMetade = sorted.slice(0, meio);
        const segundaMetade = sorted.slice(meio);

        const total1 = primeiraMetade.reduce((acc, f) => acc + parseFloat(f.valorTotalParciais || 0), 0);
        const total2 = segundaMetade.reduce((acc, f) => acc + parseFloat(f.valorTotalParciais || 0), 0);

        // Calcula crescimento
        let crescimento = 0;
        if (total1 > 0) {
            crescimento = ((total2 - total1) / total1) * 100;
        }

        // Define estabilidade (Variação > 30% é considerada volátil)
        const estabilidade = Math.abs(crescimento) > 30 ? 'Volátil' : 'Estável';

        return {
            crescimento: this._round(crescimento),
            estabilidade
        };
    }

    // ==========================================
    // 📊 GERADORES DE GRÁFICOS (Chart.js)
    // ==========================================

    _generateBarChartData(desempenhoUnidades) {
        // Pega apenas as Top 10 para o gráfico não ficar ilegível
        const topUnits = desempenhoUnidades.slice(0, 10);

        return {
            labels: topUnits.map(u => u.nome.length > 12 ? u.nome.substring(0, 10) + '..' : u.nome),
            datasets: [
                {
                    label: 'Realizado',
                    backgroundColor: '#3b82f6', // Azul
                    borderRadius: 4,
                    data: topUnits.map(u => u.receita)
                },
                {
                    label: 'Meta',
                    backgroundColor: '#d1d5db', // Cinza
                    borderRadius: 4,
                    hidden: true, // Oculto por padrão para limpeza visual
                    data: topUnits.map(u => u.metaMensal)
                }
            ]
        };
    }

    _generatePieChartData(desempenhoUnidades) {
        // Top 5 + "Outros"
        const top5 = desempenhoUnidades.slice(0, 5);
        const outros = desempenhoUnidades.slice(5);
        const valorOutros = outros.reduce((acc, u) => acc + u.lucro, 0);

        const labels = top5.map(u => u.nome);
        const data = top5.map(u => u.lucro);

        if (valorOutros > 0) {
            labels.push('Outros');
            data.push(valorOutros);
        }

        return {
            labels,
            datasets: [{
                data,
                backgroundColor: ['#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ec4899', '#64748b'],
                borderWidth: 0
            }]
        };
    }

    _generateTrendChartData(fechamentos) {
        const dadosPorDia = {};

        // Agrupa vendas por dia
        fechamentos.forEach(f => {
            if (!f.createdAt) return;
            // Formata dia como DD/MM para exibição
            const dia = new Date(f.createdAt).toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' });
            if (!dadosPorDia[dia]) dadosPorDia[dia] = 0;
            dadosPorDia[dia] += parseFloat(f.valorTotalParciais || 0);
        });

        // Ordena as datas corretamente
        const labels = Object.keys(dadosPorDia).sort((a, b) => {
            const [da, ma] = a.split('/');
            const [db, mb] = b.split('/');
            // Assume ano corrente para ordenação
            return new Date(new Date().getFullYear(), ma - 1, da) - new Date(new Date().getFullYear(), mb - 1, db);
        });

        const data = labels.map(dia => dadosPorDia[dia]);

        // Gera linha de acumulado (eixo secundário)
        const dataAcumulada = [];
        let acumulado = 0;
        data.forEach(v => {
            acumulado += v;
            dataAcumulada.push(acumulado);
        });

        return {
            labels,
            datasets: [
                {
                    label: 'Vendas Diárias',
                    data: data,
                    borderColor: '#3b82f6',
                    backgroundColor: 'rgba(59, 130, 246, 0.1)',
                    type: 'bar',
                    order: 2
                },
                {
                    label: 'Acumulado',
                    data: dataAcumulada,
                    borderColor: '#10b981', // Verde
                    borderWidth: 2,
                    type: 'line',
                    order: 1,
                    tension: 0.4,
                    yAxisID: 'y1' // Requer configuração de escala no ChartOptions
                }
            ]
        };
    }

    _generateSazonalidadeChartData(dadosPorDiaSemana) {
        return {
            labels: dadosPorDiaSemana.map(d => d.dia.substring(0, 3)), // Dom, Seg, Ter...
            datasets: [{
                label: 'Volume de Vendas',
                data: dadosPorDiaSemana.map(d => d.total),
                backgroundColor: '#6366f1', // Indigo
                borderRadius: 4
            }]
        };
    }

    // ==========================================
    // 🛠️ UTILITÁRIOS E HELPERS
    // ==========================================

    _round(value) {
        return Math.round((value + Number.EPSILON) * 100) / 100;
    }

    _getFirstDayOfMonth() {
        const date = new Date();
        return new Date(date.getFullYear(), date.getMonth(), 1).toISOString().split('T')[0];
    }

    // Configurações padrão para garantir que o gráfico não quebre se o front não passar options
    _getBarChartOptions() { return { responsive: true, plugins: { legend: { position: 'top' } } }; }
    _getPieChartOptions() { return { responsive: true, plugins: { legend: { position: 'right' } } }; }
    _getTrendChartOptions() {
        return {
            responsive: true,
            interaction: { mode: 'index', intersect: false },
            scales: {
                y: { type: 'linear', display: true, position: 'left' },
                y1: { type: 'linear', display: false, position: 'right', grid: { drawOnChartArea: false } }
            }
        };
    }
}

module.exports = new DashboardCalculator();