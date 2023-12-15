const crypto = require('crypto');
const express = require('express');
const { createServer } = require('http');
const WebSocket = require('ws');

const app = express();
const port = 3000;

const server = createServer(app);
const wss = new WebSocket.Server({ server });

let clients = []
let unityEditorClient = null
let browserClient = null

wss.on('connection', function(ws) {
  console.log("client joined.");
  
  // send "hello world" interval
  // const textInterval = setInterval(() => ws.send("hello world!"), 100);

  // send random bytes interval
  // const binaryInterval = setInterval(() => ws.send(crypto.randomBytes(8).buffer), 110);

  clients.push(ws)
  console.log(clients.length)

  ws.on('message', function(data) {
    if (typeof(data) === "string") {
      // client sent a string
      // console.log(data)
      console.log(`string received from client -> ${data}`);
      handleMessage(data, ws)

    } else {
      console.log("binary received from client -> " + Array.from(data).join(", ") + "");
    }
  });

  ws.on('close', function() {
    console.log("client left.");
    // clearInterval(textInterval);
    // clearInterval(binaryInterval);

    clients.splice(clients.indexOf(ws), 1)
    console.log(clients.length)
  });
});

function handleMessage(data, client) {
  data = JSON.parse(data)
  console.log(data)
  if (data.topic == 'register') {
      if (data.detail == 'browser') {
        browserClient = client
        console.log('register browser client')
        browserClient.send(packMessage(
          'registered', 
          {'event':'ready', 'args':[true]}
        ))
      } else if (data.detail == 'editor') {
        unityEditorClient = client
        console.log('register unityEditor client')
        unityEditorClient.send(packMessage(
          'registered', 
          {'event':'ready', 'args':[true]}
        ))
      } 
  } else if (data.topic == 'merciv-js-to-unity') {
    console.log('bridge message: browser > editor')
  } else if (data.topic == 'merciv-unity-to-js') {
    console.log('bridge message: editor > browser')
  }
}

function packMessage(topic, detail) {
  return JSON.stringify({
    topic,
    detail
  })
}

server.listen(port, function() {
  console.log(`Listening on http://localhost:${port}`);
});
