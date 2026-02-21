// composables/mensalistas/useMensalistas.js
import { storeToRefs } from 'pinia';
import { useMensalistasStore } from '@/stores/mensalistas.store';
import { useMensalistasActions } from './useMensalistasActions';
import { useMensalistasFilters } from './useMensalistasFilters';
import { useMensalistasForm } from './useMensalistasForm';
import { useMensalistasUI } from './useMensalistasUI';
import { useMensalistasDashboard } from './useMensalistasDashboard';
import { useMensalistasRelatorio } from './useMensalistasRelatorio';
import { useMensalistasValidacao } from './useMensalistasValidacao';

export function useMensalistas() {
  const store = useMensalistasStore();
  const storeRefs = storeToRefs(store);

  return {
    // Store refs
    ...storeRefs,
    
    // Composables específicos
    ...useMensalistasActions(store),
    ...useMensalistasFilters(store),
    ...useMensalistasForm(store),
    ...useMensalistasUI(store),
    ...useMensalistasDashboard(store),
    ...useMensalistasRelatorio(store),
    ...useMensalistasValidacao(store),
    
    // Métodos diretos da store
    carregarMensalistas: store.carregarMensalistas.bind(store),
    resetarStore: store.resetarStore.bind(store),
    clearError: store.clearError.bind(store),
  };
}