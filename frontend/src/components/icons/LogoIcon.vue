<!-- src/components/icons/LogoIcon.vue -->
<template>
  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" role="img" aria-label="DS SysTech icon" :width="width" :height="height" :style="{ color: color }">
    <title>DS SysTech (Icon)</title>
    <desc>Ícone em estilo financeiro/tecnologia com monograma DS.</desc>

    <defs>
      <linearGradient id="iconGrad" x1="0" y1="0" x2="1" y2="1">
        <stop offset="0" :stop-color="gradientStart"/>
        <stop offset="1" :stop-color="gradientEnd"/>
      </linearGradient>

      <linearGradient id="strokeGrad" x1="0" y1="0" x2="1" y2="0">
        <stop offset="0" stop-color="#EAF2FF"/>
        <stop offset="1" stop-color="#FFFFFF"/>
      </linearGradient>

      <filter id="softShadow" x="-20%" y="-20%" width="140%" height="140%">
        <feDropShadow dx="0" dy="18" stdDeviation="18" flood-color="#0A1A2B" flood-opacity="0.22"/>
      </filter>

      <filter id="innerGlow" x="-20%" y="-20%" width="140%" height="140%">
        <feGaussianBlur in="SourceAlpha" stdDeviation="5" result="blur"/>
        <feComposite in="blur" in2="SourceAlpha" operator="arithmetic" k2="-1" k3="1" result="inner"/>
        <feColorMatrix in="inner" type="matrix"
          values="1 0 0 0 1
                  0 1 0 0 1
                  0 0 1 0 1
                  0 0 0 0.18 0" />
        <feComposite in="SourceGraphic" operator="over"/>
      </filter>
    </defs>

    <g filter="url(#softShadow)">
      <!-- Fundo -->
      <rect x="52" y="52" width="408" height="408" rx="110" fill="url(#iconGrad)"/>

      <!-- Detalhes de fundo -->
      <g opacity="0.20">
        <rect x="118" y="310" width="32" height="86" rx="16" fill="#FFFFFF"/>
        <rect x="168" y="270" width="32" height="126" rx="16" fill="#FFFFFF"/>
        <rect x="218" y="230" width="32" height="166" rx="16" fill="#FFFFFF"/>
        <path d="M110 190 C190 140, 260 136, 380 190" fill="none" stroke="#FFFFFF" stroke-width="14" stroke-linecap="round"/>
      </g>

      <!-- Monograma DS com efeito de glow interno -->
      <g filter="url(#innerGlow)">
        <!-- Letra D -->
        <path d="M170 170
                 V342
                 C170 372 194 396 224 396
                 H270
                 C340 396 388 348 388 278
                 C388 216 344 170 282 170
                 H224
                 C194 170 170 194 170 224"
              fill="none"
              stroke="url(#strokeGrad)"
              stroke-width="34"
              stroke-linecap="round"
              stroke-linejoin="round"/>

        <!-- Letra S -->
        <path d="M362 216
                 C348 188 314 170 276 170
                 C230 170 202 192 202 226
                 C202 258 228 276 276 284
                 C328 292 354 312 354 342
                 C354 378 318 396 276 396
                 C238 396 206 380 192 352"
              fill="none"
              stroke="#E9FFFB"
              stroke-width="30"
              stroke-linecap="round"
              stroke-linejoin="round"
              opacity="0.95"/>

        <!-- Nós/circuitos -->
        <circle cx="392" cy="186" r="14" fill="#FFFFFF" opacity="0.9"/>
        <circle cx="140" cy="392" r="14" fill="#FFFFFF" opacity="0.8"/>
        <circle cx="414" cy="388" r="11" fill="#FFFFFF" opacity="0.7"/>
      </g>
    </g>

    <!-- Borda invisível para manter o viewBox -->
    <rect x="0" y="0" width="512" height="512" fill="none"/>
  </svg>
</template>

<script>
export default {
  name: 'LogoIcon',
  props: {
    width: {
      type: [String, Number],
      default: '512'
    },
    height: {
      type: [String, Number],
      default: '512'
    },
    variant: {
      type: String,
      default: 'light', // 'light' ou 'dark'
      validator: (value) => ['light', 'dark'].includes(value)
    },
    color: {
      type: String,
      default: ''
    }
  },
  computed: {
    gradientStart() {
      return this.variant === 'dark' ? '#1A6BFF' : '#0B5FFF'
    },
    gradientEnd() {
      return this.variant === 'dark' ? '#10D6C2' : '#08B3A4'
    },
    textColor() {
      return this.variant === 'dark' ? '#F4F7FF' : '#0A1A2B'
    }
  }
}
</script>