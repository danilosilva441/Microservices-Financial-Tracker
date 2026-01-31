<template>
  <PageFile
    title="Unidades"
    subtitle="Lista de unidades com metas, status e acesso por perfil."
  >
    <div class="flex items-center justify-between gap-3 flex-wrap">
      <div class="flex-1 min-w-[220px]">
        <input
          v-model="q"
          placeholder="Pesquisar unidade pelo nome..."
          class="w-full rounded-xl border px-4 py-2"
        />
      </div>

      <button
        class="rounded-xl bg-black text-white px-4 py-2 shadow hover:opacity-90"
        @click="onCreate"
      >
        + Criar Unidade
      </button>
    </div>

    <div class="mt-6 rounded-xl border overflow-hidden">
      <div class="grid grid-cols-12 bg-gray-50 text-xs font-semibold p-3">
        <div class="col-span-1">#</div>
        <div class="col-span-5">Nome</div>
        <div class="col-span-3">Meta (mês)</div>
        <div class="col-span-2">Status</div>
        <div class="col-span-1">Ação</div>
      </div>

      <div
        v-for="(u, idx) in filtered"
        :key="u.id"
        class="grid grid-cols-12 p-3 border-t items-center hover:bg-gray-50 cursor-pointer"
        @click="goDetails(u.id)"
      >
        <div class="col-span-1 text-sm text-gray-500">{{ idx + 1 }}</div>
        <div class="col-span-5 font-medium">{{ u.nome }}</div>
        <div class="col-span-3 text-sm text-gray-700">R$ {{ u.metaMensal }}</div>
        <div class="col-span-2 text-sm">
          <span
            class="px-2 py-1 rounded-full text-xs"
            :class="u.isAtivo ? 'bg-emerald-100 text-emerald-800' : 'bg-gray-100 text-gray-700'"
          >
            {{ u.isAtivo ? 'Ativa' : 'Inativa' }}
          </span>
        </div>
        <div class="col-span-1 text-sm text-gray-500">→</div>
      </div>
    </div>
  </PageFile>
</template>

<script setup>
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import PageFile from '@/components/common/PageFile.vue'

const router = useRouter()
const q = ref('')

// Mock temporário só pra layout.
// Depois vamos ligar na API GET /api/unidades com um store/service.
const units = ref([
  { id: '1', nome: 'Matriz', metaMensal: 25000, isAtivo: true },
  { id: '2', nome: 'Filial Norte', metaMensal: 18000, isAtivo: true },
  { id: '3', nome: 'Filial Sul', metaMensal: 12000, isAtivo: false },
])

const filtered = computed(() => {
  const term = q.value.trim().toLowerCase()
  if (!term) return units.value
  return units.value.filter((u) => u.nome.toLowerCase().includes(term))
})

function onCreate() {
  alert('Depois vamos abrir o modal de criação (POST /api/unidades).')
}

function goDetails(id) {
  // Na próxima parte: /app/unidades/:id (detalhes com abas)
  // Por enquanto só mostra que navega:
  console.log('Ir para detalhes da unidade:', id)
}
</script>
