<!-- src/views/unidades/EditarUnidadeView.vue - CORRETO: -->
<template>
  <div class="edita-unidades">
    <EditarUnidade />
  </div>
</template>

<script setup>
// Apenas importações - NÃO export default!
import EditarUnidade from '@/components/unidades/EditarUnidade.vue';

// Se precisar definir opções do componente:
defineOptions({
  name: 'EditarUnidadeView' // Nome diferente para evitar conflito
});
</script>

<style scoped>
.edita-unidades {
  padding: 2rem;
}
</style>