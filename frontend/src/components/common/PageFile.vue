<template>
  <section class="mx-auto w-full max-w-6xl">
    <!-- Cabeçalho da página -->
    <div class="mb-4 md:mb-6">
      <h1 class="text-2xl md:text-3xl font-bold">{{ title }}</h1>
      <p v-if="subtitle" class="text-sm md:text-base text-gray-600 mt-1">
        {{ subtitle }}
      </p>
    </div>

    <!-- “Arquivo” -->
    <div class="rounded-2xl bg-white border border-gray-200 shadow-sm">
      <div class="p-4 md:p-6">
        <slot />
      </div>
    </div>
  </section>
</template>

<script setup>
/**
 * PageFile:
 * - padroniza o visual de todas as páginas do painel
 * - evita repetição de layout e CSS
 */
defineProps({
  title: { type: String, required: true },
  subtitle: { type: String, default: '' },
})
</script>
