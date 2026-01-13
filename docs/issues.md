# Known Issues

## Open Issues

### ISSUE-001: Server-side prerendering disabled due to router NullReferenceException

**Status:** Open (workaround in place)

**Severity:** Low (functionality works, minor performance impact)

**Description:**
When server-side prerendering is enabled (`prerendered = true` in `AddBoleroHost`), the application crashes with a `NullReferenceException` at `Bolero.ProgramComponent.InitRouter`. This occurs because `NavigationManager` is not available during server-side rendering.

**Stack Trace:**
```
System.NullReferenceException: Object reference not set to an instance of an object.
   at Bolero.ProgramComponent`2.InitRouter(IRouter`1 router, Program`3 program)
   at CareerStoryBuilder.Client.Main.App.get_Program()
   at Bolero.ProgramComponent`2.OnInitialized()
```

**Root Cause:**
The Bolero router uses `NavigationManager` to handle client-side navigation. During SSR, this service is null because navigation is a browser-only concept.

**Current Workaround:**
Disabled prerendering in `src/Server/Program.fs`:
```fsharp
builder.Services.AddBoleroHost(prerendered = false, devToggle = true)
```

**Impact:**
- Initial page load shows "Loading..." briefly while WebAssembly initializes
- No SEO benefit from server-rendered HTML (acceptable for this authenticated app)
- Slightly slower perceived first paint

**Potential Solutions:**
1. Check `IsPrerendering` before attaching router (property not found on `ProgramComponent`)
2. Use conditional router attachment based on render mode
3. Wait for Bolero update that handles this case
4. Implement a custom `ProgramComponent` that guards router initialization

**Related Files:**
- `src/Server/Program.fs:78` - Prerendering disabled
- `src/Client/Main.fs:70-72` - Router attachment

**Added:** Phase 1A.2
