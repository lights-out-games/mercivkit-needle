# mercivkit-needle


## Setup 
```bash
cd web/
npm install
```

### Avatars: ReadyPlayerMe
Dashboard:
https://studio.readyplayer.me/applications/6574c71f97d205a206335ff2
Subdomain:
https://mercivkit-dev.readyplayer.me
App ID:
6574c71f97d205a206335ff2
Org ID:
6506bd572964e767f4b64bb5

### Multiplayer: Photon Fusion
Name:
MercivKit DEV
(Dashboard)[https://dashboard.photonengine.com/app/manage/449f6b57-c9fd-481d-85e9-01d5f6c7f1af]
App ID:
449f6b57-c9fd-481d-85e9-01d5f6c7f1af

Name: [Alon Dev]
(Dashboard)[https://dashboard.photonengine.com/app/manage/3fc7d901-5fc8-4bb9-9c6d-7b9a0e92acbf]
App ID:
3fc7d901-5fc8-4bb9-9c6d-7b9a0e92acbf


## Development
- Make a WebGL build into a staging folder (ex 'Build/WebGL').
- Name the build 'webgl' to set the prefix filenames of the core build assets which must match the paths in web/static/WebGL/index.html (otherwise you have to manually rename them after buidling)
- Copy to the following assets in the staging build folder to web/static/WebGL/Build:
  - webgl.wasm
  - webgl.data
  - webgl.framework.js
  - webgl.loader.js
- Confirm that the paths reference in web/static/WebGL/index.html match the copied files

```bash
cd web/
npm start
```

## Publish
- To build the static web app
```bash
cd web/
npm run build
```
- It'll output the directory you need to host into the "Build" folder


### Publish to Firebase Hosting
Firebase hosting 
(Dashboard)[https://console.firebase.google.com/project/mercivkit/hosting/sites/mercivkit]
Domains: 
- (mercivkit.web.app)[https://mercivkit.web.app/]
- mercivkit.firebaseapp.com[https://mercivkit.firebaseapp.com/]

```bash
firebase login

firebase deploy
```