<!-- src/views/unidades/UnidadesIndexView.vue - CORRETO: -->
<template>
  <div class="unidades-index">
    <UnidadeIndex />
  </div>
</template>

<script setup>
// Apenas importações - NÃO export default!
import UnidadeIndex from '@/components/unidades/UnidadesIndex.vue';

// Se precisar definir opções do componente:
defineOptions({
  name: 'UnidadesIndexView' // Nome diferente para evitar conflito
});
</script>

<style scoped>
.unidades-index {
  padding: 2rem;
}
</style>