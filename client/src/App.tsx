import { BrowserRouter, Route, Routes } from 'react-router-dom'
import AppLayout from './layouts/AppLayout'
import HomePage from './pages/HomePage'
import MachinesPage from './pages/MachinesPage'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppLayout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/machines" element={<MachinesPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App
