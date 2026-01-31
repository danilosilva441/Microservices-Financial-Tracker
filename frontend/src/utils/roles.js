// src/utils/roles.js
export const ROLES = {
  USER: 'User',
  ADMIN: 'Admin',
  DEV: 'Dev',
  GERENTE: 'Gerente',
  SUPERVISOR: 'Supervisor',
  LIDER: 'Lider',
  OPERADOR: 'Operador',
};

// Função original (mantida para compatibilidade)
export function hasRole(user, ...roles) {
  const role = (user?.role || user?.roles?.[0] || '').toString().toLowerCase();
  return roles.some(r => r.toLowerCase() === role);
}

// Função para obter role do usuário
export function getUserRole(user) {
  const role = user?.role || user?.roles?.[0];
  if (!role) return null;
  
  // Encontra a role correspondente nas constantes
  const normalizedRole = role.toString().toLowerCase();
  const roleKey = Object.keys(ROLES).find(
    key => ROLES[key].toLowerCase() === normalizedRole
  );
  
  return roleKey ? ROLES[roleKey] : null;
}

// Função para verificar hierarquia de roles
export function canManageRole(managerRole, targetRole) {
  const hierarchy = [
    ROLES.ADMIN,
    ROLES.GERENTE,
    ROLES.SUPERVISOR,
    ROLES.LIDER,
    ROLES.DEV,
    ROLES.OPERADOR,
    ROLES.USER
  ];
  
  const managerIndex = hierarchy.indexOf(managerRole);
  const targetIndex = hierarchy.indexOf(targetRole);
  
  return targetIndex > managerIndex;
}

// Exporta todas as roles como array
export const ROLE_LIST = Object.values(ROLES);

// Helper: Verifica se é admin
export function isAdmin(user) {
  return getUserRole(user) === ROLES.ADMIN;
}

// Helper: Verifica se é gerente
export function isGerente(user) {
  return getUserRole(user) === ROLES.GERENTE;
}

// Helper: Verifica se é supervisor
export function isSupervisor(user) {
  return getUserRole(user) === ROLES.SUPERVISOR;
}