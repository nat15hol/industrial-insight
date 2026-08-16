# 0004: JWT authentication strategy — short-lived token, no refresh token

**Date:** 2026-08-17
**Status:** Decided

## Context

The system needs a concrete token strategy: lifetime, whether refresh tokens exist, what claims the token carries, and where the signing secret lives. `architecture.md` and `api-contract.md` already establish *that* JWT Bearer auth is used and that server-side RBAC is authoritative, but not the specifics — leaving this undecided risks the frontend being built around wrong assumptions (e.g. a silent refresh flow that the backend never implements).

## Decision

- **Token lifetime:** 60 minutes. On expiry, the frontend redirects to login — there is no silent refresh.
- **No refresh token** for this project's scope. Re-authentication via the login form is the only renewal path.
- **Claims:** `sub` (user ID), `role` (`Technician` | `Manager`), `email`, standard `iat`/`exp`. No additional PII in the token payload.
- **Signing:** symmetric (HMAC-SHA256), secret read from environment configuration (`Jwt:Secret` or equivalent), never committed — listed in the environment variables list (see [setup.md](setup.md)) without a value.
- **Transport:** `Authorization: Bearer <token>` header only. No cookies, no token in URL/query string.

## Why

- **Scope-appropriate.** A refresh-token flow (rotation, revocation list, storage strategy) is meaningful additional surface area for a project of this size and timeline, with no corresponding requirement in `known-limitations.md`'s P0/P1 scope.
- **Predictable failure mode.** A hard expiry with redirect-to-login is simple to implement and simple to test — it produces one clear 401 behavior rather than two (expired-but-refreshable vs. truly expired).
- **Matches the RBAC model.** Carrying `role` directly in the token avoids an extra DB lookup per request to check permissions, consistent with the "server-side RBAC is the authority" principle in `architecture.md` — the role is still re-validated against the DB on any mutating operation to guard against a role change mid-session, not blindly trusted for the token's full lifetime on sensitive actions.

## Alternatives Considered

- **Refresh tokens with rotation** — rejected: real-world best practice, but disproportionate implementation and testing cost for the project's timeline and demo context; noted as a plausible Future Improvement.
- **Long-lived token (e.g. 7 days)** — rejected: increases the impact window of a leaked token with no offsetting benefit for a project that isn't optimizing for reduced login friction.
- **Session cookies instead of JWT** — rejected: doesn't fit the stated REST API + separate frontend architecture already committed to in `architecture.md`; would also need CSRF handling not otherwise required.

## Consequences

- Frontend must handle a 401 from an expired token by clearing local auth state and redirecting to login — no silent-retry-after-refresh logic needed.
- `setup.md` must list `Jwt:Secret` (and issuer/audience if used) in the environment variables section, without a value.
- If a Manager's role is changed after token issuance (not currently a supported flow — see ADR 0003), the change won't take effect until re-login, since role is baked into the token for its 60-minute lifetime. Acceptable given no such flow exists in current scope.