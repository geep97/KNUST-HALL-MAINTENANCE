# KNUST Hall Maintenance Reporting System

A digital hall maintenance reporting and work-order management system for
Kwame Nkrumah University of Science and Technology (KNUST).

The system replaces the traditional paper-based complaint book at hall
assistant desks with a centralized digital workflow where students can report
maintenance issues, hall staff can review and assign requests, and maintenance
personnel can manage and resolve their assigned work orders.

The system is designed around KNUST's six halls:

- Unity Hall
- Independence Hall
- Katanga Hall
- Queens Hall
- Republic Hall
- Africa Hall

Built as an ASP.NET Core MVC application and styled around KNUST's visual
identity.

---

## Core Problem

Traditional hall maintenance reporting can rely heavily on physical complaint
books and manual communication.

A student may report an issue but have limited visibility into:

- Whether the complaint was received
- Whether hall staff reviewed it
- Who was assigned to fix it
- Whether work has started
- Whether the issue has been resolved

This system digitizes that process and creates a traceable workflow from
submission through resolution.

---

# User Roles

The system has four main user roles.

## Student

Students authenticate using their KNUST student ID.

A student can:

- Sign in using their student ID and password
- View their hall and room information
- Submit maintenance complaints
- Select an issue category
- Describe the problem
- Upload a photo
- View previously submitted complaints
- Track complaint status
- View complaint history

A student's hall and room number come from their student record rather than
being manually entered when submitting a complaint.

---

## Hall Staff

Each hall has its own hall staff.

Hall staff are responsible for managing maintenance requests belonging to
their assigned hall.

For example:

```text
Unity Hall Staff
    ↓
Unity Hall complaints only
Unity Hall complaints only
