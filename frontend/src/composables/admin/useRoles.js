import { computed } from 'vue'
import { RolesEnum } from '@/stores/admin.store'
import { useAdminStore } from '@/stores/admin.store'

export function useRoles() {
  const adminStore = useAdminStore()

  const rolesDisponiveis = computed(() => RolesEnum.getAll())

  const getRoleNome = (role) => {
    return RolesEnum.getNome(role)
  }

  const getRoleCor = (role) => {
    return RolesEnum.getCor(role)
  }

  const getRoleDescricao = (role) => {
    return RolesEnum.getDescricao(role)
  }

  const getRoleNivel = (role) => {
    return RolesEnum.getNivel(role)
  }

  const podeGerenciar = (roleUsuario, roleAlvo) => {
    return RolesEnum.podeGerenciar(roleUsuario, roleAlvo)
  }

  const temPermissao = (roleUsuario, funcionalidade) => {
    return RolesEnum.temPermissao(roleUsuario, funcionalidade)
  }

  const getRolesPorNivel = (nivelMinimo = 1) => {
    return RolesEnum.getAll()
      .filter(r => r.nivel >= nivelMinimo)
      .sort((a, b) => b.nivel - a.nivel)
  }

  const getRolesGerenciadasPor = (roleUsuario) => {
    return RolesEnum.getAll()
      .filter(r => RolesEnum.podeGerenciar(roleUsuario, r.id))
  }

  const verificarPermissaoUsuario = (funcionalidade) => {
    // Em produção, pegaria do auth store
    const usuarioAtual = { role: 'Admin' } // Placeholder
    return temPermissao(usuarioAtual.role, funcionalidade)
  }

  const sugerirRole = (dadosUsuario) => {
    return adminStore.sugerirRoleParaUsuario(dadosUsuario)
  }

  return {
    // Enum completo
    RolesEnum,
    
    // Computados
    rolesDisponiveis,
    
    // Métodos
    getRoleNome,
    getRoleCor,
    getRoleDescricao,
    getRoleNivel,
    podeGerenciar,
    temPermissao,
    getRolesPorNivel,
    getRolesGerenciadasPor,
    verificarPermissaoUsuario,
    sugerirRole,
  }
}