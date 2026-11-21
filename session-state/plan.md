# Implementation Plan: Missing Agent in DB

## Problem statement
The repository does not currently show a definitive database source or creation path for an Agent. Agent authentication spans MySQL, Redis, and duplicate Agent implementations, while the solution currently has build-breaking interface and namespace issues.

## Clarification required
Please confirm which condition is intended:

1. **MySQL agent row is missing**: an agent exists only in Redis or configuration but has no row in `sub_agent_master`.
2. **Redis agent cache key is missing**: the intended key `g:agent1:{agentId}` has no record in the GDS Redis store.
3. **Authentication reports an agent missing**: the API receives credentials but lookup or validation fails.
4. **A persistent Agent entity/table is missing**: a new database-backed Agent model and migration/seed are required.

Also clarify whether the implementation plan should repair the existing authentication code only or also add/seed persistent agent data.

## Current state findings
- `AgentRepository` queries MySQL tables/stored procedures, including `sub_agent_master`, `sub_agent_users`, `REST_GET_CLIENT_DETAILS_V1`, and `REST_GET_CLIENT_DETAILS`.
- `AgentDetails` reads Redis key `g:agent1:{agentId}` for client-secret validation.
- `AuthRepository` validates client credentials exclusively from Redis.
- Duplicate `Agent` classes exist across the Entities, DB, and repository namespaces.
- `IAgentRepository` declares only `ValidateAgentAndUserCredentials`, but `AgentService` calls missing methods `GetAgentIdFromCache`, `GetAgentByLoginId`, and `GetAgentDetails`.
- `Repositories/AuthRepository.cs` references `RestAPICore.Domain.Agent`, but no `RestAPICore.Domain.Agent` type exists.
- The solution contains no migrations or seed scripts for an Agent entity.

## Proposed implementation approach

### 1. Lock down the expected Agent source of truth
- Identify the canonical Agent store: MySQL, Redis, or both.
- Document required fields and key conventions.
- Define what “Agent exists” means for client credentials and login credentials.

### 2. Consolidate duplicate Agent implementations
- Select one Agent domain implementation and remove or deprecate conflicting paths.
- Make `Entities.AgentDetails` use the configured Redis source consistently.
- Remove ambiguous `Agent` references and invalid namespace references.

### 3. Complete the repository/service contract
- Add all AgentRepository methods required by `AgentService` to `IAgentRepository`.
- Restore cache lookup behavior or replace it with a deterministic database fallback.
- Keep database and Redis responsibilities explicit and avoid silent fallback behavior.

### 4. Restore and standardize credential lookup
- Preserve the existing MySQL fallback for login/agent lookup.
- Ensure required agent detail fields are read safely and case-insensitively.
- Validate active state, password/secret, and identifiers consistently.
- Add clear not-found versus invalid-credentials behavior.

### 5. Add missing data support if required
- If MySQL data is missing, add a migration/seed or operational script for the minimum `sub_agent_master`/related records.
- If Redis data is missing, add cache population or synchronization logic from MySQL/system of record.
- If this is a true entity design request, add the model, schema, migration, repository methods, and seed data.
- Do not add hard-coded production credentials or secrets.

### 6. Add focused regression coverage
- Cover Redis hit and miss behavior.
- Cover MySQL agent lookup hit and miss behavior.
- Cover client secret and agent password validation.
- Cover duplicate/ambiguous Agent resolution and service/repository contract behavior.
- Run the smallest relevant build/test set first, then the full solution build.

### 7. Verification
- Confirm the solution builds without namespace or missing-interface-member errors.
- Confirm an existing Agent is found in the selected source.
- Confirm a missing Agent is not authenticated and returns the repository/API-standard failure.
- Confirm Redis/MySQL synchronization behavior, if included in scope.

## Todos
1. Answer the clarification questions above.
2. Identify the canonical Agent source and required schema/cache contract.
3. Consolidate duplicate Agent implementations and fix current build errors.
4. Complete `IAgentRepository` and credential lookup behavior.
5. Add or seed the missing Agent data in the selected source.
6. Add focused tests and perform build/test verification.

## Notes and considerations
- This is a plan only; no repository source files have been changed.
- The plan intentionally does not select a database source until the ambiguity is resolved.
