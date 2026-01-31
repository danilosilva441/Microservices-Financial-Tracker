// composables/faturamento/useFechamentoCaixa.js
import { ref, computed } from 'vue';
import { useFaturamentoStore } from '@/stores/faturamento';
import { useAuthStore } from '@/stores/authStore';

export function useFechamentoCaixa(unidadeId) {
    const store = useFaturamentoStore();
    const authStore = useAuthStore();
    // Estados reativos
    const isLoading = ref(false);
    const isModalOpen = ref(false);

    // Obter nome do usuário logado
    const getUsuarioLogado = () => {
        if (!authStore || !authStore.user) {
            console.warn('AuthStore ou usuário não disponível');
            return {
                nome: 'Sistema',
                id: null,
                email: null
            };
        }

        const user = authStore.user;
        let nomeUsuario = 'Operador';

        if (user.name) {
            nomeUsuario = user.name;
        } else if (user.nome) {
            nomeUsuario = user.nome;
        } else if (user.username) {
            nomeUsuario = user.username;
        } else if (user.email) {
            nomeUsuario = user.email.split('@')[0];
        } else if (user.sub) {
            nomeUsuario = user.sub;
        }

        return {
            nome: nomeUsuario,
            id: user.nameid || user.id || null,
            email: user.email || null,
            roles: user.roles || []
        };
    };

    // Função para extrair dados da resposta da API (mais robusta)
    const extrairDadosDaResposta = (responseData) => {
        console.log('DEBUG - Dados brutos da API:', responseData);

        // Se for null/undefined
        if (!responseData) {
            console.log('DEBUG - Dados vazios');
            return [];
        }

        // Se tiver propriedade 'data', verifica o que há dentro
        if (responseData.data !== undefined) {
            const dados = responseData.data;
            console.log('DEBUG - Dados em data:', dados);

            // Se dados for um array, retorna
            if (Array.isArray(dados)) {
                console.log('DEBUG - É um array:', dados.length);
                return dados;
            }

            // Se for um objeto, tenta encontrar arrays dentro
            if (typeof dados === 'object') {
                console.log('DEBUG - É um objeto, keys:', Object.keys(dados));

                // Procura por propriedades que são arrays
                for (const key in dados) {
                    if (Array.isArray(dados[key])) {
                        console.log('DEBUG - Encontrou array em:', key, dados[key].length);
                        return dados[key];
                    }
                }

                // Se não encontrou array, converte valores para array
                const valores = Object.values(dados);
                console.log('DEBUG - Convertendo valores para array:', valores.length);
                return valores;
            }
        }

        // Se já for array
        if (Array.isArray(responseData)) {
            console.log('DEBUG - Já é um array:', responseData.length);
            return responseData;
        }

        // Se for objeto, converte valores
        if (typeof responseData === 'object') {
            const valores = Object.values(responseData);
            console.log('DEBUG - Convertendo objeto para array:', valores.length);
            return valores;
        }

        console.log('DEBUG - Formato desconhecido, retornando array vazio');
        return [];
    };

    // Buscar dados iniciais
    const carregarDados = async () => {
        isLoading.value = true;
        try {
            console.log('Carregando dados para unidade:', unidadeId);
            await store.fetchFechamentos(unidadeId);
            await store.fetchParciais(unidadeId);
            console.log('Dados carregados com sucesso');
        } catch (error) {
            console.error('Erro ao carregar dados do caixa:', error);
        } finally {
            isLoading.value = false;
        }
    };

    // Chamar na inicialização
    carregarDados();

    // Computed values com logging para debug
    const fechamentos = computed(() => {
        const dados = store.fechamentos;
        console.log('DEBUG fechamentos store:', dados);
        const extraidos = extrairDadosDaResposta(dados);
        console.log('DEBUG fechamentos extraídos:', extraidos);
        return extraidos;
    });

    const faturamentosHoje = computed(() => {
        const dados = store.faturamentosParciais;
        console.log('DEBUG parciais store:', dados);
        const extraidos = extrairDadosDaResposta(dados);
        console.log('DEBUG parciais extraídos:', extraidos);
        return extraidos;
    });

    const resumoHoje = computed(() => {
        const parciais = faturamentosHoje.value;

        if (!Array.isArray(parciais)) {
            console.warn('faturamentosHoje não é um array:', parciais);
            return {
                total: 0,
                quantidade: 0,
                primeiro: null,
                ultimo: null
            };
        }

        console.log('DEBUG parciais para resumo:', parciais.length, 'itens');

        const total = parciais.reduce((sum, item) => {
            const valor = parseFloat(item.valor) || 0;
            console.log('DEBUG item valor:', item.valor, '->', valor);
            return sum + valor;
        }, 0);

        console.log('DEBUG total calculado:', total);

        return {
            total,
            quantidade: parciais.length,
            primeiro: parciais[0]?.dataHora || parciais[0]?.horaInicio || null,
            ultimo: parciais[parciais.length - 1]?.dataHora || parciais[parciais.length - 1]?.horaInicio || null
        };
    });
    // Expose estados e ações
    // Ações
    const realizarFechamento = async (dadosAdicionais = {}) => {
        try {
            const usuario = getUsuarioLogado();

            console.log('Realizando fechamento como usuário:', usuario);

            const fechamentoData = {
                data: new Date().toISOString().split('T')[0],
                observacoes: dadosAdicionais.observacoes || '',
                responsavel: usuario.nome,
                fechadoPorUserId: usuario.id,
                // Campos adicionais para compatibilidade
                usuarioNome: usuario.nome,
                emailResponsavel: usuario.email
            };

            const criarResult = await store.criarFechamento(unidadeId, fechamentoData);

            if (!criarResult.success) {
                throw new Error(criarResult.error);
            }

            const fechamentoId = criarResult.data.id;

            // Se você tiver uma ação específica para "fechar o dia", use:
            const fecharResult = await store.fecharDia(unidadeId, fechamentoId);

            if (!fecharResult.success) {
                throw new Error(fecharResult.error);
            }

            await carregarDados();
            isModalOpen.value = false;

            return {
                success: true,
                fechamentoId,
                responsavel: usuario.nome
            };
        } catch (error) {
            console.error('Erro ao realizar fechamento:', error);
            return {
                success: false,
                error: error.message || 'Erro ao realizar fechamento'
            };
        }
    };

    const adicionarFaturamento = async (dados) => {
        try {
            const usuario = getUsuarioLogado();

            console.log('Adicionando faturamento como usuário:', usuario);

            const dadosFormatados = {
                ...dados,
                unidadeId,
                dataHora: dados.dataHora || new Date().toISOString(),
                responsavel: usuario.nome,
                criadoPorUserId: usuario.id,
                // Campos para compatibilidade
                usuarioNome: usuario.nome,
                criadoPorNome: usuario.nome,
                emailUsuario: usuario.email
            };

            const result = await store.criarParcial(unidadeId, dadosFormatados);

            if (result.success) {
                await store.fetchParciais(unidadeId);
            }

            return {
                ...result,
                responsavel: usuario.nome
            };
        } catch (error) {
            console.error('Erro ao adicionar faturamento:', error);
            return {
                success: false,
                error: error.message,
                responsavel: 'Erro'
            };
        }
    };

    const removerFaturamento = async (faturamentoId) => {
        try {
            const usuario = getUsuarioLogado();
            console.log(`Removendo faturamento ${faturamentoId} como usuário:`, usuario);

            const result = await store.deleteParcial(unidadeId, faturamentoId);

            return {
                ...result,
                responsavel: usuario.nome
            };
        } catch (error) {
            console.error('Erro ao remover faturamento:', error);
            return {
                success: false,
                error: error.message
            };
        }
    };

    const obterDetalhesFechamento = async (fechamentoId) => {
        try {
            const usuario = getUsuarioLogado();
            console.log(`Buscando detalhes do fechamento ${fechamentoId} para usuário:`, usuario);

            const detalhes = await store.fetchFechamentoById(unidadeId, fechamentoId);

            if (!detalhes) return null;

            const dadosExtraidos = extrairDadosDaResposta(detalhes);

            return {
                ...dadosExtraidos,
                // Informações do usuário atual
                usuarioAtual: usuario,
                // Garantir que lancamentos seja um array
                lancamentos: Array.isArray(dadosExtraidos.lancamentos)
                    ? dadosExtraidos.lancamentos
                    : dadosExtraidos.itens || [],
                // Garantir que assinaturas seja um array
                assinaturas: Array.isArray(dadosExtraidos.assinaturas)
                    ? dadosExtraidos.assinaturas
                    : [
                        {
                            funcao: 'Fechamento',
                            nome: dadosExtraidos.responsavel || usuario.nome,
                            hash: dadosExtraidos.hashFechamento || `hash_${fechamentoId}`,
                            dataHora: dadosExtraidos.dataFechamento,
                            realizadoPorUsuarioAtual: dadosExtraidos.responsavel === usuario.nome
                        }
                    ],
                // Garantir que logAcoes seja um array
                logAcoes: Array.isArray(dadosExtraidos.logAcoes)
                    ? dadosExtraidos.logAcoes
                    : [
                        {
                            acao: 'Fechamento caixa',
                            usuario: dadosExtraidos.responsavel || usuario.nome,
                            dataHora: dadosExtraidos.dataFechamento,
                            detalhes: `Total: ${dadosExtraidos.valorTotal || 0}`,
                            realizadoPorUsuarioAtual: dadosExtraidos.responsavel === usuario.nome
                        }
                    ]
            };
        } catch (error) {
            console.error('Erro ao obter detalhes:', error);
            return null;
        }
    };

    return {
        // Estados
        fechamentos,
        faturamentosHoje,
        resumoHoje,
        isLoading: computed(() => isLoading.value || store.isLoading),
        isModalOpen,

        // Ações
        realizarFechamento,
        adicionarFaturamento,
        removerFaturamento,
        obterDetalhesFechamento,
        recarregarDados: carregarDados,

        // Utilitário para obter usuário atual
        getUsuarioAtual: getUsuarioLogado,
        // Verifica se é admin
        isAdmin: computed(() => authStore.isAdmin),
        // Estado de autenticação
        isAuthenticated: computed(() => authStore.isAuthenticated)
    };
}