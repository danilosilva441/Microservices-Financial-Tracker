// src/composables/auth/useRBAC.js
import { computed } from 'vue'
import { useAuth } from './useAuth'
import { ROLES, getUserRole, canManageRole } from '@/utils/roles'

export function useRBAC() {
  const { userRole, userTenantId, currentUser } = useAuth()

  // Helper para obter a role
  const safeUserRole = computed(() => {
    return getUserRole(currentUser.value) || userRole.value
  })

  // Permissões por role
  const PERMISSIONS = {
    // Admin pode tudo
    [ROLES.ADMIN]: ['*'],
    
    // Gerente gerencia o tenant/empresa
    [ROLES.GERENTE]: [
      'manage_users',
      'manage_projects',
      'view_reports',
      'manage_settings',
      'manage_financial',
      'approve_requests'
    ],
    
    // Supervisor supervisiona operações
    [ROLES.SUPERVISOR]: [
      'manage_team',
      'create_projects',
      'view_reports',
      'approve_work',
      'manage_quality'
    ],
    
    // Lider lidera equipe
    [ROLES.LIDER]: [
      'assign_tasks',
      'view_team_reports',
      'manage_daily_operations',
      'report_progress'
    ],
    
    // Dev desenvolve
    [ROLES.DEV]: [
      'access_code',
      'create_features',
      'fix_bugs',
      'deploy_applications',
      'view_tech_docs'
    ],
    
    // Operador opera sistemas
    [ROLES.OPERADOR]: [
      'operate_systems',
      'report_issues',
      'view_instructions',
      'clock_in_out'
    ],
    
    // Usuário básico
    [ROLES.USER]: [
      'view_own_data',
      'update_profile',
      'submit_requests'
    ]
  }

  // Hierarquia das roles
  const ROLE_HIERARCHY = [
    ROLES.ADMIN,
    ROLES.GERENTE,
    ROLES.SUPERVISOR,
    ROLES.LIDER,
    ROLES.DEV,
    ROLES.OPERADOR,
    ROLES.USER
  ]

  // Verifica se tem role
  const hasRole = (role) => {
    const current = safeUserRole.value
    if (!current) return false
    
    if (Array.isArray(role)) {
      return role.includes(current)
    }
    return current === role
  }

  // Verifica qualquer role
  const hasAnyRole = (roles) => {
    return roles.some(role => hasRole(role))
  }

  // Verifica permissão
  const hasPermission = (permission) => {
    const current = safeUserRole.value
    if (!current) return false
    
    if (hasRole(ROLES.ADMIN)) {
      return true
    }
    
    const rolePermissions = PERMISSIONS[current] || []
    
    if (rolePermissions.includes('*')) {
      return true
    }
    
    return rolePermissions.includes(permission)
  }

  // Múltiplas permissões
  const hasAllPermissions = (permissions) => {
    return permissions.every(permission => hasPermission(permission))
  }

  const hasAnyPermission = (permissions) => {
    return permissions.some(permission => hasPermission(permission))
  }

  // Verifica dono do recurso
  const isResourceOwner = (resourceTenantId) => {
    if (hasRole(ROLES.ADMIN)) {
      return true
    }
    
    if (!resourceTenantId) {
      return true
    }
    
    return userTenantId.value === resourceTenantId
  }

  // Pode gerenciar usuário
  const canManageUser = (targetUser) => {
    const current = safeUserRole.value
    const targetUserRole = getUserRole(targetUser)
    
    if (!current || !targetUserRole) return false
    
    if (hasRole(ROLES.ADMIN)) {
      return true
    }
    
    if (hasRole(ROLES.GERENTE)) {
      return targetUser?.tenantId === userTenantId.value
    }
    
    return canManageRole(current, targetUserRole) && 
           targetUser?.tenantId === userTenantId.value
  }

  // Pode visualizar recurso
  const canViewResource = (resource, resourceType) => {
    if (hasRole(ROLES.ADMIN)) {
      return true
    }
    
    if (!isResourceOwner(resource?.tenantId)) {
      return false
    }
    
    switch (resourceType) {
      case 'financial':
        return hasPermission('view_financial') || hasPermission('manage_financial')
      case 'reports':
        return hasPermission('view_reports')
      case 'projects':
        return hasPermission('manage_projects') || hasPermission('create_projects')
      case 'users':
        return hasPermission('manage_users')
      default:
        return true
    }
  }

  return {
    // Constants
    ROLES,
    PERMISSIONS,
    ROLE_HIERARCHY,
    
    // Computeds
    currentRole: safeUserRole,
    currentTenantId: userTenantId,
    
    // Permission checks
    hasRole,
    hasAnyRole,
    hasPermission,
    hasAllPermissions,
    hasAnyPermission,
    isResourceOwner,
    canManageUser,
    canViewResource,
    
    // Helper computeds para roles específicas
    isAdmin: computed(() => hasRole(ROLES.ADMIN)),
    isGerente: computed(() => hasRole(ROLES.GERENTE)),
    isSupervisor: computed(() => hasRole(ROLES.SUPERVISOR)),
    isLider: computed(() => hasRole(ROLES.LIDER)),
    isDev: computed(() => hasRole(ROLES.DEV)),
    isOperador: computed(() => hasRole(ROLES.OPERADOR)),
    isUser: computed(() => hasRole(ROLES.USER)),
    
    // Helper para compatibilidade
    checkRole: (...roles) => {
      const current = safeUserRole.value
      if (!current) return false
      return roles.some(r => r.toLowerCase() === current.toLowerCase())
    }
  }
}