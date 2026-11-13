#!/usr/bin/env bash
# Opens a pull request on this repository from a prepared scenario.
# Usage: walkthrough/open-pr.sh <scenario>
#
# DRY_RUN=1 commits on a local branch and pushes nothing.
set -euo pipefail

KIT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_DIR="$(cd "$KIT_DIR/.." && pwd)"
SECOND_LOOK_URL="${SECOND_LOOK_URL:-https://second-look-review.vercel.app}"

# The repository's page on GitHub, from the origin remote in either form.
repo_url() {
  git remote get-url origin | sed -E 's#^git@github\.com:#https://github.com/#; s#\.git$##'
}

# Everything runs inside a function so the shell has read the whole file
# before a checkout can change it on disk.
main() {
  local scenario="${1:-}"
  local scenario_dir="$KIT_DIR/scenarios/$scenario"

  if [ -z "$scenario" ] || [ ! -d "$scenario_dir/change" ]; then
    echo "Usage: walkthrough/open-pr.sh <scenario>"
    echo "Scenarios: $(ls "$KIT_DIR/scenarios" | tr '\n' ' ')"
    exit 1
  fi

  cd "$REPO_DIR"
  if [ -n "$(git status --porcelain)" ]; then
    echo "This checkout has uncommitted changes. Commit or stash them first."
    exit 1
  fi

  # A fresh branch name each time, so a scenario can be run again and two
  # people can run the same one.
  local branch="feature/$scenario-$(date +%m%d-%H%M%S)"
  local title
  title="$(head -n 1 "$scenario_dir/title.txt")"

  git checkout -q main
  if [ -z "${DRY_RUN:-}" ]; then
    git pull -q --ff-only
  fi
  git checkout -q -b "$branch"
  cp -R "$scenario_dir/change/." .
  git add -A
  git commit -q -m "$title"

  # Remembered for push-fix.sh. Kept inside .git so it is never committed.
  echo "$branch" > ".git/walkthrough-$scenario"

  if [ -n "${DRY_RUN:-}" ]; then
    echo "Dry run: committed on $branch, nothing pushed."
    exit 0
  fi

  git push -q -u origin "$branch"
  if command -v gh >/dev/null 2>&1; then
    gh pr create --base main --head "$branch" --title "$title" --body-file "$scenario_dir/body.md"
  else
    echo "The branch is pushed. Open the pull request in a browser:"
    echo "  $(repo_url)/compare/main...$branch?expand=1"
  fi
  echo "Then watch the review at $SECOND_LOOK_URL/dashboard"
}

main "$@"
