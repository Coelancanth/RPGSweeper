# Git Commit Format

## 1. Basic Structure of a Commit Message

Each commit message should include three parts:

- **Title Line (Required)**: A brief description of the commit.
  - Limit to **50 characters**.
  - Use an imperative verb (e.g., “Add,” “Fix,” “Update”).
- **Body (Optional)**: A detailed explanation of the changes.
  - Limit each line to **72 characters** for better readability.
  - Explain the reasons for the changes, specific modifications, or impacts.
- **Footer (Optional)**: Used to reference issue tracking IDs, BREAKING CHANGES, or other metadata.

---

## 2. Commit Types

To facilitate categorization and automated changelog generation, start the commit message with one of the following types:

- **feat**: Introduce a new feature.
- **fix**: Fix an issue or bug.
- **docs**: Modify documentation only.
- **style**: Changes to code formatting (does not affect functionality, e.g., indentation, whitespace, formatting).
- **refactor**: Code refactoring (non-functional or logical changes).
- **perf**: Performance improvements.
- **test**: Add or update tests.
- **chore**: Changes to the build process, auxiliary tools, or dependencies (e.g., upgrading dependencies).
- **revert**: Revert a previous commit.
- **build**: Changes related to build scripts or configuration files (e.g., `webpack` or `package.json`).
- **ci**: Changes to CI configuration files and scripts.

---

## 3. Detailed Rules for the Title Line

1. Use imperative verbs, for example:
   - “Fix login timeout issue” instead of “Fixed login timeout issue.”
   - “Add logout button” instead of “Added logout button.”
2. Avoid vague descriptions:
   - Avoid “Update code” or “Fix bug.”
   - Use “Fix login crash when user enters invalid data.”
3. Do not end the title with a period.

---

## 4. Guidelines for Writing the Body

- **First Paragraph**: Explain the reason for the change:
  - **Why was this change made?**
  - **What issue does it address?**
- **Second Paragraph**: Detail the specific changes:
  - **What exactly was modified?**
  - **What is the impact on the codebase?**
- If the commit introduces breaking changes or requires additional steps, use **BREAKING CHANGE** to highlight it.
