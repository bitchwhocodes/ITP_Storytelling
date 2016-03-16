// HTTP Portion
var http = require('http');
var fs = require('fs'); // Using the filesystem module
var Shake = require('shake.js'); //detect mobile phone shake
var httpServer = http.createServer(requestHandler);
httpServer.listen(8080);

function requestHandler(req, res) {

	fs.readFile(__dirname + '/index.html', function (err, data) {
			if (err) {
				res.writeHead(500);
				return res.end('Error loading canvas_socket.html');
			}
			res.writeHead(200);
			res.end(data);
  		}
  	);
}


var io = require('socket.io').listen(httpServer);

io.sockets.on('connection', function (socket) {
		console.log("We have a new client: " + socket.id);
	
		socket.emit('sendname', { name: 'Stacey' });
		
		socket.on('image',function(data){
			console.log("receiving image from client");
			console.log(data);
			socket.broadcast.emit('img_from_phone',data);
		});

		socket.on('chatmessage', function(data) {	
			console.log("Received: 'chatmessage' " + data);
			socket.broadcast.emit('chatmessage', data);
		});

		socket.on('location', function(data) {	
			console.log("Received: 'location' " + data);
			socket.broadcast.emit('location', data);
		});
		
		socket.on('DeviceOrientation',function(data){
			//console.log(data);
			socket.broadcast.emit('DeviceOrientation', data);
		});

		socket.on('disconnect', function() {
			console.log("Client has disconnected " + socket.id);
		});
	}
);