<!-- src/views/unidades/NovaUnidadeView.vue - CORRETO: -->
<template>
  <div class="nova-unidades">
    <NovaUnidade />
  </div>
</template>

<script setup>
// Apenas importações - NÃO export default!
import NovaUnidade from '@/components/unidades/NovaUnidade.vue';

// Se precisar definir opções do componente:
defineOptions({
  name: 'NovaUnidadeView' // Nome diferente para evitar conflito
});
</script>

<style scoped>
.nova-unidades {
  padding: 2rem;
}
</style>