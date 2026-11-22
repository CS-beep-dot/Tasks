# Security notes

How authentication and authorization work in this API, where secrets live, and what is out of scope.

## Authentication: JWT bearer tokens

Every protected request must send a token in the `Authorization` header:

```
Authorization: Bearer <token>
```

The token is validated in `Program.cs` (`AddJwtBearer`). A token is accepted only if **all** of these checks pass:

| Check | Setting | Rejects |
|---|---|---|
| Signature | `ValidateIssuerSigningKey`, HMAC-SHA256 with `Jwt:SigningKey` | Forged or edited tokens |
| Issuer (`iss`) | `ValidateIssuer`, must equal `Jwt:Issuer` | Tokens created by another system |
| Audience (`aud`) | `ValidateAudience`, must equal `Jwt:Audience` | Tokens meant for another API |
| Lifetime (`nbf`, `exp`) | `ValidateLifetime`, `ClockSkew` = 30 seconds | Expired or not-yet-valid tokens |

Other settings:
- `MapInboundClaims = false` keeps the standard claim names (`sub`, `role`) instead of renaming them to long Microsoft URIs.
- `NameClaimType = "sub"` and `RoleClaimType = "role"` tell ASP.NET Core which claims hold the user id and the role.
- The default `ClockSkew` of 5 minutes is reduced to 30 seconds, so an expired token stops working almost immediately.

The token payload is only Base64URL-encoded, not encrypted. It must never contain secrets.

## Token issuer: development only

`POST /api/dev/token` (`DevTokenController`) creates a signed token for any `userId` and `role`:

```json
{ "userId": 1, "role": "Manager" }
```

- It is a **development-only** issuer. Outside the `Development` environment it returns **404**, as if it did not exist.
- It does not check a password, so anyone who can call it can choose their role. That is acceptable only on a developer machine.
- In production, tokens must come from a trusted identity provider (for example Microsoft Entra ID, Auth0 or Keycloak). Only the issuer, audience and signing key settings would change; the authorization rules stay the same.

## Authorization rules

| Rule | Policy | Applied to |
|---|---|---|
| Every endpoint requires a valid token unless it opts out | `FallbackPolicy` (`RequireAuthenticatedUser`) | All endpoints |
| Reading tasks is public | `[AllowAnonymous]` | `GET /api/tasks`, `GET /api/tasks/{id}` |
| Only Admin deletes projects | `CanDeleteProjects`: role `Admin` | `DELETE /api/projects/{id}` |
| Manager or Admin assigns tasks | `CanAssignTasks`: role `Manager` or `Admin` | `PUT /api/tasks/{id}/assignee` |
| Manager or Admin deletes tasks | `CanDeleteTasks`: role `Manager` or `Admin` | `DELETE /api/tasks/{id}` |
| Manager or Admin updates any task; other users only tasks assigned to them | `CanUpdateTasks`: custom requirement and handler | `PUT /api/tasks/{id}` |

The fallback policy makes the API **secure by default**: a new endpoint without an attribute is locked, not open.

### Per-user update rule (resource-based)

`CanUpdateTasks` depends on the task itself, so it cannot be an attribute (an attribute runs before the task is loaded). Instead:

1. `TasksController.UpdateTask` loads the task. A missing task returns **404**.
2. It calls `IAuthorizationService.AuthorizeAsync(User, task, "CanUpdateTasks")`.
3. `CanUpdateTaskHandler` (in `TaskManagementApi/Authorization`) succeeds when the role is `Manager` or `Admin`, or when `task.AssigneeId` equals the token's `sub` claim.
4. If it does not succeed, the controller returns **403**.

Assignment is only possible through `PUT /api/tasks/{id}/assignee`. `POST` and `PUT /api/tasks` do not accept `assigneeId`, so the Manager rule cannot be bypassed.

## 401 vs 403

| Status | Meaning | Examples |
|---|---|---|
| **401 Unauthorized** | The caller has not proved who they are | No token, malformed token, wrong signature, expired, wrong issuer or audience |
| **403 Forbidden** | The token is valid, but this user may not do this | A Manager deletes a project; a user updates someone else's task |

## Secrets

- **`Jwt:SigningKey` is never committed.** On a developer machine it is stored with .NET User Secrets (outside the repository, see the README). In production it must come from an environment variable (`Jwt__SigningKey`) or a secret store such as Azure Key Vault.
- The app **refuses to start** if `Jwt:Issuer`, `Jwt:Audience` or `Jwt:SigningKey` is missing, so a misconfigured server cannot run with authentication silently broken.
- HS256 needs a key of **at least 32 bytes**. Use a long random value, not a word or phrase.
- The tests use their own issuer, audience and key (`ApiFactory`). That key is clearly marked test-only and is never used by the real app.
- No passwords exist in the project. The database connection uses Windows authentication (LocalDB).
- Do not commit tokens either, for example in `TaskManagementApi.http`. The `.http` file gets fresh tokens from the login requests instead.

If a signing key leaks, replace it. Every token signed with the old key stops working immediately.

## Automated tests

| File | Covers |
|---|---|
| `TokenValidationTests` | No token, malformed, forged (wrong key), expired, wrong issuer and wrong audience all return 401; a valid token is accepted |
| `ProjectAuthTests` | Admin deletes a project (204), Manager is forbidden (403), no token (401), unknown id (404) |
| `TaskAssignAuthTests` | Manager and Admin assign (200), User is forbidden (403), no token (401), unknown task (404), unknown user (400) |
| `TaskUpdateAuthTests` | Owner updates own task (200), other user is forbidden (403), Manager updates any task (200), no token (401), unknown task (404) |

## Known limitations

- **No revocation.** A stolen token works until it expires (30 minutes). There is no token blacklist.
- **No refresh tokens.** When a token expires, the client must request a new one.
- **Roles are trusted from the token.** The API does not check the database to confirm the user's role or that the user still exists.
- **Symmetric key.** The same key signs and verifies tokens. With a real identity provider, an asymmetric key (RS256) is better: the API only needs the public key and cannot create tokens itself.
- **No rate limiting** on any endpoint, including the dev token endpoint.
