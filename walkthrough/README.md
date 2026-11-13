# Walkthrough kit

Two scripts that run the whole Second Look flow on this repository with real
pull requests, so you can see every step without writing a buggy change by
hand. You need write access to this repository (see
[Try it yourself](../README.md#try-it-yourself)) and `git`. The `gh` CLI is
optional: without it the script prints a link that opens the pull request in
a browser.

```
walkthrough/open-pr.sh work-order-export
```

opens a pull request the way a non-engineer using an AI tool might: it works
on their machine and breaks platform rules. Then:

1. Open the [dashboard](https://second-look-review.vercel.app/dashboard) and
   sign in with GitHub. The pull request is there and the first pass is
   running. It takes about a minute.
2. Confirm or reject the findings, add your own if you see something it
   missed, and publish, sending it back to the author.
3. Play the author:
   ```
   walkthrough/push-fix.sh work-order-export
   ```
   pushes the fixes. A new review starts and says which findings are fixed.
4. Publish again. The check on the pull request turns green.

To try the other publish mode, choose "I will fix it myself" in step 2. The
dashboard then gives you a checkout command and a prompt for a coding agent.

You do not have to use a scenario. Any pull request you open on this
repository from a branch of it is reviewed the same way.

## Scenarios

| Name | What the change does | What is wrong with it |
|---|---|---|
| `work-order-export` | Adds a CSV export of work orders | Turns off the tenant filter, has no authorization policy, writes a file to a fixed path on the server, builds CSV by hand |
| `rename-customer-phone` | Renames a column | Written as drop then add, so existing phone numbers are deleted. No designer file, and the model snapshot is not updated |

Both have been run to the end. The results are public:
[CSV export](https://github.com/Changliu53/fieldops-platform-sample/pull/1) and
[column rename](https://github.com/Changliu53/fieldops-platform-sample/pull/2).

Each scenario folder has `change/` (the first commit), `fix/` (the author's
second commit), `title.txt`, and `body.md`. Files are copied over the
repository as they are, so adding a scenario is adding a folder. The C# in
these folders was written by hand and has not been compiled; the designer and
snapshot files follow the shape EF Core generates.

Each run makes a new branch named after the scenario and the time, so a
scenario can be run again and two people can run the same one. Set
`DRY_RUN=1` to commit locally without pushing or opening a pull request.
