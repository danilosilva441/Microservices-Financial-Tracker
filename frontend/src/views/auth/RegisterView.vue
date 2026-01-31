<!-- src/views/auth/RegisterView.vue -->
<template>
  <div class="register-content">
    <RegisterForm />
  </div>
</template>

<script>
import RegisterForm from '@/components/auth/RegisterForm.vue'

export default {
  name: 'RegisterView',
  
  components: {
    RegisterForm
  }
}
</script>

<style scoped>
.register-content {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>