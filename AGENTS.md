# Unity MCP Instructions

## Performance

When controlling Unity through MCP:

- Prefer batch operations whenever multiple actions can be performed together.
- Use `batch_execute` instead of many individual MCP calls whenever supported.
- Use parallel execution for independent operations when supported.
- Avoid checking the Unity hierarchy after every small operation.
- Avoid reading the console after every action.
- Perform changes in large batches, then verify once at the end.
- Minimize MCP round trips.
- Do not refresh Unity unless necessary.
- Do not repeatedly inspect objects whose state is already known.

## Scene Editing

When creating many GameObjects:

1. Inspect the scene once.
2. Plan all required objects and components.
3. Create/modify them using the fewest possible MCP calls.
4. Verify the hierarchy once after completion.
5. Check the console once for errors.
6. Fix only failed operations.

Prioritize fast execution over step-by-step narration.

## Scripts

- Prefer editing C# files directly when appropriate instead of performing many small Unity Editor operations.
- After changing scripts, wait for Unity compilation only when necessary.
- Avoid unnecessary AssetDatabase refreshes.

## Token and Context Efficiency

- Keep responses extremely concise while working.
- Do not explain routine MCP actions unless asked.
- Do not repeatedly summarize completed work.
- Minimize tool calls and MCP round trips.
- Batch multiple Unity operations whenever possible.
- Inspect the scene/hierarchy only when necessary.
- Do not repeatedly reread files that were already inspected.
- Read only the files relevant to the current task.
- Avoid searching the entire project unless necessary.
- Do not read Unity-generated folders such as:
  - Library/
  - Temp/
  - Logs/
  - obj/
- Make the requested changes first, then perform one final verification.
- Do not provide lengthy reasoning or step-by-step narration.
