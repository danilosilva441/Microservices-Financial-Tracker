<!-- src/views/auth/LoginView.vue -->
<template>
  <div class="login-content">
    <LoginForm @social-login="handleSocialLogin" />
  </div>
</template>

<script>
import LoginForm from '@/components/auth/LoginForm.vue'

export default {
  name: 'LoginView',
  
  components: {
    LoginForm
  },
  
  setup() {
    const handleSocialLogin = (provider) => {
      console.log(`Login social com ${provider}`)
    }
    
    return {
      handleSocialLogin
    }
  }
}
</script>

<style scoped>
.login-content {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>