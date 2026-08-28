import { createContext, useContext, useState } from 'react'

type User = {
  userId: number
  name: string
  email: string
  roleId: number
  role: string
}

type AuthContextType = {
  user: User | null
  token: string | null
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<void>
  register: (name: string, email: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(() => {
    const storedUser = localStorage.getItem('user')

    if (!storedUser) {
      return null
    }

    try {
      return JSON.parse(storedUser) as User
    } catch (error) {
      console.error('Failed to parse stored user:', error)
      return null
    }
  })
  const [token, setToken] = useState<string | null>(
    localStorage.getItem('token'),
  )

  const login = async (email: string, password: string) => {
    const response = await fetch(`${API_BASE_URL}/api/Auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        email,
        password,
      }),
    })

    if (!response.ok) {
      throw new Error('Invalid email or password.')
    }

    const data = await response.json()

    const loggedInUser: User = {
      userId: data.userId,
      name: data.name,
      email: data.email,
      roleId: data.roleId,
      role: data.role,
    }

    localStorage.setItem('token', data.token)
    localStorage.setItem('user', JSON.stringify(loggedInUser))

    setToken(data.token)
    setUser(loggedInUser)
  }

  const register = async (
    name: string,
    email: string,
    password: string,
  ) => {
    const response = await fetch(`${API_BASE_URL}/api/Auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        name,
        email,
        password,
      }),
    })

    if (!response.ok) {
      throw new Error('Registration failed.')
    }
  }

  const logout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    setToken(null)
    setUser(null)
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: token !== null,
        login,
        register,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }

  return context
}