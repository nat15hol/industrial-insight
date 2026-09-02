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
  priorityScore: number
  priorityBucket: string
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

function MachinesPage() {
  const [machines, setMachines] = useState<Machine[]>([])
  const [selectedMachine, setSelectedMachine] = useState<Machine | null>(null)
  const [telemetry, setTelemetry] = useState<TelemetryRecord[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [isTelemetryLoading, setIsTelemetryLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [telemetryError, setTelemetryError] = useState<string | null>(null)

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
        setIsLoading(false)
      })
      .catch((error) => {
        console.error('Failed to fetch machines:', error)
        setError('Failed to load machines.')
        setIsLoading(false)
      })
  }, [])

  const handleMachineSelect = async (machine: Machine) => {
    setSelectedMachine(machine)
    setTelemetry([])
    setTelemetryError(null)
    setIsTelemetryLoading(true)

    const token = localStorage.getItem('token')

    try {
      const response = await fetch(
        `${API_BASE_URL}/api/Machine/${machine.machineId}/telemetry`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        },
      )

      if (!response.ok) {
        throw new Error(`HTTP error ${response.status}`)
      }

      const data: TelemetryRecord[] = await response.json()
      setTelemetry(data)
    } catch (error) {
      console.error('Failed to fetch telemetry:', error)
      setTelemetryError('Failed to load telemetry.')
    } finally {
      setIsTelemetryLoading(false)
    }
  }

  return (
    <main className="machine-page">
      <header className="page-header">
        <h1 className="text-4xl font-bold">Machines</h1>
        <p>Browse and inspect industrial machines.</p>
      </header>

      <section className="machine-layout grid grid-cols-1 gap-6 md:grid-cols-2">
        <div className="machine-list">
          <h2 className="mb-3 text-xl font-semibold">Machine list</h2>

          {isLoading ? (
            <p>Loading machines...</p>
          ) : error ? (
            <p>{error}</p>
          ) : machines.length === 0 ? (
            <p>No machines found.</p>
          ) : (
            <div className="flex flex-col gap-2">
              {machines.map((machine) => (
                <button
                  key={machine.machineId}
                  type="button"
                  className="machine-card flex flex-col items-start gap-1 rounded border p-3 text-left hover:bg-gray-50"
                  onClick={() => handleMachineSelect(machine)}
                >
                  <strong>{machine.name}</strong>
                  <span className="text-sm text-gray-600">
                    {machine.status}
                  </span>
                </button>
              ))}
            </div>
          )}
        </div>

        <div className="machine-detail mt-8 rounded border p-4">
          {selectedMachine ? (
            <>
              <h2 className="mb-3 text-xl font-semibold">
                {selectedMachine.name}
              </h2>

              <dl className="mb-6 flex flex-col gap-2">
                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Status
                  </dt>
                  <dd>{selectedMachine.status}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Runtime
                  </dt>
                  <dd>{selectedMachine.runtime}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Priority Score
                  </dt>
                  <dd>
                    {selectedMachine.priorityScore} ({selectedMachine.priorityBucket})
                  </dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Location
                  </dt>
                  <dd>{selectedMachine.location?.name ?? 'Unknown'}</dd>
                </div>

                <div>
                  <dt className="text-sm font-semibold text-gray-600">
                    Address
                  </dt>
                  <dd>{selectedMachine.location?.address ?? 'Unknown'}</dd>
                </div>
              </dl>

              <h3 className="mb-3 text-lg font-semibold">Telemetry</h3>

              {isTelemetryLoading ? (
                <p>Loading telemetry...</p>
              ) : telemetryError ? (
                <p>{telemetryError}</p>
              ) : telemetry.length === 0 ? (
                <p>No telemetry records found.</p>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full border-collapse text-sm">
                    <thead>
                      <tr className="border-b text-left">
                        <th className="p-2">Timestamp</th>
                        <th className="p-2">Temperature</th>
                        <th className="p-2">Pressure</th>
                        <th className="p-2">Vibration</th>
                        <th className="p-2">Energy</th>
                      </tr>
                    </thead>
                    <tbody>
                      {telemetry.map((record) => (
                        <tr
                          key={record.telemetryRecordId}
                          className="border-b"
                        >
                          <td className="p-2">
                            {new Date(record.timestamp).toLocaleString()}
                          </td>
                          <td className="p-2">{record.temperature}</td>
                          <td className="p-2">{record.pressure}</td>
                          <td className="p-2">{record.vibration}</td>
                          <td className="p-2">{record.energy}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
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