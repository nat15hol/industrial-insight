import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

function AppLayout() {
  const { isAuthenticated, user, logout } = useAuth()

  return (
    <div className="min-h-screen">
      <header className="border-b">
        <nav className="mx-auto flex max-w-6xl items-center gap-6 px-6 py-4">
          <NavLink
            to="/"
            className={({ isActive }) =>
              isActive ? 'font-bold' : 'text-gray-600'
            }
          >
            Home
          </NavLink>

          <NavLink
            to="/machines"
            className={({ isActive }) =>
              isActive ? 'font-bold' : 'text-gray-600'
            }
          >
            Machines
          </NavLink>

          <NavLink
            to="/incidents"
            end
            className={({ isActive }) =>
              isActive ? 'font-bold' : 'text-gray-600'
            }
          >
            Incidents
          </NavLink>

          <NavLink
            to="/incidents/new"
            className={({ isActive }) =>
              isActive ? 'font-bold' : 'text-gray-600'
            }
          >
            Report incident
          </NavLink>

          <NavLink
            to="/tasks"
            end
            className={({ isActive }) =>
              isActive ? 'font-bold' : 'text-gray-600'
            }
          >
            Tasks
          </NavLink>

          {user?.role === 'Manager' && (
            <NavLink
              to="/tasks/new"
              className={({ isActive }) =>
                isActive ? 'font-bold' : 'text-gray-600'
              }
            >
              Assign task
            </NavLink>
          )}

          <div className="ml-auto flex items-center gap-3 text-sm">
            {isAuthenticated ? (
              <>
                <span className="text-gray-600">
                  Logged in as {user?.email ?? 'unknown'}
                </span>
                <button
                  type="button"
                  onClick={logout}
                  className="rounded border px-3 py-1 hover:bg-gray-50"
                >
                  Log out
                </button>
              </>
            ) : (
              <>
                <span className="text-gray-600">Not logged in</span>
                <NavLink
                  to="/login"
                  className="rounded border px-3 py-1 hover:bg-gray-50"
                >
                  Log in
                </NavLink>
              </>
            )}
          </div>
        </nav>
      </header>

      <main className="mx-auto max-w-6xl px-6 py-8">
        <Outlet />
      </main>
    </div>
  )
}

export default AppLayout