const net = require('net');
const WebSocket = require('ws');


const webSocketServer = new WebSocket.Server({ port: 7171 });
webSocketServer.on('connection', (ws) => {
  console.log("Connected client to 127.0.0.1:7171");
  let tcpSocket = new net.Socket();
  tcpSocket.connect(7070, '127.0.0.1', () => {
    console.log("Connected node.js socket to 127.0.0.1:7070");
  });
  
  tcpSocket.on('data', (data) => {
    console.log('Data received from server. ' + data.toString());
    ws.send(data.toString());
  })

  ws.on('message', (data) => {
    console.log('Data received - ' + data.toString());
    tcpSocket.write(data.toString());
  });

  ws.on('close', () => {
    console.log('Web Socket closed.');
    tcpSocket.destroy();
  })

  tcpSocket.on('close', () => {
    console.log();
    ws.close('Tcp socket.');
  })

  tcpSocket.on('error', () => {
    console.log();
    
  });
})

setInterval(() => {
}, 1000);
