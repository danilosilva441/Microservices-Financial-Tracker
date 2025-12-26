/** @type {import('tailwindcss').Config} */
export default {
  darkMode: 'class',

  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}", // Garanta que esta linha está correta
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: "#2563eb", // Azul principal
          dark: "#1e40af",    // Azul escuro
          light: "#60a5fa",   // Azul claro
        },
        secondary: {
          DEFAULT: "#f59e0b", // Laranja/dourado
          dark: "#b45309",
          light: "#fbbf24",
        },
        neutral: {
          DEFAULT: "#374151", // Cinza padrão
          light: "#9ca3af",
          dark: "#111827",
        },
      },
      fontFamily: {
        sans: ["Inter", "Roboto", "sans-serif"], // Fonte padrão
        heading: ["Poppins", "sans-serif"],      // Fonte para títulos
      },
      spacing: {
        18: "4.5rem", // adiciona espaçamento extra
        22: "5.5rem",
      },
      borderRadius: {
        "4xl": "2rem", // bordas bem arredondadas
      },
      boxShadow: {
        card: "0 4px 10px rgba(0, 0, 0, 0.1)", // sombra suave para cards
      },
    },
  },
  plugins: [
    require("@tailwindcss/forms"),
    require("@tailwindcss/typography"),
  ],

}