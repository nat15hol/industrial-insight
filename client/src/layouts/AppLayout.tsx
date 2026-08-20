import { NavLink, Outlet } from 'react-router-dom'

function AppLayout() {
  return (
    <div className="min-h-screen">
      <header className="border-b">
        <nav className="mx-auto flex max-w-6xl gap-6 px-6 py-4">
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
        </nav>
      </header>

      <main className="mx-auto max-w-6xl px-6 py-8">
        <Outlet />
      </main>
    </div>
  )
}

export default AppLayout