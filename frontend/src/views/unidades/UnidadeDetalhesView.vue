<!-- src/views/unidades/UnidadeDetalhesView.vue - CORRETO: -->
<template>
  <div class="unidades-detalhes">
    <UnidadeDetalhes />
  </div>
</template>

<script setup>
// Apenas importações - NÃO export default!
import UnidadeDetalhes from '@/components/unidades/UnidadeDetalhes.vue';

// Se precisar definir opções do componente:
defineOptions({
  name: 'UnidadeDetalhesView' // Nome diferente para evitar conflito
});
</script>

<style scoped>
.unidades-detalhes {
  padding: 2rem;
}
</style>