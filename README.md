# FieldOps Platform (sample)

A small multi-tenant service for field-service companies: customers, wells, and work orders. Each operating company is a tenant and must only ever see its own data.

This repository is a sample codebase. It exists so that [Second Look](https://github.com/Changliu53/second-look) has a realistic place to review pull requests: ASP.NET Core, Entity Framework Core, and a few platform rules that are easy to break. It is not a product and is not deployed anywhere.

## Layout

```
src/FieldOps.Api/
  Tenancy/      who the current tenant is
  Data/         DbContext, entities, migrations
  WorkOrders/   work order endpoints
  Customers/    customer endpoints
  Auditing/     who changed what, and when
```

## Platform rules

The rules a change must respect are in [.second-look.md](.second-look.md). Second Look reads that file from `main` and applies it to every pull request.

In short:

- Every query on tenant-owned data is filtered by the current tenant. The `DbContext` does this with global query filters, so the usual way to break it is to turn them off.
- Every endpoint that changes data has an authorization policy.
- Migrations keep the data in existing rows.
- Changes to work orders and customers are written to the audit log.
