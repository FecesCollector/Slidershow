# Slidershow
This is a tool that downloads media files from websites.

## Commands:
  1. `get <url>` - downloads media from a url into a directory.
  2. `get <url> -g` - same as above, but forces the generic downloader.
  3. `list` - lists all directories downloaded with indeces.
  4. `load <index>` - loads a directory.
  5. `hide <index or all>` - hides a directory.
  6. `unhide <index or all>` - unhides a directory.
  7. `convert <index or all>` - converts webm files to mp4 files.
  8. `delete <index or all>` - deletes a directory.
  9. `exit` - closes the application.
  10. `open` - Opens root directory in file explorer.
  11. `combine <id1> <id2>` - Combines two directories into one. (Doesnt delete the original ones)
  
## Examples:
1. `get http://ferrohound.tumblr.com/` - This will download media using the Tumblr preset.

2. `get http://ferrohound.tumblr.com/ -g` - This will download media the Generic Downloader, desprite a Tumblr preset available.

3. `get https://www.reddit.com/r/pcmasterrace/` - This will download media using Generic Downloader because a Reddit preset doesnt exist.
