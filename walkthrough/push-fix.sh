#!/usr/bin/env bash
# Pushes the author's fixes to the pull request opened by open-pr.sh.
# Usage: walkthrough/push-fix.sh <scenario>
#
# DRY_RUN=1 commits the fix locally and pushes nothing.
set -euo pipefail

KIT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_DIR="$(cd "$KIT_DIR/.." && pwd)"
SECOND_LOOK_URL="${SECOND_LOOK_URL:-https://second-look-review.vercel.app}"

# Everything runs inside a function so the shell has read the whole file
# before a checkout can change it on disk.
main() {
  local scenario="${1:-}"
  local scenario_dir="$KIT_DIR/scenarios/$scenario"
  local branch_file="$REPO_DIR/.git/walkthrough-$scenario"

  if [ -z "$scenario" ] || [ ! -d "$scenario_dir/fix" ]; then
    echo "Usage: walkthrough/push-fix.sh <scenario>"
    echo "Scenarios: $(ls "$KIT_DIR/scenarios" | tr '\n' ' ')"
    exit 1
  fi
  if [ ! -f "$branch_file" ]; then
    echo "No open walkthrough for '$scenario'. Run walkthrough/open-pr.sh $scenario first."
    exit 1
  fi

  local branch
  branch="$(cat "$branch_file")"
  cd "$REPO_DIR"
  git checkout -q "$branch"
  cp -R "$scenario_dir/fix/." .
  git add -A
  git commit -q -m "Address review comments"

  if [ -n "${DRY_RUN:-}" ]; then
    echo "Dry run: fix committed on $branch, nothing pushed."
    exit 0
  fi

  git push -q
  echo "Pushed the fix to $branch. A new review is starting: $SECOND_LOOK_URL/dashboard"
}

main "$@"
