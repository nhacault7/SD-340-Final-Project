# SD-340-Final-Project

These instructions were adapted from the Course Lab Submission Repository

If you need a refresher on git, see
[this high-level](https://rogerdudler.github.io/git-guide/) git guide.

For this project we will be using [Github-Flow](https://githubflow.github.io/) methodology.

# Table of Contents

- [Table of Contents](#table-of-contents)
  - [Starting Work On A Feature](#starting-work-on-a-feature)
      - [Clone the Repository](#clone-the-repository)
      - [Create Your Feature Branch](create-your-feature-branch)
      - [Add Your Branch to the Remote Repo](#add-your-branch-to-the-remote-repo)
  - [Submitting Your Feature](#submitting-your-feature)
      - [Stage and Commit Your Changes](#stage-and-commit-your-changes)
      - [Push Your Commits to Remote](#push-your-commits-to-remote)
      - [Open a Pull Request](#open-a-pull-request)
      - [Update Your Trello Card](#update-your-trello-card)
  - [Branch Organization](#branch-organization)
  - [Updating Your Code with Changes from Main](#updating-your-code-with-changes-from-main)

## Starting Work On A Feature

#### Clone the Repository

Clone a copy of this repository to your computer and setup tracking by running.   

```bash 
git clone [repo URL]
```

#### Create Your Feature Branch
*Each feature will be created in its own branch off of Main. This allows for continuous deployment as features get completed*

Create your feature branch which will be for your local solution for the feature.   
Branches should be given a short but descriptive name that relates to the feature being worked on. Spaces will be replaced by a dash 

For example, `add-priority-to-task-model`.

*There is no need to add your name to branch as each commit contains the name you assigned when setting up git. **Push to your feature branch as much as possible as it has no effect on the stable main branch***

Run the following to create your branch.  

```bash 
git checkout -b <feature-description>
``` 

#### Add Your Branch to the Remote Repo
Run the following to add your branch to the remote repository and setup tracking between your local and remote branches.

*NOTE: You will only have to run this comment once. After that, there will be a remote branch that exists on GitHub that you can simply `push` to.*

```bash 
git push -u origin <local-branch-name> 
```
## Submitting Your Feature

After you have completed the feature you will: 
1. [Stage and commit](#stage-and-commit-your-changes) your changes locally.
2. [Push your commits](#push-your-commits-to-remote) up to your remote branch.
3. [Open a pull request](#open-a-pull-request) on Github comparing your feature branch to the main branch (Pull requests will always be compared to main)
4. [Move your feature card from "Doing" to "Testing" in **Trello**](#update-your-trello-card)

#### Stage and Commit Your Changes

```bash
git status
git add .
git commit -m "<a useful commit message about your code>"
```

#### Push Your Commits to Remote

```bash
git push
```

#### Open a Pull Request

1. Go to [the Pull Request tab in GitHub](https://github.com/nhacault7/SD-340-Final-Project/pulls)
2. Press the **New Pull Request** Button
3. Set the `base` branch to the main branch.
4. Set the `compare` branch to the branch that holds your feature.
5. Press the **Create Pull Request** Button
6. Title your pull request `<Feature name> <(Your name)>`
    - For example `Add The Priority Property to the Task Model (Nicholas)` \
    Feel free to be more descriptive in your pull request than your branch name \
**Do not merge the request yet**

#### Update Your Trello Card

Take your Trello card and move it from "Doing" to "Testing". Be sure to add your features pull request link to the card as well.

Once you have done this let the rest of the group know so they can review the solution. If you are reviewing someones solution feel free to clone their solution branch into a new project on your machine to test it

If a bug or issue is found in the feature leave a comment on the card. If it is a severe issue that requires special attention a meeting can be arranged to discuss the next steps

Once everyone in the team agrees that the feature is in a completed state you may merge your feature into the main branch. Then move your Trello card to "Done". Let the group know once you have merged the feature so they know to pull the new version of main into their own local copy

**It is important to check Trello regularly for this process to work smoothly**

## Branch Organization

For this repository, we will follow the Github-Flow pattern.

*Simply create a new branch for each feature based off of the main branch. Make Sure your local main branch is always up to date with the repository main branch*

For Example:

- `Feature 1` is created based off of the `main` branch.
```bash
git checkout main
git checkout -b <feature-description>
```
- Pull the latest code changes from `main` if there are any.
```bash
git checkout main
git pull
```
- `Feature 2` is created based off of the `main` branch, and so on.

This will allow main to always have a stable version while feature branches can be in any state of testing or development.

## Updating Your Code with Changes from Main

Every time a feature is merged into main you want to update your code base with changes from the main branch.

To do so you will need to:
1. Stage and commit any current changes in your working branch.
2. Switch to the main branch.
```bash
git checkout main
```
3. Pull the latest code changes from the repository.
```bash
git pull
```
4. Switch back to your working branch where you want to add the changes.
```bash
git checkout <feature-branch>
```
5. Perform a merge to merge the changes in. *Remember to add a merge commit and close the editor.*
```bash
git merge main
```
6. Update your remote branch with the changes.
```bash
git push
```

Now continue to work as usual.