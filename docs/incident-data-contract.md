# Incident Data Contract

## Create Incident

When a technician creates an incident:

- `Status` defaults to `Open`
- `Priority` defaults to `Medium`
- `Category` defaults to `Other`

The technician selects the machine and provides the description.
Priority and Category may be changed later by a Manager.

## Allowed values

### Status
- `Open`
- `In Progress`
- `Resolved`

### Priority
- `High`
- `Medium`
- `Low`

### Category
- `Mechanical`
- `Electrical`
- `Automation`
- `Software`
- `Hydraulics`
- `Other`