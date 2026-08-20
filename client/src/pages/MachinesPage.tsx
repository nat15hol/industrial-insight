import { useEffect, useState } from 'react'

type Location = {
  locationId: number
  name: string
  address: string
}

type Machine = {
  machineId: number
  name: string
  status: string
  runtime: number
  locationId: number
  location: Location | null
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

function MachinesPage() {
  const [machines, setMachines] = useState<Machine[]>([])
  const [selectedMachine, setSelectedMachine] = useState<Machine | null>(null)

  useEffect(() => {
    const token = localStorage.getItem('token')

    fetch(`${API_BASE_URL}/api/Machine`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error(`HTTP error ${response.status}`)
        }

        return response.json()
      })
      .then((data: Machine[]) => {
        setMachines(data)
      })
      .catch((error) => {
        console.error('Failed to fetch machines:', error)
      })
  }, [])

  return (
    <main className="machine-page">
      <header className="page-header">
        <h1 className="text-4xl font-bold">Machines</h1>
        <p>Browse and inspect industrial machines.</p>
      </header>

      <section className="machine-layout">
        <div className="machine-list">
          <h2>Machine list</h2>

          {machines.length === 0 ? (
            <p>No machines found.</p>
          ) : (
            machines.map((machine) => (
              <button
                key={machine.machineId}
                type="button"
                className="machine-card"
                onClick={() => setSelectedMachine(machine)}
              >
                <strong>{machine.name}</strong>
                <span>{machine.status}</span>
              </button>
            ))
          )}
        </div>

        <div className="machine-detail">
          {selectedMachine ? (
            <>
              <h2>{selectedMachine.name}</h2>

              <dl>
                <dt>Status</dt>
                <dd>{selectedMachine.status}</dd>

                <dt>Runtime</dt>
                <dd>{selectedMachine.runtime}</dd>

                <dt>Location</dt>
                <dd>{selectedMachine.location?.name ?? 'Unknown'}</dd>

                <dt>Address</dt>
                <dd>{selectedMachine.location?.address ?? 'Unknown'}</dd>
              </dl>
            </>
          ) : (
            <p>Select a machine to view its details.</p>
          )}
        </div>
      </section>
    </main>
  )
}

export default MachinesPage