# Local Website Cache

This cache job fetches the Master Apollon article once, extracts the ranked artist blocks, and saves one deduplicated canonical image per artist into its own folder.

Example:

```powershell
python .\polite_cache.py --output-dir "C:\Users\windy\OneDrive\Desktop\Prompt Forge\cache\masterapollon"
```
