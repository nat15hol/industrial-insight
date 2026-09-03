import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [showPassword, setShowPassword] = useState(false)

  const canSubmit = email.trim() !== '' && password.trim() !== ''

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setError('')

    if (!canSubmit) {
      setError('Please enter both email and password.')
      return
    }

    try {
      await login(email, password)
      navigate('/')
    } catch {
      setError('Incorrect email or password.')
    }
  }

  return (
    <main className="mx-auto max-w-md p-6">
      <div className="space-y-8">
        <div>
          <h1 className="mb-2 text-4xl font-bold">Sign In</h1>

          <p className="text-sm text-gray-500">
            Sign in to access Industrial Insight.
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label htmlFor="email" className="mb-1 block">
              Email
            </label>

            <input
              id="email"
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              required
              autoComplete="email"
              className="w-full rounded border p-2"
            />
          </div>

          <div>
            <label htmlFor="password" className="mb-1 block">
              Password
            </label>

            <div className="relative">
              <input
                id="password"
                type={showPassword ? 'text' : 'password'}
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                required
                autoComplete="current-password"
                className="w-full rounded border p-2 pr-20"
              />

              <button
                type="button"
                onClick={() => setShowPassword((current) => !current)}
                className="absolute right-2 top-1/2 -translate-y-1/2 text-sm text-gray-500 hover:text-gray-700"
              >
                {showPassword ? 'Hide' : 'Show'}
              </button>
            </div>
          </div>

          {error && (
            <p className="text-sm text-red-600" role="alert">
              {error}
            </p>
          )}

          <button
            type="submit"
            disabled={!canSubmit}
            className={`w-full rounded px-4 py-2 text-white transition ${
              canSubmit
                ? 'bg-blue-600 hover:bg-blue-700'
                : 'cursor-not-allowed bg-gray-400'
            }`}
          >
            Sign In
          </button>
        </form>

        <p className="pb-4 text-sm">
          Don't have an account?{' '}
          <Link to="/register" className="text-blue-600 hover:underline">
            Register
          </Link>
        </p>


        <p className="text-xs text-gray-500">
          Industrial Insight is a prototype for industrial maintenance
          prioritization. Account information is used for authentication and
          application functionality. Passwords are hashed and never stored in plain
          text.{' '}
          <Link to="/privacy" className="underline hover:text-gray-700">
            Privacy Notice
          </Link>
        </p>
      </div>
    </main>
  )
}

export default LoginPage