# SpeckleRevit
Placeholder for the SpeckleRevit plugin. If you've got the expertise, feel free to dig in! 

## About Speckle

Speckle reimagines the design process from the Internet up: an open source (MIT) initiative for developing an extensible Design & AEC data communication protocol and platform. Contributions are welcome - we can't build this alone! 

## Development

This repo currently contains four projects:
1. [SpeckleView](https://github.com/speckleworks/SpeckleView) (Submodule) - The HTML5/JS/CSS UI based on Vue.js. Make sure that the first time you clone this repo you run `npm install`	and `npm run build` to generate the app files.
2. [SpeckleCore](https://github.com/speckleworks/SpeckleCore) (Submodule) - The Speckle .net client library.
3. SpeckleRevitConverter - TODO: How Speckle objects convert to Revit and vice versa.
4. SpeckleRevitPlugin - Integration into Revit. Check the .csproj for Before and After build targets.