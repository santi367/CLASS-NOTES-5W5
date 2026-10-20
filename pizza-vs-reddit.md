# Pizza (mock exam) → Reddit-style app

Same SignalR patterns, different context. Left column = what you coded in the mock exam, right column = what it would become in a Reddit-style app.

> This is a guess at what a Reddit-inspired exam could ask. The real exam may differ.

---

## Quick mapping

| Pizza (mock exam) | Reddit-style app | Pattern |
|---|---|---|
| `PizzaChoice` (2 pizzas) | A post / subreddit (by id) | The thing people "join" |
| `SelectChoice` → join the pizza's group | Open a post → join the post's group | `Groups.AddToGroupAsync` |
| `UnselectChoice` → leave the group | Leave the post | `Groups.RemoveFromGroupAsync` |
| `AddMoney` (+2$ for the group) | Upvote (+1 score) | Change a value on the server, then send to the group |
| `BuyPizza` (money -price, pizzas +1) | Downvote / any action that changes 2 values | One message with 2 values |
| `UpdatePizzaPrice` to `Caller` | Send the post's current score to the person who just opened it | `Clients.Caller` |
| `UpdateMoney` to `Group` | `UpdateScore` to everyone on the post | `Clients.Group` |
| `UpdateNbUsers` to `All` | Number of people online | `Clients.All` |
| `_pizzaManager` (singleton) | `_postManager` / `_redditManager` (singleton) | Shared state between Hub calls |
| (nothing) | OP-only actions (edit, delete, pin) | `[Authorize]` + `Context.UserIdentifier` (chat lab) |
| (nothing) | Notify the OP when someone replies | `Clients.User(userId)` (chat lab) |
| Q1: nested drawing events | Nested comments (replies to replies) | Tree + recursive function on the client |

---

## Caller / Group / All: the rule

| Who needs the info? | Use | Pizza example | Reddit example |
|---|---|---|---|
| Only the person who clicked | `Clients.Caller` | Pizza price when choosing | Current score when opening a post |
| Everyone on the same thing | `Clients.Group(name)` | Money, number of pizzas | Score of a post, new comment |
| Everyone connected | `Clients.All` | Number of users | Number of users, new post on the front page |
| One specific user | `Clients.User(userId)` | (not used) | "Someone replied to your post" |

---

## What changes in the code

### 1. Join / leave (same as pizza)

```csharp
// Pizza
await Groups.AddToGroupAsync(Context.ConnectionId, _pizzaManager.GetGroupName(choice));

// Reddit
await Groups.AddToGroupAsync(Context.ConnectionId, "Post" + postId);
```

Same as `CreateChannelGroupName` in the chat lab: a group name built from an id.

### 2. Upvote (same as AddMoney)

```csharp
// Pizza
_pizzaManager.IncreaseMoney(choice);
await Clients.Group(groupname).SendAsync("UpdateMoney", _pizzaManager.GetMoney(choice));

// Reddit
_postManager.Upvote(postId);
await Clients.Group("Post" + postId).SendAsync("UpdateScore", _postManager.GetScore(postId));
```

### 3. Prevent voting twice (new)

Pizza lets you click "Ajouter 2$" as many times as you want. Reddit wouldn't. You need to remember who voted, on the **server**:

```csharp
// In the manager (singleton): postId -> set of userIds who voted
Dictionary<int, HashSet<string>> votes = new();

public bool Upvote(int postId, string userId)
{
    if (!votes.ContainsKey(postId))
        votes[postId] = new HashSet<string>();

    // Add returns false if the user already voted
    if (!votes[postId].Add(userId))
        return false;

    scores[postId]++;
    return true;
}
```

Same idea as `UserHandler` in the chat lab (dictionary keyed by user), or the "no duplicates" logic from the Platforms relation lab.

### 4. OP-only actions (new, from the chat lab)

Requires authentication (`[Authorize]` on the Hub + token, like the chat lab).

```csharp
public async Task DeletePost(int postId)
{
    string userId = Context.UserIdentifier!;

    // Check on the SERVER, not just by hiding the button in React
    if (_postManager.GetAuthorId(postId) != userId)
        return;

    _postManager.Delete(postId);
    await Clients.Group("Post" + postId).SendAsync("PostDeleted");
}
```

`PostDeleted` works like `LeaveChannel` in the chat lab: kick everyone out of a deleted thing.

### 5. Notify the OP (from the chat lab)

```csharp
string opId = _postManager.GetAuthorId(postId);
await Clients.User(opId).SendAsync("NewReply", "[De: " + CurentUser.UserName + "] replied to your post");
```

Same as private messages in the chat lab.

### 6. Nested comments (from Q1 / hangman)

A comment can contain replies, which can contain replies. It's the same tree as `DrawingEvent.DrawingEvents`:

```csharp
public class Comment
{
    public string Text { get; set; }
    public List<Comment>? Replies { get; set; }
}
```

Client side, the same recursion as `applyEvents`:

```tsx
function renderComment(comment: any) {
  // display the comment...
  if (comment.replies) {
    for (const reply of comment.replies) {
      renderComment(reply);
    }
  }
}
```

---

## React side: what stays the same

| Pizza | Reddit |
|---|---|
| `on('UpdateMoney', (money) => setMoney(money))` | `on('UpdateScore', (score) => setScore(score))` |
| `on('UpdateNbPizzasAndMoney', (nb, money) => ...)` | 2 parameters, same order as `SendAsync` |
| `invoke("AddMoney", selectedChoice)` | `invoke("Upvote", postId)` |
| `invoke("UnselectChoice", ...)` **before** resetting the state | Same: invoke before you lose the id |
| `on(...)` **before** `start()` | Same |

---

## Checklist before coding (any context)

1. Read the base class / manager: which methods already exist?
2. Read what React listens for (`on(...)` names) and what it invokes.
3. For each action: change the state with the manager, then decide `Caller` / `Group` / `All` / `User`.
4. Names must match exactly between `SendAsync` ↔ `on` and `invoke` ↔ Hub method.
5. Server checks (voted twice? is OP?) go on the **server**.
