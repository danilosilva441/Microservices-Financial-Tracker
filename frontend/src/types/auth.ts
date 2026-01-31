// src/types/auth.ts
export interface User {
  id: string
  name: string
  email: string
  role: string
  tenantId?: string
  avatar?: string
  createdAt: string
  updatedAt: string
}

export interface LoginCredentials {
  email: string
  password: string
}

export interface LoginResponse {
  token: string
  user?: User
}

export interface TenantData {
  nome: string
  cnpj: string
  emailDoGerente: string
  senhaDoGerente: string
  telefone?: string
  endereco?: {
    rua: string
    numero: string
    cidade: string
    estado: string
    cep: string
  }
}

export interface Tenant {
  id: string
  nome: string
  cnpj: string
  ativo: boolean
  createdAt: string
  updatedAt: string
}

export interface JWTDecoded {
  sub: string
  email: string
  role?: string
  tenantId?: string
  exp: number
  iat: number
  [key: string]: any
}