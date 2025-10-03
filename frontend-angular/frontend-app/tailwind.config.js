/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {},
  },
  plugins: [],
  important: true, //Force the dependecies of TailwindCss, important in all utilities
}