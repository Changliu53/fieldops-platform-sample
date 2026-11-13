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
walkthrough/    scripts that open a prepared pull request and push its fix
```

## Platform rules

The rules a change must respect are in [.second-look.md](.second-look.md). Second Look reads that file from `main` and applies it to every pull request.

In short:

- Every query on tenant-owned data is filtered by the current tenant. The `DbContext` does this with global query filters, so the usual way to break it is to turn them off.
- Every endpoint that changes data has an authorization policy.
- Migrations keep the data in existing rows.
- Changes to work orders and customers are written to the audit log.

## Try it yourself

You can run a real review here: open a pull request, watch the first pass, decide on the findings, publish, push a fix, and see the second review.

1. Ask the owner of this repository to add you as a collaborator with write access, and accept the invitation GitHub emails you.
2. Clone this repository and open a pull request. Either make a change of your own on a branch, or use a prepared one:
   ```
   walkthrough/open-pr.sh work-order-export
   ```
3. Open the [Second Look dashboard](https://second-look-review.vercel.app/dashboard) and sign in with GitHub. Your pull request is listed and the review takes about a minute.
4. Confirm or reject each finding and publish. The review appears on the pull request.
5. Push a fix (`walkthrough/push-fix.sh work-order-export` for the prepared one). A second review starts and says what was fixed.

The steps in detail are in [walkthrough/README.md](walkthrough/README.md). To look without an account, the [playground](https://second-look-review.vercel.app) and the [finished example reviews](https://second-look-review.vercel.app/example) need no sign-in.

### What access this involves

- **You install nothing.** The Second Look GitHub App is installed by this repository's owner, on this repository. It is a private app and cannot be installed on your account. On this repository it can read code and write checks and pull request reviews, and nothing more.
- **Signing in identifies you.** Second Look reads your GitHub login from the sign-in and keeps it in a signed session cookie. It does not store the token GitHub issues. GitHub's sign-in screen uses the same wording for every GitHub App, including "act on your behalf"; what an app can reach is still limited to the repositories it is installed on, and it is installed on none of yours.
- **Your other repositories are not involved.** The dashboard asks GitHub one question about you: does this login have write access to this repository? That is why step 1 is needed, and it is the only thing that changes when you are added or removed.
- **Reviews run only for collaborators.** A pull request from someone without write access is ignored, because each review is a paid model call.

