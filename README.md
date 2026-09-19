# BKE Launcher

Customer-facing software hub for the BKE ecosystem.

This repository is intentionally initialized with a minimal main branch. Active Launcher development is performed on feature branches and remains unmerged until explicitly approved.

## Architecture ownership

- **Digital Solutions** owns cloud identity, accounts, organizations, entitlements, and catalog authority.
- **BKE Licensing Agent** owns machine identity, account-session secrets, local authorization, and privileged installation/update work.
- **BKE Launcher** owns customer-facing account UX, catalog UX, install/open/update UX, and the runtime host for owner-designated Launcher products.
- **Standalone products** remain independently executable after managed installation and continue to authorize through the Licensing Agent.

Product execution type is explicit owner policy. It is never inferred from product size or complexity.

The final Launcher product ID and release version are deliberately not locked by this initialization commit.
